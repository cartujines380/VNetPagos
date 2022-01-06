using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cryptography;
using VisaNet.Utilities.Helpers;
using LogType = VisaNet.Common.Logging.Entities.LogType;

namespace VisaNet.Application.Implementations
{
    public class ServiceApplicationUser : BaseService<ApplicationUser, ApplicationUserDto>, IServiceApplicationUser
    {
        private readonly IRepositoryMembershipUser _repositoryMembershipUser;
        private readonly ILoggerService _loggerService;
        private readonly IRepositorySistarbancUser _repositorySistarbancUser;
        private readonly IRepositorySubscriber _repositorySubscriber;
        private readonly IServiceAnalyzeCsCall _serviceAnalyzeCsCall;
        private readonly IRepositoryApplicationUser _repository;
        private readonly IRepositoryServiceAsociated _repositoryServiceAsociated;
        private readonly IServiceSubscriber _serviceSubscriber;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly IServiceSystemUser _serviceSystemUser;

        public ServiceApplicationUser(IRepositoryApplicationUser repository, IRepositoryMembershipUser repositoryMembershipUser, ILoggerService loggerService,
            IRepositorySistarbancUser repositorySistarbancUser, IServiceAnalyzeCsCall serviceAnalyzeCsCall, IRepositorySubscriber repositorySubscriber,
            IRepositoryServiceAsociated repositoryServiceAsociated, IServiceSubscriber serviceSubscriber, IServiceEmailMessage serviceNotificationMessage,
            IServiceSystemUser serviceSystemUser)
            : base(repository)
        {
            _repository = repository;
            _repositoryMembershipUser = repositoryMembershipUser;
            _loggerService = loggerService;
            //_serviceServiceAssosiate = serviceServiceAssosiate;
            _repositorySistarbancUser = repositorySistarbancUser;
            _repositorySubscriber = repositorySubscriber;
            _serviceAnalyzeCsCall = serviceAnalyzeCsCall;
            _repositoryServiceAsociated = repositoryServiceAsociated;
            _serviceNotificationMessage = serviceNotificationMessage;
            _serviceSystemUser = serviceSystemUser;
            _serviceSubscriber = serviceSubscriber;
        }

        public override IQueryable<ApplicationUser> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override ApplicationUserDto Converter(ApplicationUser entity)
        {
            var userDto = new ApplicationUserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Address = entity.Address,
                CallCenterKey = entity.CallCenterKey,
                Email = entity.Email,
                IdentityNumber = entity.IdentityNumber,
                MembreshipIdentifier = entity.MembershipIdentifier,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
                //ServicesAssociated = entity.ServicesAssociated != null ? entity.ServicesAssociated.ToList() : new List<ServiceAssociated>(),
                SistarbancUserId = entity.SistarbancUserId,
                SistarbancUser = entity.SistarbancUser == null ? null : new SistarbancUserDto()
                {
                    Active = entity.SistarbancUser.Active,
                    UniqueIdentifier = entity.SistarbancUser.UniqueIdentifier
                },
                SistarbancBrouUserId = entity.SistarbancBrouUserId,
                SistarbancBrouUser = entity.SistarbancBrouUser == null ? null : new SistarbancUserDto()
                {
                    Active = entity.SistarbancBrouUser.Active,
                    UniqueIdentifier = entity.SistarbancBrouUser.UniqueIdentifier
                },
                Platform = (PlatformDto)entity.Platform,
                CreationDate = entity.CreationDate,
                CyberSourceIdentifier = entity.CyberSourceIdentifier,

            };
            if (entity.Cards != null && entity.Cards.Any())
            {
                userDto.CardDtos = new List<CardDto>();
                foreach (var card in entity.Cards)
                {
                    userDto.CardDtos.Add(new CardDto()
                    {
                        DueDate = card.DueDate,
                        MaskedNumber = card.MaskedNumber,
                        Id = card.Id,
                        PaymentToken = card.PaymentToken,
                        Active = card.Active,
                        Deleted = card.Deleted,
                        ExternalId = card.ExternalId,
                        Description = card.Description
                    });
                }
            }
            if (entity.Payments != null && entity.Payments.Any())
            {
                userDto.PaymentDtos = new List<PaymentDto>();
                foreach (var payment in entity.Payments)
                {
                    userDto.PaymentDtos.Add(new PaymentDto()
                    {
                        Id = payment.Id,
                        TransactionNumber = payment.TransactionNumber
                    });
                }
            }
            if (entity.MembershipIdentifierObj != null)
            {
                userDto.MembershipIdentifierObj = new MembershipUserDto
                {
                    Active = entity.MembershipIdentifierObj.Active,
                    LastAttemptToLogIn = entity.MembershipIdentifierObj.LastAttemptToLogIn,
                    FailLogInCount = entity.MembershipIdentifierObj.FailLogInCount,
                    LastResetPassword = entity.MembershipIdentifierObj.LastResetPassword,
                    ConfirmationToken = entity.MembershipIdentifierObj.ConfirmationToken,
                    ResetPasswordToken = entity.MembershipIdentifierObj.ResetPasswordToken,
                    Id = entity.MembershipIdentifierObj.Id,
                    Blocked = entity.MembershipIdentifierObj.Blocked,
                    PasswordHasBeenChangedFromBO = entity.MembershipIdentifierObj.PasswordHasBeenChangedFromBO
                };
            }
            if (entity.ServicesAssociated != null)
            {
                userDto.ServicesAssociated = new List<ServiceAssociatedDto>();
                foreach (var sa in entity.ServicesAssociated)
                {
                    userDto.ServicesAssociated.Add(new ServiceAssociatedDto()
                    {
                        Id = sa.Id,
                    });
                }
            }
            return userDto;
        }

