using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Management;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.SFTPClient;

namespace VisaNet.Application.Implementations
{
    public class ServiceBinFile : IServiceBinFile
    {
        private readonly IServiceBin _serviceBin;
        private readonly IServiceBinGroup _serviceBinGroup;
        private readonly IRepositoryBin _repositoryBin;
        private readonly IRepositoryParameters _repositoryParameters;
        private readonly IServiceEmailMessage _serviceEmailMessage;
        private readonly object _locker = new object();
        private readonly object _lockerAuxList = new object();
        private string _fileName;

        //Si la clave existe, el value es el id del grupo
        private IDictionary<CardTypeDto, string> _defaultBinGroups;

        public ServiceBinFile(IServiceBin serviceBin, IRepositoryBin repositoryBin, IServiceBinGroup serviceBinGroup, IRepositoryParameters repositoryParameters, IServiceEmailMessage serviceEmailMessage)
        {
            _serviceBin = serviceBin;
            _repositoryBin = repositoryBin;
            _serviceBinGroup = serviceBinGroup;
            _repositoryParameters = repositoryParameters;
            _serviceEmailMessage = serviceEmailMessage;
        }

        public BinFileProcessResultDto ProcessFile()
        {
            var sw = new Stopwatch();
            sw.Start();
            var localPath = GetFile(); //GetFileFromSFTP();
            sw.Stop();
            NLogLogger.LogEvent(NLogType.Info, "SE OBTUVO EL ARCHIVO (DURACION: " + sw.Elapsed.ToString(@"hh\:mm\:ss") + ")");
            sw.Restart();
            var lines = ReadFile(localPath);
            sw.Stop();
            NLogLogger.LogEvent(NLogType.Info, "SE LEYERON LAS LINEAS DEL ARCHIVO (DURACION: " + sw.Elapsed.ToString(@"hh\:mm\:ss") + ")");
            sw.Restart();
            //Bines que fueron tocados en el back office y no debo tocar
            var notEditableDataInBd = _serviceBin.GetBinsEditedFromBO();
            //Bines que fueron traídos de un archivo
            var editableDataInBd = _serviceBin.GetBinsNotEditedFromBO();
            sw.Stop();
            NLogLogger.LogEvent(NLogType.Info, "SE TRAJERON LOS BINES DE LA BD A MEMORIA (DURACION: " + sw.Elapsed.ToString(@"hh\:mm\:ss") + ")");
            sw.Restart();
            var result = ProcessLines(lines, editableDataInBd, notEditableDataInBd);
            sw.Stop();
            NLogLogger.LogEvent(NLogType.Info, "SE PROCESO EL ARCHIVO (DURACION: " + sw.Elapsed.ToString(@"hh\:mm\:ss") + ")");

            //Muevo el archivo a la carpeta procesados
            MoveFile();
            
            var parameter = _repositoryParameters.AllNoTracking().First();
            _serviceEmailMessage.SendBinFileProcessed(parameter, result);

            return result;
        }

        #region private methods

        private string GetFileFromSFTP()
        {
            
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "INTENTO OBTENER EL ARCHIVO DESDE SFTP");
                var options = new SftpConfigurationOptions()
                {
                    SshPrivateKeyPath = ConfigurationManager.AppSettings["HBSshPrivateKeyPath"],
                    SshPrivateKeyName = ConfigurationManager.AppSettings["HBSshPrivateKeyName"],
                    HostName = ConfigurationManager.AppSettings["HBSFTPHostName"],
                    PortNumber = int.Parse(ConfigurationManager.AppSettings["HBSFTPPortNumber"]),
                    SshHostKeyFingerprint = ConfigurationManager.AppSettings["HBSshHostKeyFingerprint"],
                    UsePassAndCertificate = true,
                    Password = ConfigurationManager.AppSettings["HBSFTPPassword"],
                    UserName = ConfigurationManager.AppSettings["HBSFTPUserName"],
                    SessionLogPath = ConfigurationManager.AppSettings["HBSFTPLogPath"] + @"\SFTPLog.txt",
                };

                var sftpClient = new SftpClient(options);

                var savepath = ConfigurationManager.AppSettings["BinFilesDirectory"];
                var fileName = ConfigurationManager.AppSettings["BinFileName"];

                //Borro archivos en la carpeta raiz
                var di = new DirectoryInfo(savepath);
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }

                sftpClient.GetFile("", fileName, savepath);

                var fullPathSaved = Path.Combine(savepath, fileName);

