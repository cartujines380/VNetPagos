namespace VisaNet.Domain.Entities.Enums
{
    /// <summary>
    /// Especifica la version a utilizar en las Url Alta y Baja utilizados por los comercios que se integraron con VisaNet
    /// </summary>
    public enum UrlIntegrationVersionEnum
    {
        NotApply = 0,
        FirstVersion = 1, //02.02.00
        SecondVersion = 2, //02.02.11
        ThirdVersion = 3, //02.03.02
        FourthVersion = 4, //03.00.00 (VisaNetOn)
        FifthVersion = 5, //03.00.05 (Banco como código + Programa tarjeta)

        //Si se agregan mas versiones se deben considerar en ServiceExternalNotification, ServicePostSignatureFactory
    }
}