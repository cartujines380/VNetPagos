using System.Collections.Generic;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryRole : IRepository<Role>
    {
        IList<Action> Actions { get; }
        IList<Action> ActionsNoTracking { get; }
        IList<Functionality> Functionalities { get; }
        IList<FunctionalityGroup> FunctionalityGroups { get; }
        IList<FunctionalityGroup> FunctionalityGroupsNoTracking { get; }
    }
}
