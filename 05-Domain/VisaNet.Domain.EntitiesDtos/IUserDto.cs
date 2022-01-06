using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public interface IUserDto
    {
        Guid Id { get; set; }
        string Email { get; set; }
        string Name { get; set; }
        string Surname { get; set; }
        string IdentityNumber { get; set; }
        string MobileNumber { get; set; }
        string PhoneNumber { get; set; }
        string Address { get; set; }
        DateTime CreationDate { get; set; }

        bool IsRegisteredUser { get; }

        string Password { get; set; }
    }
}
