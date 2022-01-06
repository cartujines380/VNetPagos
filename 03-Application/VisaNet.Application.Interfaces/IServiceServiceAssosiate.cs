using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceServiceAssosiate : IService<ServiceAssociated, ServiceAssociatedDto>
    {
        IEnumerable<ServiceAssociatedDto> GetDataForTable(ServiceFilterAssosiateDto filters);
        /// <summary>
        /// Devuelve la lista de servicios activos que pueden tener un pago programado
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>

        IEnumerable<ServiceAssociatedDto> GetServicesForAutomaticPayment(ServiceFilterAssosiateDto filters);
        /// <summary>
        /// Devuelve la lista de servicios asociados activos con un pago programado configurado para un usuario registrado
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        IEnumerable<ServiceAssociatedDto> GetServicesActiveAutomaticPayment(ServiceFilterAssosiateDto filters);

        /// <summary>
        /// Devuelve la lista de servicios asociados activos con un pago programado configurado o con notificaciones activas
        /// </summary>
        /// <returns></returns>
        IEnumerable<ServiceAssociatedDto> GetServicesActiveAutomaticPaymentOrNotification();

        /// <summary>
        /// Devuelve la lista de servicios asociados activos con un pago programado configurado
        /// </summary>
        /// <returns></returns>
        IEnumerable<ServiceAssociatedDto> GetServicesActiveAutomaticPayment();

        /// <summary>
        /// Devuelve la lista de servicios asociados activos con notificaciones activas y que no tienen pago programado
        /// </summary>
        /// <returns></returns>
        IEnumerable<ServiceAssociatedDto> GetServicesActiveNotification();

        void AddAutomaticPayment(AutomaticPaymentDto dto);
        void ChangeState(Object[] data);
        void DeleteAutomaticPayment(Guid serviceAssosiatedId);
        void UpdateAutomaticPaymentRunsData(Guid processHistoryId, Dictionary<Guid, PaymentResultTypeDto> results);

        bool DeleteService(Guid serviceAssociatedId, bool notify = true);

        Guid IsServiceAssosiatedToUser(Guid userId, Guid serviceId, string ref1, string ref2, string ref3, string ref4,
            string ref5, string ref6);

        ServiceAssociatedDto ServiceAssosiatedToUser(Guid userId, Guid serviceId, string ref1, string ref2, string ref3,
            string ref4, string ref5, string ref6);

        List<ServiceAssociatedDto> GetServicesForBills(Guid userId);

        /// <summary>
        /// Aumento la cantidad de factuas pagas
        /// </summary>
        /// <param name="dto"></param>
        void AutomaticPaymentAddQuotasDone(AutomaticPaymentDto dto);

        IEnumerable<ServiceAssociatedDto> ReportsServicesAssociatedData(ReportsServicesAssociatedFilterDto filters);

        IEnumerable<ServiceAssociatedViewDto> ReportsServicesAssociatedDataFromDbView(ReportsServicesAssociatedFilterDto filters);

        int ReportsServicesAssociatedDataCount(ReportsServicesAssociatedFilterDto filters);

        /// <summary>
        /// Edita solamente la descripción de un servicio asociado.
        /// </summary>
        /// <param name="entity"></param>
        void EditDescription(ServiceAssociatedDto entity);

        /// <summary>
        /// Chequea si hay servicios asosiados para el usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True si hay servicios asociados</returns>
        bool HasAsosiatedService(Guid userId);
        /// <summary>
        /// Chequea si hay pagos programados creados
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True si hay algun servicio asociado con un pago programado creado</returns>
        bool HasAutomaticPaymentCreated(Guid userId);

        /// <summary>
        /// Metodo que devuelve listado de clientes que actualiaron el servicio asociado desed la fecha indicada en el filtro. O 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        IQueryable<WebServiceApplicationClientDto> AssosiatedServiceClientUpdate(WebServiceClientInputDto dto);

        IList<ServiceAssociatedDto> GetServicesDebit(string refCliente, Guid serviceId);

        ServiceAssociatedDto CreateOrUpdateDeleted(ServiceAssociatedDto entityDto, bool? notifyExternal = null);

        ServiceAssociatedDto CreateWithoutExternalNotification(ServiceAssociatedDto entity);

        bool DeleteCardFromService(Guid serviceId, Guid cardId, Guid userId, bool notifyExternalSource = true);
        bool AddCardToService(Guid serviceId, Guid cardId, Guid oldCardId, Guid userId, string operationId);

        IEnumerable<AutomaticPaymentsViewDto> ReportsAutomaticPaymentsDataFromDbView(ReportsAutomaticPaymentsFilterDto filters);

        int ReportsAutomaticPaymentsDataCount(ReportsAutomaticPaymentsFilterDto filters);

        CybersourceCreateServiceAssociatedDto ProccesDataFromCybersource(IDictionary<string, string> csDictionary);
        CybersourceCreateAppAssociationDto ProccesDataFromCybersourceForApps(IDictionary<string, string> csDictionary);
        ServiceAssociatedDto AssociateServiceToUserFromCardCreated(ServiceAssociatedDto dto);

        void LogCybersourceData(PaymentDto payment, CybersourceTransactionsDataDto csTransactionsDataDto, string msg, LogOperationType logOperationType);

        IList<ServiceAssociatedDto> GetDataForFrontList(ServiceFilterAssosiateDto filters);

        ServiceAssociatedDto GetServiceAssociatedDtoFromIdUserExternal(string idUserExternal, string idApp);

    }
}