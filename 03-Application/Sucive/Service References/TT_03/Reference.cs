﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sucive.TT_03 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="GeoTribUy", ConfigurationName="TT_03.Ws03ConsultaDeudaCCSoapPort")]
    public interface Ws03ConsultaDeudaCCSoapPort {
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS03CONSULTADEUDACC.Execute", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Sucive.TT_03.ExecuteResponse Execute(Sucive.TT_03.ExecuteRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS03CONSULTADEUDACC.Execute", ReplyAction="*")]
        System.Threading.Tasks.Task<Sucive.TT_03.ExecuteResponse> ExecuteAsync(Sucive.TT_03.ExecuteRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_ColeccionCobranza.SDT_ColeccionCobranzaItem", Namespace="GeoTribUy")]
    public partial class SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string pagaField;
        
        private string permitidoField;
        
        private string anioField;
        
        private string vencimientoField;
        
        private string codigoConceptoField;
        
        private string conceptoField;
        
        private string cuotaField;
        
        private string importeField;
        
        private short lineaField;
        
        /// <remarks/>
        public string Paga {
            get {
                return this.pagaField;
            }
            set {
                this.pagaField = value;
                this.RaisePropertyChanged("Paga");
            }
        }
        
        /// <remarks/>
        public string Permitido {
            get {
                return this.permitidoField;
            }
            set {
                this.permitidoField = value;
                this.RaisePropertyChanged("Permitido");
            }
        }
        
        /// <remarks/>
        public string Anio {
            get {
                return this.anioField;
            }
            set {
                this.anioField = value;
                this.RaisePropertyChanged("Anio");
            }
        }
        
        /// <remarks/>
        public string Vencimiento {
            get {
                return this.vencimientoField;
            }
            set {
                this.vencimientoField = value;
                this.RaisePropertyChanged("Vencimiento");
            }
        }
        
        /// <remarks/>
        public string CodigoConcepto {
            get {
                return this.codigoConceptoField;
            }
            set {
                this.codigoConceptoField = value;
                this.RaisePropertyChanged("CodigoConcepto");
            }
        }
        
        /// <remarks/>
        public string Concepto {
            get {
                return this.conceptoField;
            }
            set {
                this.conceptoField = value;
                this.RaisePropertyChanged("Concepto");
            }
        }
        
        /// <remarks/>
        public string Cuota {
            get {
                return this.cuotaField;
            }
            set {
                this.cuotaField = value;
                this.RaisePropertyChanged("Cuota");
            }
        }
        
        /// <remarks/>
        public string Importe {
            get {
                return this.importeField;
            }
            set {
                this.importeField = value;
                this.RaisePropertyChanged("Importe");
            }
        }
        
        /// <remarks/>
        public short Linea {
            get {
                return this.lineaField;
            }
            set {
                this.lineaField = value;
                this.RaisePropertyChanged("Linea");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws03ConsultaDeudaCC.Execute", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        public int Idpadron;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        public string Utilizacc;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=2)]
        public string Tipoclave;
        
        public ExecuteRequest() {
        }
        
        public ExecuteRequest(int Idpadron, string Utilizacc, string Tipoclave) {
            this.Idpadron = Idpadron;
            this.Utilizacc = Utilizacc;
            this.Tipoclave = Tipoclave;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws03ConsultaDeudaCC.ExecuteResponse", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Sucive.TT_03.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] Sdtcoleccioncobranza;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        public long Auxiliarcobro;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=2)]
        public string Codigoretorno;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=3)]
        public string Mensajeretorno;
        
        public ExecuteResponse() {
        }
        
        public ExecuteResponse(Sucive.TT_03.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] Sdtcoleccioncobranza, long Auxiliarcobro, string Codigoretorno, string Mensajeretorno) {
            this.Sdtcoleccioncobranza = Sdtcoleccioncobranza;
            this.Auxiliarcobro = Auxiliarcobro;
            this.Codigoretorno = Codigoretorno;
            this.Mensajeretorno = Mensajeretorno;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Ws03ConsultaDeudaCCSoapPortChannel : Sucive.TT_03.Ws03ConsultaDeudaCCSoapPort, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Ws03ConsultaDeudaCCSoapPortClient : System.ServiceModel.ClientBase<Sucive.TT_03.Ws03ConsultaDeudaCCSoapPort>, Sucive.TT_03.Ws03ConsultaDeudaCCSoapPort {
        
        public Ws03ConsultaDeudaCCSoapPortClient() {
        }
        
        public Ws03ConsultaDeudaCCSoapPortClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Ws03ConsultaDeudaCCSoapPortClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws03ConsultaDeudaCCSoapPortClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws03ConsultaDeudaCCSoapPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Sucive.TT_03.ExecuteResponse Sucive.TT_03.Ws03ConsultaDeudaCCSoapPort.Execute(Sucive.TT_03.ExecuteRequest request) {
            return base.Channel.Execute(request);
        }
        
        public Sucive.TT_03.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] Execute(int Idpadron, string Utilizacc, string Tipoclave, out long Auxiliarcobro, out string Codigoretorno, out string Mensajeretorno) {
            Sucive.TT_03.ExecuteRequest inValue = new Sucive.TT_03.ExecuteRequest();
            inValue.Idpadron = Idpadron;
            inValue.Utilizacc = Utilizacc;
            inValue.Tipoclave = Tipoclave;
            Sucive.TT_03.ExecuteResponse retVal = ((Sucive.TT_03.Ws03ConsultaDeudaCCSoapPort)(this)).Execute(inValue);
            Auxiliarcobro = retVal.Auxiliarcobro;
            Codigoretorno = retVal.Codigoretorno;
            Mensajeretorno = retVal.Mensajeretorno;
            return retVal.Sdtcoleccioncobranza;
        }
        
        public System.Threading.Tasks.Task<Sucive.TT_03.ExecuteResponse> ExecuteAsync(Sucive.TT_03.ExecuteRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
    }
}