        public override ApplicationUser Converter(ApplicationUserDto entity)
        {
            var user = new ApplicationUser
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Address = entity.Address,
                CallCenterKey = entity.CallCenterKey,
                Email = entity.Email,
                IdentityNumber = entity.IdentityNumber,
                MembershipIdentifier = entity.MembreshipIdentifier,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
                CyberSourceIdentifier = entity.CyberSourceIdentifier,
                Platform = (Platform)entity.Platform,
            };
            //if (entity.NewCardDto != null)
            //{
            //    //tarjeta nueva
            //    var card = new Card()
            //    {
            //        DueDate = entity.NewCardDto.DueDate,
            //        MaskedNumber = entity.NewCardDto.MaskedNumber,
            //        PaymentToken = entity.NewCardDto.PaymentToken,
            //        CybersourceTransactionId = entity.NewCardDto.CybersourceTransactionId,
            //        Id = entity.NewCardDto.Id,
            //        Active = entity.NewCardDto.Active
            //    };
            //    if (card.Id == Guid.Empty)
            //    {
            //        card.GenerateNewIdentity();
            //    }

            //    if (sa.Cards == null)
            //        sa.Cards = new List<Card>();

            //    sa.Cards.Add(card);
            //}

            return user;
        }

        public IEnumerable<ApplicationUserDto> GetDataForTable(ApplicationUserFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.GenericSearch.ToLower())
                                       || sc.Surname.ToLower().Contains(filters.GenericSearch.ToLower())
                                       || sc.Email.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(sc => sc.Name.Equals(filters.Name, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(filters.Surname))
                query = query.Where(sc => sc.Surname.Equals(filters.Surname, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrEmpty(filters.Email))
                query = query.Where(sc => sc.Email.Equals(filters.Email, StringComparison.InvariantCultureIgnoreCase));

            if (filters.SortDirection == SortDirection.Asc)
                query = query.OrderByStringProperty(filters.OrderBy);
            else
                query = query.OrderByStringPropertyDescending(filters.OrderBy);

            query = query.Skip(filters.DisplayStart);

            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            return query.Select(i => new ApplicationUserDto
            {
                Id = i.Id,
                Name = i.Name,
                Surname = i.Surname,
                Email = i.Email,
            }).ToList();
        }

        public void Create(ApplicationUserCreateEditDto entity)
        {
            try
            {
                var userTuple = CreateUser(entity);
                var applicationUser = userTuple.Item1;
                var membershipUser = userTuple.Item2;

                //Se envia correo al usuario para que confirme
                _serviceNotificationMessage.SendNewUserEmail(applicationUser, membershipUser);
                _loggerService.CreateLog(LogType.Info, LogOperationType.UserCreation, LogCommunicationType.VisaNet, applicationUser.Id, string.Format(LogStrings.ServiceApplicationUser_CreationUser, applicationUser.Email));
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        public ApplicationUserDto CreateUserWithoutPassword(ApplicationUserCreateEditDto entity, long? cybersourceIdentifier = null)
        {
            try
            {
                entity.Password = GenerateRandomPassword();
                var userTuple = CreateUser(entity, cybersourceIdentifier);
                var applicationUser = userTuple.Item1;
                var membershipUser = userTuple.Item2;

                //Se genera token para cambio de contraseña
                var tokenFormat = string.Format("{0}|{1}", entity.Email, DateTime.Now.AddMinutes(30).Ticks);
                var token = RijndaelSecurity.Encrypt(tokenFormat);
                membershipUser.LastResetPassword = DateTime.Now;
                membershipUser.ResetPasswordToken = UrlHelper.Base64ForUrlEncode(token);
                _repositoryMembershipUser.Edit(membershipUser);
                Repository.Save();

                //Se envia correo al usuario para que confirme y cambie la contraseña
                _serviceNotificationMessage.SendNewUserRequestPassword(applicationUser, membershipUser);
                _loggerService.CreateLog(LogType.Info, LogOperationType.UserCreation, LogCommunicationType.VisaNet, applicationUser.Id, string.Format(LogStrings.ServiceApplicationUser_CreationUser, applicationUser.Email));

                return Converter(applicationUser);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        private Tuple<ApplicationUser, MembershipUser> CreateUser(ApplicationUserCreateEditDto entity, long? cybersourceIdentifier = null)
        {
            try
            {
                entity.Email = entity.Email.Trim();
                entity.IdentityNumber = entity.IdentityNumber != null ? entity.IdentityNumber.Trim() : string.Empty;

                if (Repository.AllNoTracking(u => u.Email.Equals(entity.Email, StringComparison.InvariantCultureIgnoreCase)).Any())
                    throw new BusinessException(CodeExceptions.APPLICATION_USER_EMAIL_DUPLICATED);

                //El campo CI no es requerido
                //if (Repository.AllNoTracking(u => u.IdentityNumber.Equals(entity.IdentityNumber, StringComparison.InvariantCultureIgnoreCase)).Any())
                //    throw new BusinessException(CodeExceptions.APPLICATION_USER_IDENTITY_DUPLICATED);

                var membershipIdentifier = Guid.NewGuid();

                var applicationUser = new ApplicationUser
                {
                    Name = !string.IsNullOrEmpty(entity.Name) ? entity.Name.Trim() : string.Empty,
                    Surname = !string.IsNullOrEmpty(entity.Surname) ? entity.Surname.Trim() : string.Empty,
                    Email = entity.Email.Trim(),
                    Address = !string.IsNullOrEmpty(entity.Address) ? entity.Address.Trim() : string.Empty,
                    CallCenterKey = !string.IsNullOrEmpty(entity.CallCenterKey) ? entity.CallCenterKey.Trim() : string.Empty,
                    IdentityNumber = !string.IsNullOrEmpty(entity.IdentityNumber) ? entity.IdentityNumber.Trim() : string.Empty,
                    MembershipIdentifier = membershipIdentifier,
                    MobileNumber = !string.IsNullOrEmpty(entity.MobileNumber) ? entity.MobileNumber.Trim() : string.Empty,
                    PhoneNumber = !string.IsNullOrEmpty(entity.PhoneNumber) ? entity.PhoneNumber.Trim() : string.Empty,
                };

                applicationUser.GenerateNewIdentity();

                var passwordSalt = string.Empty;
                var passwordHash = string.Empty;
                if (entity.PasswordAlreadyHashed)
                {
                    PasswordHash.ReadPasswordFromApps(entity.Password, out passwordHash, out passwordSalt);
                }
                else
                {
                    passwordSalt = PasswordHash.CreateSalt();
                    passwordHash = PasswordHash.CreatePasswordHash(entity.Password.Trim(), passwordSalt);
                }

                var tokenFormat = string.Format("{0}|{1}", applicationUser.Email, Guid.NewGuid());
                var token = RijndaelSecurity.Encrypt(tokenFormat);

                var membershipUser = new MembershipUser
                {
                    Id = membershipIdentifier,
                    Password = passwordHash,
                    PasswordSalt = passwordSalt,
                    Active = false,
                    FailLogInCount = 0,
                    ConfirmationToken = UrlHelper.Base64ForUrlEncode(token),
                };

                applicationUser.SistarbancUser = new SistarbancUser();
                applicationUser.SistarbancUser.GenerateNewIdentity();

                _repositorySistarbancUser.Create(applicationUser.SistarbancUser);

                applicationUser.SistarbancBrouUser = new SistarbancUser();
                applicationUser.SistarbancBrouUser.GenerateNewIdentity();

                if (cybersourceIdentifier != null)
                {
                    applicationUser.CyberSourceIdentifier = cybersourceIdentifier.Value;
                }
                else
                {
                    //OBTENGO EL SIGUIENTE CyberSourceIdentifier
                    applicationUser.CyberSourceIdentifier = GetNextCyberSourceIdentifier();
                }

                _repositorySistarbancUser.Create(applicationUser.SistarbancBrouUser);

                var subscriberUser = new Subscriber
                {
                    Email = entity.Email.Trim(),
                    Name = entity.Name,
                    Surname = entity.Surname
                };

                _repositoryMembershipUser.Create(membershipUser);
                //_repositoryMembershipUser.Save(); //VER: esta linea no estaba originalmente. Si no se agrega se cae desde los tests.

                _repositorySubscriber.Create(subscriberUser);

                Repository.Create(applicationUser);
                Repository.Save();
                return new Tuple<ApplicationUser, MembershipUser>(applicationUser, membershipUser);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        public override void Edit(ApplicationUserDto entity)
        {
            if (Repository.AllNoTracking(u => u.Id != entity.Id && u.Email.Equals(entity.Email, StringComparison.InvariantCultureIgnoreCase)).Any())
                throw new BusinessException(CodeExceptions.APPLICATION_USER_EMAIL_DUPLICATED);

            //El campo CI no es requerido
            //if (Repository.AllNoTracking(u => u.Id != entity.Id && u.IdentityNumber.Equals(entity.IdentityNumber, StringComparison.InvariantCultureIgnoreCase)).Any())
            //    throw new BusinessException(CodeExceptions.APPLICATION_USER_IDENTITY_DUPLICATED);


            var efEntity = Repository.All(u => u.Id == entity.Id).FirstOrDefault();

            if (efEntity == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            //RO: Copio y pego lógica que estaba en el ProfileController -- INICIO
            if (!entity.RecieveNewsletter)
            {
                _serviceSubscriber.DeleteByEmail(entity.Email);
            }
            else
            {
                var existsSubscriber = _serviceSubscriber.ExistsEmail(entity.Email);
                if (!existsSubscriber)
                {
                    _serviceSubscriber.Create(new SubscriberDto
                    {
                        Email = entity.Email,
                        Name = entity.Name,
                        Surname = entity.Surname
                    });
                }
            }
            //RO: Copio y pego lógica que estaba en el ProfileController -- FIN

            efEntity.Name = entity.Name.Trim();
            efEntity.Surname = entity.Surname.Trim();
            efEntity.Email = entity.Email.Trim();
            efEntity.Address = entity.Address == null ? String.Empty : entity.Address.Trim();
            efEntity.CallCenterKey = entity.CallCenterKey.Trim();
            efEntity.IdentityNumber = entity.IdentityNumber == null ? String.Empty : entity.IdentityNumber.Trim();
            efEntity.MobileNumber = entity.MobileNumber == null ? String.Empty : entity.MobileNumber.Trim();
            efEntity.PhoneNumber = entity.PhoneNumber == null ? String.Empty : entity.PhoneNumber.Trim();

            Repository.Edit(efEntity);
            Repository.Save();

            _loggerService.CreateLog(LogType.Info, LogOperationType.UserEdit, LogCommunicationType.VisaNet, LogStrings.ServiceApplicationUser_EditUser);
        }

        public void EditFromBO(ApplicationUserDto entity)
        {

            Repository.ContextTrackChanges = true;
            var efEntity = Repository.All(u => u.Id == entity.Id).FirstOrDefault();

            if (efEntity == null)
            {
                Repository.ContextTrackChanges = false;
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);
            }
            efEntity.Name = entity.Name.Trim();
            efEntity.Surname = entity.Surname.Trim();
            efEntity.Address = entity.Address == null ? String.Empty : entity.Address.Trim();
            efEntity.CallCenterKey = entity.CallCenterKey.Trim();
            efEntity.IdentityNumber = entity.IdentityNumber == null ? String.Empty : entity.IdentityNumber.Trim();
            efEntity.MobileNumber = entity.MobileNumber == null ? String.Empty : entity.MobileNumber.Trim();
            efEntity.PhoneNumber = entity.PhoneNumber == null ? String.Empty : entity.PhoneNumber.Trim();
            efEntity.Platform = (Platform)entity.Platform;

            Repository.Edit(efEntity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
            _loggerService.CreateLog(LogType.Info, LogOperationType.UserEdit, LogCommunicationType.VisaNet, LogStrings.ServiceApplicationUser_EditUser);
        }

        public void ChangePassword(Guid id, string email, string oldPassword, string newPassword)
        {
            var membershipIdentifier = Repository.All(u => u.Id == id && u.Email == email).Select(u => u.MembershipIdentifier).FirstOrDefault();

            if (membershipIdentifier == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            var membershipUser =
                _repositoryMembershipUser.All(u => u.Id == membershipIdentifier).FirstOrDefault();

            if (membershipUser == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            var oldPasswordEncrypted = PasswordHash.CreatePasswordHash(oldPassword, membershipUser.PasswordSalt);

            if (membershipUser.Password != oldPasswordEncrypted)
                throw new BusinessException(CodeExceptions.APPLICATION_USER_OLD_PASSWORD_NOT_MATCH);


            membershipUser.Password = PasswordHash.CreatePasswordHash(newPassword, membershipUser.PasswordSalt);

            _repositoryMembershipUser.Edit(membershipUser);
            Repository.Save();

            _loggerService.CreateLog(LogType.Info, LogOperationType.UserChangePassword, LogCommunicationType.VisaNet, LogStrings.ServiceApplicationUser_ChangePassword);
        }

        public void ChangePasswordWeb(string email, string oldPassword, string newPassword)
        {
            var membershipIdentifier = Repository.All(u => u.Email == email).Select(u => u.MembershipIdentifier).FirstOrDefault();

            if (membershipIdentifier == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            var membershipUser =
                _repositoryMembershipUser.All(u => u.Id == membershipIdentifier).FirstOrDefault();

            if (membershipUser == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            var oldPasswordEncrypted = PasswordHash.CreatePasswordHash(oldPassword, membershipUser.PasswordSalt);

            if (membershipUser.Password != oldPasswordEncrypted)
                throw new BusinessException(CodeExceptions.APPLICATION_USER_OLD_PASSWORD_NOT_MATCH);

            membershipUser.Password = PasswordHash.CreatePasswordHash(newPassword, membershipUser.PasswordSalt);
            membershipUser.PasswordHasBeenChangedFromBO = false;

            _repositoryMembershipUser.Edit(membershipUser);
            Repository.Save();

            _loggerService.CreateLog(LogType.Info, LogOperationType.UserChangePassword, LogCommunicationType.VisaNet, LogStrings.ServiceApplicationUser_ChangePassword);
        }

        public void ChangePasswordFromBO(Guid id, string newPassword)
        {
            var membershipIdentifier = Repository.All(u => u.Id == id).Select(u => u.MembershipIdentifier).FirstOrDefault();

            if (membershipIdentifier == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            var membershipUser =
                _repositoryMembershipUser.All(u => u.Id == membershipIdentifier).FirstOrDefault();

            if (membershipUser == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            membershipUser.Password = PasswordHash.CreatePasswordHash(newPassword, membershipUser.PasswordSalt);
            membershipUser.PasswordHasBeenChangedFromBO = true;

            _repositoryMembershipUser.Edit(membershipUser);
            Repository.Save();

            _loggerService.CreateLog(LogType.Info, LogOperationType.UserChangePassword, LogCommunicationType.VisaNet, LogStrings.ServiceApplicationUser_ChangePassword);
        }

        /// <summary>
        /// Envia email de cambio de contraseña o activacion de cuenta
        /// </summary>
        /// <param name="email"></param>
        /// <returns>
        /// 1 = cambio de contraseña
        /// 2 = activacion de usuario 
        /// </returns>
        public int ResetPassword(string email)
        {
            var userData = Repository.All(u => u.Email == email).FirstOrDefault();

            if (userData == null)
                throw new BusinessException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            var membershipUser =
                _repositoryMembershipUser.All(u => u.Id == userData.MembershipIdentifier).FirstOrDefault();

            if (membershipUser == null)
                throw new BusinessException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            if (membershipUser.Blocked)
            {
                throw new BusinessException(CodeExceptions.APPLICATION_USER_DISABLED);
            }

            var tokenFormat = string.Format("{0}|{1}", email, DateTime.Now.AddMinutes(30).Ticks);
            var token = RijndaelSecurity.Encrypt(tokenFormat);

            membershipUser.LastResetPassword = DateTime.Now;
            membershipUser.ResetPasswordToken = UrlHelper.Base64ForUrlEncode(token);

            _serviceNotificationMessage.SendResetPassword(userData, membershipUser);
            _repositoryMembershipUser.Edit(membershipUser);
            Repository.Save();

            _loggerService.CreateLog(LogType.Info, LogOperationType.UserResetPassword, LogCommunicationType.VisaNet, LogStrings.ServiceApplicationUser_ResetPassword);
            return 1;
        }

        public bool ResetPasswordFromToken(ResetPasswordFromTokenDto model)
        {
            Guid? userId = null;
            try
            {
                var decryptedToken = RijndaelSecurity.Decrypt(UrlHelper.Base64ForUrlDecode(model.Token));
                var splitedToken = decryptedToken.Split('|');

                var emailToken = splitedToken[0];
                var dateLimitTicsToken = splitedToken[1];

                var userData = Repository.All(u => u.Email == emailToken).Select(u => new { u.MembershipIdentifier, u.Id }).FirstOrDefault();
                if (userData == null)
                {
                    throw new FatalException(CodeExceptions.APPLICATION_USER_MISSING_MEMBERSHIP_IDENTIFIER);
                }

                userId = userData.Id;

                if ((model.UserName.ToLower() != emailToken.ToLower()) || (long.Parse(dateLimitTicsToken) <= DateTime.Now.Ticks))
                {
                    throw new BusinessException(CodeExceptions.APPLICATION_USER_INVALID_TOKEN);
                }

                if (userData.MembershipIdentifier == Guid.Empty)
                {
                    throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);
                }
                var membershipUser =
                    _repositoryMembershipUser.All(u => u.Id == userData.MembershipIdentifier).FirstOrDefault();

                if (membershipUser == null)
                {
                    throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);
                }

                if (membershipUser.Blocked)
                {
                    throw new FatalException(CodeExceptions.APPLICATION_USER_DISABLED);
                }

                if (membershipUser.ResetPasswordToken == null || membershipUser.ResetPasswordToken != model.Token)
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("ResetPasswordFromToken Error - El token enviado {0} no concuerda con el del BD {1}",
                        model.Token, membershipUser.ResetPasswordToken));
                    throw new BusinessException(CodeExceptions.APPLICATION_USER_INVALID_TOKEN);
                }

                membershipUser.Password = PasswordHash.CreatePasswordHash(model.Password, membershipUser.PasswordSalt);
                membershipUser.FailLogInCount = 0;
                membershipUser.Active = true;
                membershipUser.PasswordHasBeenChangedFromBO = false;

                membershipUser.ResetPasswordToken = null;

                _repositoryMembershipUser.Edit(membershipUser);
                _repositoryMembershipUser.Save();

                _loggerService.CreateLog(LogType.Info, LogOperationType.UserResetPassword, LogCommunicationType.VisaNet, userData.Id,
                    string.Format(LogStrings.ServiceApplicationUser_ResetPasswordFromTokenConfirmation));

                return true;
            }
            catch (BusinessException ex)
            {
                if (userId != null)
                {
                    _loggerService.CreateLog(LogType.Info, LogOperationType.UserResetPassword, LogCommunicationType.VisaNet, userId.Value,
                        string.Format(LogStrings.ServiceApplicationUser_ResetPasswordFromTokenFail, ex.Message));
                }

                throw new BusinessException(ex.Message);
                //return false;
            }
            catch (FatalException ex)
            {
                _loggerService.CreateLog(LogType.Info, LogOperationType.UserResetPassword, LogCommunicationType.VisaNet, null,
                    string.Format(LogStrings.ServiceApplicationUser_ResetPasswordFromTokenConfirmation, ex.Message));

                throw new FatalException(ex.Message);
                //return false;
            }
        }

        public bool ValidateUser(string email, string password)
        {
            MembershipUser membershipUser = null;
            ApplicationUser applicationUser = null;
            try
            {
                var userData = Repository.All(u => u.Email == email).Select(u => new { u.MembershipIdentifier, u.Id }).FirstOrDefault();
                if (userData == null)
                    throw new FatalException(CodeExceptions.APPLICATION_USER_MISSING_MEMBERSHIP_IDENTIFIER);

                applicationUser = new ApplicationUser
                {
                    Id = userData.Id,
                    MembershipIdentifier = userData.MembershipIdentifier,
                };

                membershipUser = _repositoryMembershipUser.All(u => u.Id == applicationUser.MembershipIdentifier).FirstOrDefault();

                if (membershipUser == null)
                    throw new FatalException(CodeExceptions.USER_NOT_EXIST);

                if (membershipUser.Active == false)
                    throw new BusinessException(CodeExceptions.SYSTEM_USER_DISABLED);

                if (membershipUser.Password == PasswordHash.CreatePasswordHash(password, membershipUser.PasswordSalt) == false)
                    throw new BusinessException(CodeExceptions.APPLICATION_USER_INCORRECT_PASSWORD);

                if (membershipUser.Blocked)
                    throw new FatalException(CodeExceptions.APPLICATION_USER_DISABLED);

                if (membershipUser.FailLogInCount > 0)
                    RegisterLogInAttemptSuccess(membershipUser);

                return true;
            }
            catch (BusinessException)
            {
                if (membershipUser != null)
                    RegisterLogInAttemptFail(applicationUser.Id, membershipUser);

                return false;
            }
            catch (FatalException)
            {
                _loggerService.CreateLog(LogType.Info, LogOperationType.LogInFail, LogCommunicationType.VisaNet, string.Format(LogStrings.ServiceApplicationUser_LogInFail_UserNotExist, email, ""));
                return false;
            }
        }

        public ValidateUserResponse ValidateUserWeb(string email, string password)
        {
            MembershipUser membershipUser = null;
            ApplicationUser applicationUser = null;
            try
            {
                var userData = Repository.All(u => u.Email == email).Select(u => new { u.MembershipIdentifier, u.Id }).FirstOrDefault();
                if (userData == null)
                    throw new FatalException(CodeExceptions.APPLICATION_USER_MISSING_MEMBERSHIP_IDENTIFIER);

                applicationUser = new ApplicationUser
                {
                    Id = userData.Id,
                    MembershipIdentifier = userData.MembershipIdentifier,
                };

                membershipUser = _repositoryMembershipUser.All(u => u.Id == applicationUser.MembershipIdentifier).FirstOrDefault();

                if (membershipUser == null)
                    throw new FatalException(CodeExceptions.USER_NOT_EXIST);

                if (membershipUser.Active == false)
                    throw new BusinessException(CodeExceptions.SYSTEM_USER_DISABLED);

                if (membershipUser.Password == PasswordHash.CreatePasswordHash(password, membershipUser.PasswordSalt) == false)
                    throw new BusinessException(CodeExceptions.APPLICATION_USER_INCORRECT_PASSWORD);

                if (membershipUser.Blocked)
                    throw new FatalException(CodeExceptions.APPLICATION_USER_DISABLED);

                if (membershipUser.PasswordHasBeenChangedFromBO)
                    return ValidateUserResponse.Valid_Change_Password;

                if (membershipUser.FailLogInCount > 0)
                    RegisterLogInAttemptSuccess(membershipUser);

                return ValidateUserResponse.Valid;
            }
            catch (BusinessException)
            {
                if (membershipUser != null)
                    RegisterLogInAttemptFail(applicationUser.Id, membershipUser);

                return ValidateUserResponse.Invalid;
            }
            catch (FatalException)
            {
                _loggerService.CreateLog(LogType.Info, LogOperationType.LogInFail, LogCommunicationType.VisaNet, string.Format(LogStrings.ServiceApplicationUser_LogInFail_UserNotExist, email, ""));
                return ValidateUserResponse.Invalid;
            }
        }

        public bool ConfirmUser(string email, string token)
        {
            MembershipUser membershipUser = null;
            try
            {
                var membershipIdentifier =
                    Repository.All(u => u.Email == email).Select(u => u.MembershipIdentifier).FirstOrDefault();

                if (membershipIdentifier == Guid.Empty)
                    throw new FatalException(CodeExceptions.APPLICATION_USER_MISSING_MEMBERSHIP_IDENTIFIER);

                membershipUser = _repositoryMembershipUser.All(u => u.Id == membershipIdentifier).FirstOrDefault();

                if (membershipUser == null)
                    throw new FatalException(CodeExceptions.USER_NOT_EXIST);

                if (membershipUser.Blocked)
                    throw new FatalException(CodeExceptions.APPLICATION_USER_DISABLED);

                if (membershipUser.ConfirmationToken != token)
                    throw new BusinessException(CodeExceptions.APPLICATION_USER_INVALID_CONFIRMATION_TOKEN);


                var decryptedDatabaseToken = RijndaelSecurity.Encrypt(membershipUser.ConfirmationToken).Split('|');
                var decryptedUserToken = RijndaelSecurity.Encrypt(token).Split('|');

                if ((email == decryptedDatabaseToken[0]) &&
                    (decryptedDatabaseToken[0] == decryptedUserToken[0]) &&
                    (decryptedDatabaseToken[1] == decryptedUserToken[1]))
                    throw new BusinessException(CodeExceptions.APPLICATION_USER_INVALID_CONFIRMATION_TOKEN);


                membershipUser.Active = true;
                membershipUser.ConfirmationToken = string.Empty;

                _repositoryMembershipUser.Edit(membershipUser);
                _repositoryMembershipUser.Save();

                return true;
            }
            catch (BusinessException exception)
            {
                NLogLogger.LogEvent(exception);
                return false;
            }
            catch (FatalException exception)
            {
                NLogLogger.LogEvent(exception);
                return false;
            }
        }

        public override ApplicationUserDto Create(ApplicationUserDto entity, bool returnEntity = false)
        {
            throw new NotImplementedException();
        }

        private void RegisterLogInAttemptSuccess(MembershipUser membershipUser)
        {
            membershipUser.LastAttemptToLogIn = null;
            membershipUser.FailLogInCount = 0;

            _repositoryMembershipUser.Edit(membershipUser);
            _repositoryMembershipUser.Save();
        }

        private void RegisterLogInAttemptFail(Guid applicationUserId, MembershipUser membershipUser)
        {
            membershipUser.LastAttemptToLogIn = DateTime.Now;
            membershipUser.FailLogInCount++;

            _loggerService.CreateLog(LogType.Info, LogOperationType.LogInFail, LogCommunicationType.VisaNet, applicationUserId,
                string.Format(LogStrings.ServiceApplicationUser_LogInFail, membershipUser.FailLogInCount),
                string.Format(LogStrings.ServiceApplicationUser_LogInFail, membershipUser.FailLogInCount));


            //TODO: cambiar por variable en página de configuración
            if (membershipUser.FailLogInCount >= 5 && membershipUser.Active)
            {
                membershipUser.Active = false;

                _loggerService.CreateLog(LogType.Info, LogOperationType.LogInFail, LogCommunicationType.VisaNet,
                    applicationUserId, string.Format(LogStrings.ServiceApplicationUser_UserLocked, 5));
            }

            _repositoryMembershipUser.Edit(membershipUser);
            _repositoryMembershipUser.Save();


        }

        public ApplicationUserDto GetUserByUserName(string username)
        {
            var user = AllNoTracking(null, u => u.Email.Equals(username), u => u.ServicesAssociated, u => u.Cards).FirstOrDefault();

            if (user == null)
                throw new BusinessException(CodeExceptions.SYSTEM_USER_NOT_EXIST);

            //RO: Pongo lógica que estaba en el ProfileController --Inicio
            user.RecieveNewsletter = _serviceSubscriber.ExistsEmail(user.Email);
            //RO: Pongo lógica que estaba en el ProfileController --Fin

            return user;
        }

        public ApplicationUserDto SearchUser(Guid id, string identityNumber)
        {
            var user = AllNoTracking(null, u => (id == default(Guid) || u.Id.Equals(id)) &&
                                                (String.IsNullOrEmpty(identityNumber) || u.IdentityNumber.Equals(identityNumber))).FirstOrDefault();

            if (user == null)
                throw new BusinessException(CodeExceptions.SYSTEM_USER_NOT_EXIST);

            return user;
        }

        public string ResetPasswordForUser(string email)
        {
            var userData = Repository.All(u => u.Email == email).Select(u => new { u.MembershipIdentifier, u.Id }).FirstOrDefault();

            if (userData == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            var membershipUser =
                _repositoryMembershipUser.All(u => u.Id == userData.MembershipIdentifier).FirstOrDefault();

            if (membershipUser == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            if (membershipUser.Blocked)
                throw new FatalException(CodeExceptions.APPLICATION_USER_DISABLED);

            var tokenFormat = string.Format("{0}|{1}", email, DateTime.Now.AddMinutes(30).Ticks);
            var token = RijndaelSecurity.Encrypt(tokenFormat);

            membershipUser.LastResetPassword = DateTime.Now;
            membershipUser.ResetPasswordToken = UrlHelper.Base64ForUrlEncode(token);

            _repositoryMembershipUser.Edit(membershipUser);
            Repository.Save();

            _loggerService.CreateLog(LogType.Info, LogOperationType.UserResetPassword, LogCommunicationType.VisaNet, userData.Id,
                string.Format(LogStrings.ServiceApplicationUser_CallCenterResetPassword));

            return membershipUser.ResetPasswordToken;
        }

        public void InactivateUser(Guid id)
        {
            var efEntity = Repository.All(u => u.Id == id, u => u.ServicesAssociated).FirstOrDefault();

            if (efEntity == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            var membershipUser =
                _repositoryMembershipUser.All(u => u.Id == efEntity.MembershipIdentifier).FirstOrDefault();

            if (membershipUser == null)
                throw new FatalException(CodeExceptions.APPLICATION_USER_NOT_EXIST);

            if (membershipUser.Blocked)
                throw new FatalException(CodeExceptions.APPLICATION_USER_DISABLED);

            membershipUser.Active = false;
            _repositoryMembershipUser.Edit(membershipUser);

            foreach (var serviceAssociatedId in efEntity.ServicesAssociated.Where(s => s.AutomaticPaymentId != null).Select(s => s.Id))
            {
                _repositoryServiceAsociated.ChangeState(new object[] { serviceAssociatedId, false });
            }

            Repository.Save();
        }

        public IEnumerable<ApplicationUserDto> GetApplicationUserAutoComplete(string contains)
        {
            if (string.IsNullOrEmpty(contains))
                return new List<ApplicationUserDto>();

            contains = contains.ToLower();
            var data = Repository.AllNoTracking(a => a.Email.ToLower().Contains(contains)).Select(n => new ApplicationUserDto
            {
                Id = n.Id,
                Email = n.Email,
            }).ToList();

            return data;
        }

        public void ActiveUserSistarbanc(Guid userId)
        {
            Repository.ContextTrackChanges = true;
            var efEntity = Repository.All(u => u.Id == userId, u => u.SistarbancUser).FirstOrDefault();
            if (efEntity != null && efEntity.SistarbancUser != null)
            {
                efEntity.SistarbancUser.Active = true;
            }

            Repository.Edit(efEntity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public void ActiveUserSistarbancBrou(Guid userId)
        {
            Repository.ContextTrackChanges = true;
            var efEntity = Repository.All(u => u.Id == userId, u => u.SistarbancBrouUser).FirstOrDefault();
            if (efEntity != null && efEntity.SistarbancBrouUser != null)
            {
                efEntity.SistarbancBrouUser.Active = true;
            }

            Repository.Edit(efEntity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public CardDto AddCard(CardDto cardDto, Guid userId)
        {
            var entity = Repository.GetById(userId, s => s.Cards);
            var card = new Card()
            {
                CybersourceTransactionId = cardDto.CybersourceTransactionId,
                DueDate = cardDto.DueDate,
                MaskedNumber = cardDto.MaskedNumber,
                PaymentToken = cardDto.PaymentToken,
                Active = true,
                Name = cardDto.Name,
                ExternalId = cardDto.ExternalId,
                Description = cardDto.Description
            };
            _repository.AddCardToUser(userId, card);

            _loggerService.CreateLog(LogType.Info, LogOperationType.NewCardAdded, LogCommunicationType.VisaNet, userId,
                string.Format(LogStrings.ServiceApplicationUser_AddNewCard,
                    entity.Email, card.MaskedNumber, card.DueDate.ToString("MM/yyyy")));

            cardDto.Id = card.Id;

            return cardDto;
        }

        public CybersourceCreateCardDto AddCard(IDictionary<string, string> cybersourceData)
        {
            var result = new CybersourceCreateCardDto();
            try
            {
                var data = _serviceAnalyzeCsCall.ProcessCybersourceOperation(cybersourceData);
                result.TokenizationData = data.TokenizationData;

                if (data.TokenizationData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    var userId = data.PaymentDto.RegisteredUserId.Value;
                    var cardDto = data.PaymentDto.Card;
                    var newCardDto = this.AddCard(cardDto, userId);
                    result.NewCardDto = newCardDto;
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceApplicationUser - AddCard Exception");
                NLogLogger.LogEvent(exception);
                throw;
            }
            return result;
        }

        public IEnumerable<ApplicationUserDto> GetDashboardData(ReportsDashboardFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (filters.From != default(DateTime))
            {
                query = query.Where(u => u.CreationDate >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(u => u.CreationDate < filters.To);
            }

            return query.Select(i => new ApplicationUserDto
            {
                Id = i.Id,
                Name = i.Name,
                Surname = i.Surname,
                Email = i.Email,
            }).ToList();
        }

        public int GetDashboardDataCount(ReportsDashboardFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (filters.From != default(DateTime))
            {
                query = query.Where(u => u.CreationDate >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(u => u.CreationDate < filters.To);
            }

            return query.Count();
        }

        public List<WebServiceApplicationClientDto> AssosiatedServiceClientUpdate(WebServiceClientInputDto dto)
        {
            var services = _repositoryServiceAsociated.AssosiatedServiceClientUpdate(new WebServiceClientInput()
            {
                CodBranch = dto.CodBranch,
                CodCommerce = dto.CodCommerce,
                FechaDesde = dto.FechaDesde,
                IdApp = dto.IdApp,
                RefCliente1 = dto.RefCliente1,
                RefCliente2 = dto.RefCliente2,
                RefCliente3 = dto.RefCliente3,
                RefCliente4 = dto.RefCliente4,
                RefCliente5 = dto.RefCliente5,
                RefCliente6 = dto.RefCliente6,
            });
            var result = new List<WebServiceApplicationClientDto>();
            foreach (var client in services)
            {
                var user = Repository.GetById(client.UserId);
                client.Apellido = user.Surname;
                client.Documento = user.IdentityNumber;
                client.Email = user.Email;
                client.Nombre = user.Name;
                client.Telefono = user.PhoneNumber;
                result.Add(new WebServiceApplicationClientDto()
                {
                    Apellido = client.Apellido,
                    Nombre = client.Nombre,
                    Documento = client.Documento,
                    Email = client.Email,
                    RefCliente1 = client.RefCliente1,
                    RefCliente2 = client.RefCliente2,
                    RefCliente3 = client.RefCliente3,
                    RefCliente4 = client.RefCliente4,
                    RefCliente5 = client.RefCliente5,
                    RefCliente6 = client.RefCliente6,
                    Estado = client.Estado,
                    FchModificacion = client.FchModificacion,
                    Telefono = client.Telefono,
                    UserId = client.UserId
                });
            }

            return result;
        }

        public ICollection<ApplicationUserDto> GetDataForReportsUser(ReportsUserFilterDto filterDto)
        {
            var query = GetDataForTableReportsUser(filterDto);

            //ordeno, skip y take
            if (filterDto.OrderBy.Equals("0"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDate) : query.OrderByDescending(x => x.CreationDate);
            }
            else if (filterDto.OrderBy.Equals("1"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Email) : query.OrderByDescending(x => x.Email);
            }
            else if (filterDto.OrderBy.Equals("5"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.MembershipIdentifierObj.Active) : query.OrderByDescending(x => x.MembershipIdentifierObj.Active);
            }
            else if (filterDto.OrderBy.Equals("11"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.MembershipIdentifierObj.Blocked) : query.OrderByDescending(x => x.MembershipIdentifierObj.Blocked);
            }
            else
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDate) : query.OrderByDescending(x => x.CreationDate);
            }

            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            return query.ToList().Select(Converter).ToList();
        }

        public int GetDataForReportsUserCount(ReportsUserFilterDto filterDto)
        {
            var query = GetDataForTableReportsUser(filterDto);
            return query.Count();
        }

        private IQueryable<ApplicationUser> GetDataForTableReportsUser(ReportsUserFilterDto filterDto)
        {
            var query = Repository.AllNoTracking(null, x => x.Cards, x => x.Payments, x => x.ServicesAssociated, x => x.MembershipIdentifierObj);

            filterDto.DateTo = filterDto.DateTo.AddDays(1);

            query = query.Where(p => p.CreationDate.CompareTo(filterDto.DateFrom) >= 0);
            query = query.Where(p => p.CreationDate.CompareTo(filterDto.DateTo) < 0);

            if (!string.IsNullOrEmpty(filterDto.Email))
                query = query.Where(x => x.Email.Contains(filterDto.Email));

            if (filterDto.ActiveOrInactive == ActiveOrInactiveEnumDto.Active)
            {
                query = query.Where(x => x.MembershipIdentifierObj.Active);
            }
            if (filterDto.ActiveOrInactive == ActiveOrInactiveEnumDto.Inactive)
            {
                query = query.Where(x => !x.MembershipIdentifierObj.Active);
            }
            if (filterDto.ActiveOrInactive == ActiveOrInactiveEnumDto.Blocked)
            {
                query = query.Where(x => x.MembershipIdentifierObj.Blocked);
            }

            return query;
        }

        public bool ChangeBlockStatusUser(Guid userid)
        {
            var context = _repository.GetTransactionContext();
            if (_serviceSystemUser.ValidateUserAction(context.UserName, Actions.ReportsUsersEdit))
            {
                //SOLO DESDE EL BO SE PUEDE REALIZAR ESTA ACCION
                Repository.ContextTrackChanges = true;
                var user = _repository.GetById(userid, x => x.MembershipIdentifierObj);
                user.MembershipIdentifierObj.Blocked = !user.MembershipIdentifierObj.Blocked;
                Repository.Edit(user);
                Repository.Save();
                Repository.ContextTrackChanges = false;

                try
                {
                    var msg = string.Format("El usuario registrado fue bloqueado por el usuario {0}", context.UserName);
                    _loggerService.CreateLog(LogType.Info, LogOperationType.UserBlocked, LogCommunicationType.VisaNet, userid, msg, msg);
                }
                catch (Exception e)
                {

                }
                return true;
            }

            return false;
        }

        public override ApplicationUserDto GetById(Guid id, params Expression<Func<ApplicationUser, object>>[] properties)
        {
            var user = base.GetById(id, properties);
            if (user == null)
                throw new BusinessException(CodeExceptions.SYSTEM_USER_NOT_EXIST);

            //RO: Pongo lógica que estaba en el ProfileController --Inicio
            user.RecieveNewsletter = _serviceSubscriber.ExistsEmail(user.Email);
            //RO: Pongo lógica que estaba en el ProfileController --Fin
            return user;
        }

        private string GenerateRandomPassword()
        {
            //Devuelve un password de 8 caracteres con al menos una mayuscula, una minuscula y un numero
            var random = new Random();

            var randomPassword = System.Web.Security.Membership.GeneratePassword(5, 0).ToLower();

            var numUpperLet = random.Next(0, 26);
            var upperLetter = ((char)('a' + numUpperLet)).ToString().ToUpper();

            var numLowLet = random.Next(0, 26);
            var lowLetter = ((char)('a' + numLowLet)).ToString();

            var num = random.Next(0, 10);

            var password = string.Concat(upperLetter, lowLetter, randomPassword, num);
            return password;
        }

        public long GetNextCyberSourceIdentifier()
        {
            return ((IRepositoryApplicationUser)Repository).GetNextCyberSourceIdentifier();
        }

    }
}