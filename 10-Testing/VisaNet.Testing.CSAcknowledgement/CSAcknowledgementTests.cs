using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Ninject;
using VisaNet.Common.DependencyInjection;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Testing.CSAcknowledgement
{
    public abstract class CSAcknowledgementTests
    {
        protected static IRepositoryCyberSourceAcknowledgement RepositoryCyberSourceAcknowledgement;
        protected static IRepositoryCyberSourceVoid RepositoryCyberSourceVoid;
        private static IRepositoryApplicationUser _repositoryApplicationUser;
        private static IRepositoryPayment _repositoryPayment;
        private static IRepositoryBill _repositoryBill;
        private static IRepositoryService _repositoryService;

        protected static Guid ApplicationUserId;
        protected static Guid CardId;
        protected static string CardPaymentToken;

        //Test 1
        protected static Guid CsAck1Id;
        protected static Guid Service1Id;

        //Test 2
        protected static Guid CsAck2Id;
        protected static Guid Service2Id;
        private static Guid _payment2Id;
        protected static string Transaction2Id;
        protected static string ReqTransaction2Id;

        //Test 3
        protected static Guid CsAck3Id;
        protected static Guid Service3Id;
        private static Guid _payment3Id;
        protected static string Transaction3Id;
        protected static string ReqTransaction3Id;
        private static Payment _payment3;
        private static Guid? _bill3Id;

        protected CSAcknowledgementTests()
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            RepositoryCyberSourceAcknowledgement = NinjectRegister.Get<IRepositoryCyberSourceAcknowledgement>();
            RepositoryCyberSourceVoid = NinjectRegister.Get<IRepositoryCyberSourceVoid>();
            _repositoryApplicationUser = NinjectRegister.Get<IRepositoryApplicationUser>();
            _repositoryPayment = NinjectRegister.Get<IRepositoryPayment>();
            _repositoryBill = NinjectRegister.Get<IRepositoryBill>();
            _repositoryService = NinjectRegister.Get<IRepositoryService>();
        }

        protected static void LoadDataForTests()
        {
            CsAck1Id = Guid.NewGuid();
            CsAck2Id = Guid.NewGuid();
            CsAck3Id = Guid.NewGuid();
            _bill3Id = null;
            Service1Id = _repositoryService.All(x => x.Name.ToUpper() == "ASUR").FirstOrDefault().Id;

            LoadPaymentsData();
            DeleteLatePayment();
        }

        protected static void CleanDataForTests()
        {
            var csAcknowledgementsIds = new List<Guid> { CsAck1Id, CsAck2Id, CsAck3Id };

            var csVoidsIds =
                RepositoryCyberSourceVoid.All(x =>
                    csAcknowledgementsIds.Contains(x.CyberSourceAcknowledgementId)).Select(x => x.Id).ToList();

            using (var context = new AppContext())
            {
                //Se vuelven a poner los Payments en estado Done
                var payment2 = context.Set<Payment>().Find(_payment2Id);
                payment2.PaymentStatus = PaymentStatus.Done;
                context.Entry(payment2).State = EntityState.Modified;

                var payment3 = context.Set<Payment>().Find(_payment3Id);
                payment3.PaymentStatus = PaymentStatus.Done;
                context.Entry(payment3).State = EntityState.Modified;

                //Se vuelve a referenciar al Payment desde la Bill
                if (_bill3Id != null)
                {
                    var bill = context.Set<Bill>().Find(_bill3Id);
                    bill.PaymentId = _payment3Id;
                    context.Entry(bill).State = EntityState.Modified;
                }

                //Se eliminan los CyberSourceAcknowledgements de los tests
                foreach (var id in csAcknowledgementsIds)
                {
                    var csAcknowledgement = context.Set<CyberSourceAcknowledgement>().Find(id);
                    if (csAcknowledgement != null)
                    {
                        context.Entry(csAcknowledgement).State = EntityState.Deleted;
                    }
                }

                //Se eliminan los CyberSourceVoids de los tests
                foreach (var id in csVoidsIds)
                {
                    var csVoid = context.Set<CyberSourceVoid>().Find(id);
                    if (csVoid != null)
                    {
                        context.Entry(csVoid).State = EntityState.Deleted;
                    }
                }

                context.SaveChanges();
            }
        }

        protected static void InsertLatePayment()
        {
            using (var context = new AppContext())
            {
                var paymentsTable = context.Set<Payment>();
                paymentsTable.Add(_payment3);
                context.SaveChanges();
            }
        }

        protected static bool VerifyUserWithPaymentsExists()
        {
            //Pre-condición: que exista el usuario indicado, y que haya realizado pagos
            var emailForTests = ConfigurationManager.AppSettings["EmailForTests"];
            var appUser = _repositoryApplicationUser.All(null, x => x.Cards).FirstOrDefault(x => x.Email == emailForTests);
            if (appUser != null)
            {
                ApplicationUserId = appUser.Id;

                if (appUser.Cards != null && appUser.Cards.Any())
                {
                    foreach (var card in appUser.Cards)
                    {
                        var hasPayments =
                            _repositoryPayment.All(x =>
                                x.RegisteredUserId == ApplicationUserId &&
                                x.CardId == card.Id &&
                                x.PaymentStatus == PaymentStatus.Done).Count() >= 2;

                        if (hasPayments)
                        {
                            CardId = card.Id;
                            CardPaymentToken = card.PaymentToken;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static void LoadPaymentsData()
        {
            var payments = _repositoryPayment.All(x =>
                x.RegisteredUserId == ApplicationUserId &&
                x.CardId == CardId &&
                x.PaymentStatus == PaymentStatus.Done).OrderByDescending(x => x.Date).Take(2).ToList();

            _payment2Id = payments.FirstOrDefault().Id;
            Transaction2Id = payments.FirstOrDefault().TransactionNumber;
            ReqTransaction2Id = payments.FirstOrDefault().CyberSourceData.ReqTransactionUuid;
            Service2Id = payments.FirstOrDefault().ServiceId;

            _payment3Id = payments.LastOrDefault().Id;
            Transaction3Id = payments.LastOrDefault().TransactionNumber;
            ReqTransaction3Id = payments.LastOrDefault().CyberSourceData.ReqTransactionUuid;
            _payment3 = payments.LastOrDefault();
            Service3Id = payments.LastOrDefault().ServiceId;
        }

        private static void DeleteLatePayment()
        {
            var bill =
                _repositoryBill.All(x => x.PaymentId != null && x.PaymentId == _payment3Id).FirstOrDefault();

            using (var context = new AppContext())
            {
                //Se elimina el Payment del test 3 porque se inserta después
                var payment = context.Set<Payment>().Find(_payment3Id);
                context.Entry(payment).State = EntityState.Deleted;

                //Se quita la referencia de la Bill para poder eliminar el pago
                if (bill != null)
                {
                    _bill3Id = bill.Id;
                    var entityBill = context.Set<Bill>().Find(_bill3Id);
                    entityBill.PaymentId = null;
                    context.Entry(entityBill).State = EntityState.Modified;
                }

                context.SaveChanges();
            }
        }

    }

}