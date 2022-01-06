using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Domain.Entities.ComplexTypes
{
    [ComplexType]
    public class VerifyByVisaData
    {
        //payer_authentication_eci
        public string PayerAuthenticationEci { get; set; }
        //payer_authentication_xid
        public string PayerAuthenticationXid { get; set; }
        //payer_authentication_cavv
        public string PayerAuthenticationCavv { get; set; }
        //payer_authentication_proof_xml
        public string PayerAuthenticationProofXml { get; set; }
    }
}
