using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryServiceAsociated : IRepository<ServiceAssociated>
    {
        void ChangeState(Object[] data);
        void ChangeState(IList<ServiceAssociated> servicesAssociated, bool active, bool? enable);
        IQueryable<WebServiceApplicationClient> AssosiatedServiceClientUpdate(WebServiceClientInput dto);
    }
}
