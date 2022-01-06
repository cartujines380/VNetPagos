namespace VisaNet.LIF.WebApi.Models
{
    public class OutModel
    {
        /// <summary>
        /// Objeto con la respuesta del método invocado
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Código de la respuesta
        /// </summary>
        public int Code { get; set; }
    }
}