                //Devuelvo el directorio local en el que guarde el archivo
                return fullPathSaved;
            }
            catch (Exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "GetFileFromSFTP - Excepcion ");
                throw;
            }
        }

        private string GetFile()
        {
            NLogLogger.LogEvent(NLogType.Info, "INTENTO OBTENER EL ARCHIVO ");
            var fromPath = ConfigurationManager.AppSettings["BinFilePath"];
            GetFileName(fromPath);
            var local = ConfigurationManager.AppSettings["BinFilesDirectory"];
            var localPath = Path.Combine(local, _fileName); 
            File.Move(fromPath, localPath);
            return localPath;
        }

        private IEnumerable<string> ReadFile(string path)
        {
            return File.ReadAllLines(path);
        }

        private void MoveFile()
        {
            try
            {
                //Lo muevo a procesados agregando fecha al nombre del archivo

                var savepath = ConfigurationManager.AppSettings["BinFilesDirectory"];
                var savepathprocessed = ConfigurationManager.AppSettings["BinFilesDirectoryProcessed"];

                var sourceFile = Path.Combine(savepath, _fileName);
                var destinationFile = Path.Combine(savepathprocessed, _fileName);

                File.Move(sourceFile, destinationFile);
                File.Delete(ConfigurationManager.AppSettings["BinFilePath"]);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
            }
        }

        private BinFileProcessResultDto ProcessLines(IEnumerable<string> lines, ConcurrentDictionary<int, BinDto> editableDataInBd, ConcurrentDictionary<int, Guid> notEditableDataInBd)
        {
            var sw = new Stopwatch();
            sw.Start();
            var inserts = 0;
            var updates = 0;
            var errors = 0;

            //Como en el archivo vienen bines repetidos mantenemos una estructura auxiliar para cargar los bines ya ingresados para no tratar de volverlos a insertar o editar
            var alreadyUsedBins = new List<int>();
            SetDefaultBinGroups();
            var enumerable = lines as IList<string> ?? lines.ToList();

            var gatewayId = ConfigurationManager.AppSettings["GatewayId"];
            var user = ConfigurationManager.AppSettings["User"];

            //, new ParallelOptions { MaxDegreeOfParallelism = 1 }
            Parallel.ForEach(enumerable, new ParallelOptions { MaxDegreeOfParallelism = 1 }, line =>
            {
                try
                {
                    var strFrom = line.Substring(12, 6);
                    var strTo = line.Substring(0, 6);
                    var issuerBin = line.Substring(24, 6);
                    var processorBin = line.Substring(35, 6);
                    var country = line.Substring(43, 2);
                    var accountFundingSource = line.Substring(69, 1);

                    var from = int.Parse(strFrom);
                    var to = int.Parse(strTo);

                    var isNational = country.ToLower().Equals("uy");

                    CardTypeDto cardType;
                    switch (accountFundingSource)
                    {
                        case "C":
                        case "H":
                        case "R":
                            cardType = isNational ? CardTypeDto.Credit : CardTypeDto.InternationalCredit;
                            break;
                        case "D":
                            cardType = isNational ? CardTypeDto.Debit : CardTypeDto.InternationalDebit;
                            break;
                        case "P":
                            cardType = isNational ? CardTypeDto.NationalPrepaid : CardTypeDto.InternationalPrepaid;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("accountFundingSource");
                    }

                    //Las tarjetas de debito extranjeras las trabajamos con el default bin. Esto es para reducir la cantidad de datos (la mayoria de los bines en el archivo son debito extranjeras)
                    if (cardType != CardTypeDto.InternationalDebit)
                    {
                        Parallel.For((long)@from, to + 1, i =>
                        {
                            var binValue = (int)i;
                          
                            //Si el bin esta en la bd y fue editado en el backoffice no lo debo tocar
                            var mustContinue = !notEditableDataInBd.ContainsKey(binValue);
                            if (mustContinue)
                            {
                                lock (_lockerAuxList)
                                {
                                    if (!alreadyUsedBins.Contains(binValue))
                                    {
                                        //Si no lo procese al bin lo agrego a la estructura auxiliar y sigo
                                        alreadyUsedBins.Add(binValue);
                                    }
                                    else
                                    {
                                        //Si ya lo procese al bin en otra linea, no lo debo tocar
                                        mustContinue = false;
                                    }
                                }
                                if (mustContinue)
                                {
                                    AuthorizationAmountTypeDto authorizationAmountType;
                                    switch (cardType)
                                    {
                                        case CardTypeDto.InternationalDebit:
                                        case CardTypeDto.InternationalCredit:
                                        case CardTypeDto.InternationalPrepaid:
                                        case CardTypeDto.Debit:
                                            authorizationAmountType = AuthorizationAmountTypeDto.Net;
                                            break;
                                        case CardTypeDto.Credit:
                                        case CardTypeDto.NationalPrepaid:
                                            authorizationAmountType = AuthorizationAmountTypeDto.Gross;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException("cardType");
                                    }


                                    var binGroupId = _defaultBinGroups.ContainsKey(cardType) ? _defaultBinGroups[cardType] : null;

                                    if (editableDataInBd.ContainsKey(binValue))
                                    {
                                        BinDto databaseBin;
                                        editableDataInBd.TryRemove(binValue, out databaseBin);

                                        var readedBin = new BinDto
                                        {
                                            Active = true,
                                            Country = country,
                                            Name = ((int)i).ToString(),
                                            Value = (int)i,
                                            GatewayId = new Guid(gatewayId),
                                            CardType = cardType,
                                            IssuerBin = issuerBin,
                                            ProcessorBin = processorBin,
                                            BinAuthorizationAmountTypeDtoList = new List<BinAuthorizationAmountTypeDto>()
                                            {
                                                new BinAuthorizationAmountTypeDto()
                                                {
                                                    LawDto = DiscountTypeDto.FinancialInclusion,
                                                    AuthorizationAmountTypeDto = authorizationAmountType,

                                                },
                                                new BinAuthorizationAmountTypeDto()
                                                {
                                                    LawDto = DiscountTypeDto.TourismOrTaxReintegration,
                                                    AuthorizationAmountTypeDto = authorizationAmountType,
                                                }
                                            }
                                        };

                                        CheckNationalBins(databaseBin, readedBin);
                                        
                                        if (_serviceBin.MustUpdate(databaseBin, readedBin))
                                        {
                                            var oldDefaultGroupId = _defaultBinGroups.ContainsKey(databaseBin.CardType) ? _defaultBinGroups[databaseBin.CardType] : null;
                                            Interlocked.Increment(ref updates);
                                            lock (_locker)
                                            {
                                                _repositoryBin.ExecuteBinManagerSp_Update(databaseBin.Id, binValue.ToString().PadLeft(6, '0'), gatewayId, (int)cardType, (int)authorizationAmountType, country, issuerBin, processorBin, user, binGroupId, oldDefaultGroupId, false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Interlocked.Increment(ref inserts);
                                        lock (_locker)
                                        {
                                            _repositoryBin.ExecuteBinManagerSp_insert(binValue.ToString().PadLeft(6, '0'), binValue, gatewayId, (int)cardType, (int)authorizationAmountType, country, issuerBin, processorBin, user, binGroupId, false);
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    NLogLogger.LogEvent(NLogType.Error, "ERROR EN LA LINEA: " + Environment.NewLine + e.ToString());
                    Interlocked.Increment(ref errors);
                }
            });

            sw.Stop();
            NLogLogger.LogEvent(NLogType.Info, "INSERTS/UPDATES (DURACION: " + sw.Elapsed.ToString(@"hh\:mm\:ss") + ")");
            sw.Restart();

            var deletes = editableDataInBd.Count();

            //Borro los bines que esten editables en la base de datos y no haya sido leídos en el archivo
            Parallel.ForEach(editableDataInBd, data =>
            {
                lock (_locker)
                {
                    _repositoryBin.ExecuteBinManagerSp_Delete(data.Value.Id);
                }
            });

            sw.Stop();
            NLogLogger.LogEvent(NLogType.Info, "DELETE (DURACION: " + sw.Elapsed.ToString(@"hh\:mm\:ss") + ")");
            return new BinFileProcessResultDto
            {
                Deletes = deletes,
                Inserts = inserts,
                Updates = updates,
                Errors = errors,
                LinesProcessed = enumerable.Count()
            };
        }

        private void SetDefaultBinGroups()
        {
            _defaultBinGroups = new Dictionary<CardTypeDto, string>();

            foreach (CardTypeDto type in Enum.GetValues(typeof(CardTypeDto)))
            {
                var id = _serviceBin.GetDefaultGroupBin(type);
                var group = _serviceBinGroup.GetById(Guid.Parse(id));
                if (group != null)
                {
                    _defaultBinGroups.Add(type, id);
                }
            }
        }

        private void GetFileName(string path)
        {
            var array = path.Split('\\');
            var name = array[array.Length - 1 ];
            _fileName = DateTime.Now.ToString("yyyyMMddHHmmss_") + name;
        }


        // bin_bd, tipo_bd, pais_bd, bin_file, tipo_file, pais_file
        private Task CheckNationalBins(BinDto oldBin, BinDto newBin)
        {
            if (string.Compare(oldBin.Country, "UY", StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(newBin.Country, "UY", StringComparison.OrdinalIgnoreCase) != 0 ||
                (string.Compare(oldBin.Country, "UY", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(newBin.Country, "UY", StringComparison.OrdinalIgnoreCase) == 0))
            {
                //UN BIN ES DE UY Y EL OTRO NO
                NLogLogger.LogEvent(NLogType.Info, string.Format("Dif entre bines: {0},{01},{02},{03},{04},{05}",
                    oldBin.Value, oldBin.CardType, oldBin.Country, newBin.Value, newBin.CardType, newBin.Country));
            }

            if (string.Compare(oldBin.Country, "UY", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(newBin.Country, "UY", StringComparison.OrdinalIgnoreCase) == 0)
            {
                //AL MENOS UNO ES DE UY
                if (oldBin.CardType != newBin.CardType)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Dif entre bines: {0},{01},{02},{03},{04},{05}", 
                        oldBin.Value, oldBin.CardType, oldBin.Country, newBin.Value, newBin.CardType, newBin.Country));
                }
            }
            return null;
        }

        #endregion

        
    }
}