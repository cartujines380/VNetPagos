using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceMailgun
    {
        string CreateRoute(string email);

        void DeleteRoute(string routeId);

        MailgunResponse SendHtml(MailMessage mail);

        MailgunStatus GetEmailStatus(string mailgunId, string emailTo);
    }
}
