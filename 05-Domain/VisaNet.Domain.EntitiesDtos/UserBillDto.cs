using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Domain.EntitiesDtos
{
    public interface IUserBillDto
    {
        ICollection<BillDto> Bills { get; set; }
    }

    public class AnonymousUserBillDto : IUserBillDto
    {
        public ICollection<BillDto> Bills { get; set; }
        public AnonymousUserDto User { get; set; }
    }

    public class ApplicationUserBillDto : IUserBillDto
    {
        public ICollection<BillDto> Bills { get; set; }
        public ApplicationUserDto User { get; set; }
    }
}
