using System.Threading.Tasks;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceSmsNotification : IService<SmsMessage, SmsMessageDto>
    {
        void SendVonAccessSms(SmsMessageVonAccessDto dto);
    }
}
