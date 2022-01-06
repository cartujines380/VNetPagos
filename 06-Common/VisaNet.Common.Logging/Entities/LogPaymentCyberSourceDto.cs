using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Common.Logging.Entities
{
    public class LogPaymentCyberSourceDto
    {
        [Key]
        public Guid Id { get; set; }

        public Guid LogId { get; set; }
        public Guid TransactionIdentifier { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public CyberSourceLogDataDto CyberSourceLogData { get; set; }
        public CyberSourceVerifyByVisaDataDto CyberSourceVerifyByVisaData { get; set; }
        
    }

    [ComplexType]
    public class CyberSourceLogDataDto
    {
        public TransactionType TransactionType { get; set; }

        public PaymentPlatform PaymentPlatform { get; set; }

        #region CyberSourceResult
        //decision
        public string Decision { get; set; }
        //reason_code
        public string ReasonCode { get; set; }
        //transaction_id
        public string TransactionId { get; set; }
        //message
        public string Message { get; set; }
        //bill_trans_ref_no 
        public string BillTransRefNo { get; set; }
        //payment_token
        public string PaymentToken { get; set; }
        #endregion

        #region CyberSourceReqData
        //req_card_number
        public string ReqCardNumber { get; set; }
        //req_card_expiry_date
        public string ReqCardExpiryDate { get; set; }
        //req_profile_id
        public string ReqProfileId { get; set; }
        //req_card_type
        public string ReqCardType { get; set; }
        //req_payment_method
        public string ReqPaymentMethod { get; set; }
        //req_transaction_type
        public string ReqTransactionType { get; set; }
        //req_transaction_uuid
        public string ReqTransactionUuid { get; set; }
        //req_currency
        public string ReqCurrency { get; set; }
        //req_reference_number
        public string ReqReferenceNumber { get; set; }
        //req_amount
        public string ReqAmount { get; set; }
        #endregion

        #region CyberSourceAuthorizationData
        //auth_avs_code
        public string AuthAvsCode { get; set; }
        //auth_code
        public string AuthCode { get; set; }
        //auth_amount
        public string AuthAmount { get; set; }
        //auth_time
        public string AuthTime { get; set; }
        //auth_response
        public string AuthResponse { get; set; }
        //auth_trans_ref_no
        public string AuthTransRefNo { get; set; }
        #endregion
        
    }
    
    public class CyberSourceVerifyByVisaDataDto
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
