using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces
{
    public interface IStateFactory
    {
        RunState GetState();
    }
}
