﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://spf.sistarbanc.com.uy/spfws/services/WsPagosLIF", ConfigurationName="WsPagosLIF.WsPagosLIF")]
    public interface WsPagosLIF {
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(Style=System.ServiceModel.OperationFormatStyle.Rpc, SupportFaults=true, Use=System.ServiceModel.OperationFormatUse.Encoded)]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(Resultado))]
        [return: System.ServiceModel.MessageParameterAttribute(Name="pagarReciboReturn")]
        VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF.ResultadoWsPagosLIF pagarRecibo(string idClienteBanco, string codigoFactura, string tipoServicio, string idOrganismo, string banco, string nroTranBanco, string importe, string moneda, string guardarPerfil, string nroTranSB, string fecha, string firmaPago, string origen);
        
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        [return: System.ServiceModel.MessageParameterAttribute(Name="pagarReciboReturn")]
        System.Threading.Tasks.Task<VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF.ResultadoWsPagosLIF> pagarReciboAsync(string idClienteBanco, string codigoFactura, string tipoServicio, string idOrganismo, string banco, string nroTranBanco, string importe, string moneda, string guardarPerfil, string nroTranSB, string fecha, string firmaPago, string origen);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:WsPagosLIF")]
    public partial class ResultadoWsPagosLIF : Resultado {
    }
    
    /// <remarks/>
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(ResultadoWsPagosLIF))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="urn:WsPagosLIF")]
    public partial class Resultado : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int codigoErrorField;
        
        private string descripcionErrorField;
        
        /// <remarks/>
        public int codigoError {
            get {
                return this.codigoErrorField;
            }
            set {
                this.codigoErrorField = value;
                this.RaisePropertyChanged("codigoError");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.SoapElementAttribute(IsNullable=true)]
        public string descripcionError {
            get {
                return this.descripcionErrorField;
            }
            set {
                this.descripcionErrorField = value;
                this.RaisePropertyChanged("descripcionError");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WsPagosLIFChannel : VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF.WsPagosLIF, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WsPagosLIFClient : System.ServiceModel.ClientBase<VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF.WsPagosLIF>, VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF.WsPagosLIF {
        
        public WsPagosLIFClient() {
        }
        
        public WsPagosLIFClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WsPagosLIFClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WsPagosLIFClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WsPagosLIFClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF.ResultadoWsPagosLIF pagarRecibo(string idClienteBanco, string codigoFactura, string tipoServicio, string idOrganismo, string banco, string nroTranBanco, string importe, string moneda, string guardarPerfil, string nroTranSB, string fecha, string firmaPago, string origen) {
            return base.Channel.pagarRecibo(idClienteBanco, codigoFactura, tipoServicio, idOrganismo, banco, nroTranBanco, importe, moneda, guardarPerfil, nroTranSB, fecha, firmaPago, origen);
        }
        
        public System.Threading.Tasks.Task<VisaNet.Components.Sistarbanc.Implementations.WsPagosLIF.ResultadoWsPagosLIF> pagarReciboAsync(string idClienteBanco, string codigoFactura, string tipoServicio, string idOrganismo, string banco, string nroTranBanco, string importe, string moneda, string guardarPerfil, string nroTranSB, string fecha, string firmaPago, string origen) {
            return base.Channel.pagarReciboAsync(idClienteBanco, codigoFactura, tipoServicio, idOrganismo, banco, nroTranBanco, importe, moneda, guardarPerfil, nroTranSB, fecha, firmaPago, origen);
        }
    }
}
