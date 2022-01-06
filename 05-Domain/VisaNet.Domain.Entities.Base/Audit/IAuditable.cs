using System;

namespace VisaNet.Domain.Entities.Base.Audit
{
    public interface IAuditable
    {
        string CreationUser { get; set; }
        string LastModificationUser { get; set; }

        DateTime CreationDate { get; set; }
        DateTime LastModificationDate { get; set; }
    }
}
