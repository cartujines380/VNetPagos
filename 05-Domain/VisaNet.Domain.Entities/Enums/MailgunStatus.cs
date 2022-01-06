namespace VisaNet.Domain.Entities.Enums
{
    public enum MailgunStatus
    {
        /// <summary>
        /// El mail no pudo ser enviado a mailgun
        /// </summary>
        FailureReachingMg = 0,

        /// <summary>
        /// Se envio el mail a mailgun y aun no se ha tenido respuesta de si fue enviado al destinatario o no
        /// </summary>
        SuccessReachingMg = 1,
        
        /// <summary>
        /// Mailgun logro enviar el mail al destinatario
        /// </summary>
        Delivered = 2,

        /// <summary>
        /// Mailgun documentation: "not delivering to an address that previously bounced, unsubscribed, or complained"
        /// </summary>
        DroppedHardFail = 3,

        /// <summary>
        /// Mailgun documentation: "indicates that Mailgun tried to deliver the message unsuccessfully for more than 8 hours."
        /// </summary>
        DroppedOld = 4,

        /// <summary>
        /// Email cancelado por el usuario
        /// </summary>
        Canceled = 5,

        /// <summary>
        /// Reenviado
        /// </summary>
        Resend = 6,

        /// <summary>
        /// No conocido
        /// </summary>
        Unknown = 7,

        /// <summary>
        /// No encontrado en Mailgun
        /// </summary>
        NotFound = 8
    }
}