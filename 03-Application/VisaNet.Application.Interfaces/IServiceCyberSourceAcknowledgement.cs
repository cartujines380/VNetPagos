using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceCyberSourceAcknowledgement
    {
        void Process(CyberSourceAcknowledgementDto post);
        void VoidPayments();
    }
}