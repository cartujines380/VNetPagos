using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryBinGroup : BaseRepository<BinGroup>, IRepositoryBinGroup
    {
        public RepositoryBinGroup(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        private const string sqlQuery = " SELECT bg.[Id], bg.[CreationDate], bg.[LastModificationDate],  bg.[Name], " +
                                        "bin.[Id] 'Bin_Id', bin.[Name] 'Bin_Name', bin.[Value] 'Bin_Value', " +
                                        "bin.[Country] 'Bin_Country', bank.[Id] 'Bin_BankDto_Id', bank.[Name] 'Bin_BankDto_Name', " +
                                        "bin.[CardType] 'Bin_CardType', bin.[CreationDate] 'Bin_CreationDate', " +
                                        "bin.[LastModificationDate] 'Bin_LastModificationDate' " +
                                        "FROM [dbo].[BinGroups] bg " +
                                        "LEFT OUTER JOIN [dbo].[BinBinGroups] bbg on bg.Id = bbg.[BinGroup_Id] " +
                                        "LEFT OUTER JOIN [dbo].[Bin] bin on bin.Id = bbg.[Bin_Id] " +
                                        "LEFT OUTER JOIN [dbo].[Bank] bank on bin.[BankId] = bank.Id " +
                                        "WHERE bg.ID = '{0}'";

        public new BinGroup GetById(Guid id, params Expression<Func<BinGroup, object>>[] includeProperties)
        {
            var query = string.Format(sqlQuery, id);

            var rawData = _db.SqlQuery<BinGroupDataReader>(query);

            var retObject = new BinGroup();

            if (!rawData.Any())
            {
                return retObject;
            }

            var data0 = rawData.ElementAt(0);
            retObject.Id = data0.Id;
            retObject.Name = data0.Name;
            retObject.CreationDate = data0.CreationDate;
            retObject.LastModificationDate = data0.LastModificationDate;
            retObject.Bins = new List<Bin>();

            //busca los servicos asociados
            string sqlQry = "SELECT DISTINCT s.* FROM services s " +
                            "INNER JOIN[dbo].[BinGroupServices] bgs ON s.id = bgs.[Service_Id] " +
                            "INNER JOIN[dbo].[BinGroups] bg ON bg.id = bgs.BinGroup_Id " +
                            "WHERE bg.id = '{0}'";

            query = string.Format(sqlQry, id);

            var servicesData = _db.SqlQuery<Service>(query).ToList();

            retObject.Services = servicesData;

            //El grupo de bines no tiene ningun bin asociado
            if (!rawData.First().Bin_Id.HasValue)
            {
                return retObject;
            }

            foreach (var reader in rawData)
            {
                retObject.Bins.Add(new Bin
                {
                    Id = reader.Bin_Id.Value,
                    Name = reader.Bin_Name,
                    Value = reader.Bin_Value.Value,
                    Country = reader.Bin_Country,
                    Bank = reader.Bin_BankDto_Id.HasValue ? new Bank { Id = reader.Bin_BankDto_Id.Value, Name = reader.Bin_BankDto_Name } : null,
                    CardType = (CardType)reader.Bin_CardType
                });
            }

            return retObject;
        }

        public BinGroup GetByIdTracking(Guid id, params Expression<Func<BinGroup, object>>[] includeProperties)
        {
            var query = string.Format(sqlQuery, id);

            var rawData = _db.SqlQuery<BinGroupDataReader>(query);

            var retObject = new BinGroup();

            if (!rawData.Any())
            {
                return retObject;
            }

            var data0 = rawData.ElementAt(0);
            retObject.Id = data0.Id;
            retObject.Name = data0.Name;
            retObject.CreationDate = data0.CreationDate;
            retObject.LastModificationDate = data0.LastModificationDate;
            retObject.Bins = new List<Bin>();

            //busca los servicos asociados
            string sqlQry = "SELECT DISTINCT s.* FROM services s " +
                            "INNER JOIN[dbo].[BinGroupServices] bgs ON s.id = bgs.[Service_Id] " +
                            "INNER JOIN[dbo].[BinGroups] bg ON bg.id = bgs.BinGroup_Id " +
                            "WHERE bg.id = '{0}'";

            query = string.Format(sqlQry, id);

            var servicesData = _db.SqlQuery<Service>(query).ToList();

            retObject.Services = servicesData;

            //El grupo de bines no tiene ningun bin asociado
            if (!rawData.First().Bin_Id.HasValue)
            {
                _context.Set<BinGroup>().Attach(retObject);
                return retObject;
            }

            foreach (var reader in rawData)
            {
                retObject.Bins.Add(new Bin
                {
                    Id = reader.Bin_Id.Value,
                    Name = reader.Bin_Name,
                    Value = reader.Bin_Value.Value,
                    Country = reader.Bin_Country,
                    BankId = reader.Bin_BankDto_Id,
                    CardType = (CardType)reader.Bin_CardType,
                    CreationDate = reader.Bin_CreationDate.Value,
                    LastModificationDate = reader.Bin_LastModificationDate.Value
                });
            }

            _context.Set<BinGroup>().Attach(retObject);

            return retObject;
        }

        private class BinGroupDataReader
        {
            public Guid? Bin_Id { get; set; }
            public string Bin_Name { get; set; }
            public int? Bin_Value { get; set; }
            public string Bin_Country { get; set; }
            public Guid? Bin_BankDto_Id { get; set; }
            public string Bin_BankDto_Name { get; set; }
            public DateTime? Bin_CreationDate { get; set; }
            public DateTime? Bin_LastModificationDate { get; set; }
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime LastModificationDate { get; set; }
            public int? Bin_CardType { get; set; }
        }
    }
}
