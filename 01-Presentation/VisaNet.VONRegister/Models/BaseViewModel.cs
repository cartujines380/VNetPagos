namespace VisaNet.VONRegister.Models
{
    public abstract class BaseViewModel<T> where T : class
    {
        public abstract T ToDto();
    }
}