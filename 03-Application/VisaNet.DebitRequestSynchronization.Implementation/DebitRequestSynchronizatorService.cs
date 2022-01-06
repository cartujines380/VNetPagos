using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using VisaNet.Application.Implementations;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security;
using VisaNet.Domain.Debit.Base;
using VisaNet.Domain.Debit.Entities;
using VisaNet.Domain.Debit.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.DebitRequestSynchronization.Implementation
{
    public class DebitRequestSynchronizatorService : DebitBaseService<IMerchantDebitRequestRepository, MerchantDebitRequest>, IDebitRequestSynchronizatorService
    {
        private readonly IMerchantDebitRequestRepository merchantDebitRequestRepository;
        private readonly IRepositoryDebitRequest repositoryDebitRequest;
        private readonly IServiceDebitRequest _serviceDebitRequest;

        public DebitRequestSynchronizatorService(
            IMerchantDebitRequestRepository merchantDebitRequestRepository,
            IRepositoryDebitRequest repositoryDebitRequest, IServiceDebitRequest serviceDebitRequest) : base(merchantDebitRequestRepository)
        {
            this.merchantDebitRequestRepository = merchantDebitRequestRepository;
            this.repositoryDebitRequest = repositoryDebitRequest;
            _serviceDebitRequest = serviceDebitRequest;
        }

        public void StartSynchronization()
        {
            var debitRequestFromVNPList = this.repositoryDebitRequest
                .All()
                .AsNoTracking()
                .Where(x => x.State == DebitRequestState.Pending)
                .Where(x => x.DebitRequestEventId.HasValue)
                .ToList();

            var debitRequestEventIdFromVNPList = debitRequestFromVNPList
                .Select(x => x.DebitRequestEventId.Value)
                .Distinct()
                .ToList();

            var debitRequestFromDebitList = this.merchantDebitRequestRepository
                .GetAllMerchantDebitRequestForSynchronization(debitRequestEventIdFromVNPList);

            var message = string.Format("Debit requests to synchronize:{0}", debitRequestFromVNPList.Count());
            NLogLogger.LogEvent(NLogType.Info, message);

            var debitRequestToSynchronizeList = new List<DebitRequest>();

            foreach (var debitRequestFromVNP in debitRequestFromVNPList)
            {
                var debitRequestFromDebitDb = debitRequestFromDebitList
                    .MerchantDebitRequestEventList
                    .FirstOrDefault(x => x.Id == debitRequestFromVNP.DebitRequestEventId);
                    

                if (debitRequestFromDebitDb == null)
                {
                    continue;
                }

                var eventStatusEnumFromVNP = debitRequestFromDebitDb.EventStatusEnum;
                var eventStatusEnumFromDebit = debitRequestFromDebitDb.EventStatusEnum;
                var validState = eventStatusEnumFromDebit == EventStatusEnum.AC || eventStatusEnumFromDebit == EventStatusEnum.RC;

                if (!validState)
                {
                    message = string.Format(
                        "Estado no contemplado DebitRequestId:{0}, DebitRequestEventId:{1}, DebitEventStatus:{2}", 
                        debitRequestFromVNP.Id,
                        debitRequestFromVNP.DebitRequestEventId,
                        debitRequestFromDebitDb.EventStatus);
                    NLogLogger.LogEvent(NLogType.Info, message);
                    continue;
                }

                debitRequestFromVNP.State = eventStatusEnumFromDebit == EventStatusEnum.AC ?
                    DebitRequestState.Accepted :
                    eventStatusEnumFromDebit == EventStatusEnum.RC ?
                        DebitRequestState.Rejected :
                        debitRequestFromVNP.State;

                debitRequestToSynchronizeList.Add(debitRequestFromVNP);
            }

            if (debitRequestToSynchronizeList.Any())
            {
                SaveChanges(debitRequestToSynchronizeList);
                UpdateRequestParent(debitRequestToSynchronizeList.Where(x => x.Type == DebitRequestType.Low).ToList());
                NotifyClients(debitRequestToSynchronizeList);
            }

            message = string.Format("Debit requests synchronized:{0}", debitRequestToSynchronizeList.Count());
            NLogLogger.LogEvent(NLogType.Info, message);
        }

        private void SaveChanges(List<DebitRequest> debitRequestList)
        {
            var transactionContext = new DebitRequestSynchronizationProcess();

            using (var tx = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 30, 0)))
            {
                DbContext context;

                using (context = new AppContext(transactionContext))
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;
                    
                    foreach (var entity in debitRequestList)
                    {
                        entity.LastModificationDate = DateTime.Now;
                        entity.LastModificationUser = transactionContext.UserName;
                        context.Entry(entity).State = EntityState.Modified;
                    }

                    context.SaveChanges();
                }//context

                tx.Complete();
            }//tx
        }

        private void UpdateRequestParent(List<DebitRequest> debitRequestList)
        {

            var debitRequestToUpdateList = new List<DebitRequest>();
            debitRequestList.ForEach(x => 
            {
                if(x.State == DebitRequestState.Accepted && x.AssociatedDebitRequestId.HasValue)
                {
                    var parent = repositoryDebitRequest.GetById(x.AssociatedDebitRequestId.Value);
                    parent.State = DebitRequestState.AcceptedCancellation;

                    debitRequestToUpdateList.Add(parent);
                }
                else if(x.State == DebitRequestState.Rejected)
                {
                    var parent = repositoryDebitRequest.GetById(x.AssociatedDebitRequestId.Value);
                    parent.State = DebitRequestState.RejectedCancellation;

                    debitRequestToUpdateList.Add(parent);
                }
            });

            SaveChanges(debitRequestToUpdateList);
        }

        private void NotifyClients(List<DebitRequest> debitRequestList)
        {
            //envio mails de confirmacion
            foreach (var debitRequest in debitRequestList)
            {
                _serviceDebitRequest.SendDebitSuscriptionNotification(debitRequest.Id);
            } 
        }
        
    }
}