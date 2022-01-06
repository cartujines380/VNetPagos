﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Geocom.GFO_02 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="GeoTribUy", ConfigurationName="GFO_02.Ws02BusquedaCCSoapPort")]
    public interface Ws02BusquedaCCSoapPort {
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS02BUSQUEDACC.Execute", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Geocom.GFO_02.ExecuteResponse Execute(Geocom.GFO_02.ExecuteRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS02BUSQUEDACC.Execute", ReplyAction="*")]
        System.Threading.Tasks.Task<Geocom.GFO_02.ExecuteResponse> ExecuteAsync(Geocom.GFO_02.ExecuteRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_BusquedaCargada.SDT_BusquedaCargadaItem", Namespace="GeoTribUy")]
    public partial class SDT_BusquedaCargadaSDT_BusquedaCargadaItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int tipoBusquedaField;
        
        private int numeroParametroField;
        
        private string valorParametroField;
        
        /// <remarks/>
        public int TipoBusqueda {
            get {
                return this.tipoBusquedaField;
            }
            set {
                this.tipoBusquedaField = value;
                this.RaisePropertyChanged("TipoBusqueda");
            }
        }
        
        /// <remarks/>
        public int NumeroParametro {
            get {
                return this.numeroParametroField;
            }
            set {
                this.numeroParametroField = value;
                this.RaisePropertyChanged("NumeroParametro");
            }
        }
        
        /// <remarks/>
        public string ValorParametro {
            get {
                return this.valorParametroField;
            }
            set {
                this.valorParametroField = value;
                this.RaisePropertyChanged("ValorParametro");
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_DatosdePadronesCC.SDT_DatosdePadronesCCItem", Namespace="GeoTribUy")]
    public partial class SDT_DatosdePadronesCCSDT_DatosdePadronesCCItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int codigoIDPadronField;
        
        private string descripcionPadronField;
        
        private string nombreContribuyenteField;
        
        private string montoCuentaCorrienteField;
        
        /// <remarks/>
        public int CodigoIDPadron {
            get {
                return this.codigoIDPadronField;
            }
            set {
                this.codigoIDPadronField = value;
                this.RaisePropertyChanged("CodigoIDPadron");
            }
        }
        
        /// <remarks/>
        public string DescripcionPadron {
            get {
                return this.descripcionPadronField;
            }
            set {
                this.descripcionPadronField = value;
                this.RaisePropertyChanged("DescripcionPadron");
            }
        }
        
        /// <remarks/>
        public string NombreContribuyente {
            get {
                return this.nombreContribuyenteField;
            }
            set {
                this.nombreContribuyenteField = value;
                this.RaisePropertyChanged("NombreContribuyente");
            }
        }
        
        /// <remarks/>
        public string MontoCuentaCorriente {
            get {
                return this.montoCuentaCorrienteField;
            }
            set {
                this.montoCuentaCorrienteField = value;
                this.RaisePropertyChanged("MontoCuentaCorriente");
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
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws02BusquedaCC.Execute", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        public string Tipopadron;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Geocom.GFO_02.SDT_BusquedaCargadaSDT_BusquedaCargadaItem[] Sdtbusquedacargada;
        
        public ExecuteRequest() {
        }
        
        public ExecuteRequest(string Tipopadron, Geocom.GFO_02.SDT_BusquedaCargadaSDT_BusquedaCargadaItem[] Sdtbusquedacargada) {
            this.Tipopadron = Tipopadron;
            this.Sdtbusquedacargada = Sdtbusquedacargada;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws02BusquedaCC.ExecuteResponse", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Geocom.GFO_02.SDT_DatosdePadronesCCSDT_DatosdePadronesCCItem[] Sdtdatosdepadronescc;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        public string Codigoretorno;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=2)]
        public string Mensajeretorno;
        
        public ExecuteResponse() {
        }
        
        public ExecuteResponse(Geocom.GFO_02.SDT_DatosdePadronesCCSDT_DatosdePadronesCCItem[] Sdtdatosdepadronescc, string Codigoretorno, string Mensajeretorno) {
            this.Sdtdatosdepadronescc = Sdtdatosdepadronescc;
            this.Codigoretorno = Codigoretorno;
            this.Mensajeretorno = Mensajeretorno;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Ws02BusquedaCCSoapPortChannel : Geocom.GFO_02.Ws02BusquedaCCSoapPort, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Ws02BusquedaCCSoapPortClient : System.ServiceModel.ClientBase<Geocom.GFO_02.Ws02BusquedaCCSoapPort>, Geocom.GFO_02.Ws02BusquedaCCSoapPort {
        
        public Ws02BusquedaCCSoapPortClient() {
        }
        
        public Ws02BusquedaCCSoapPortClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Ws02BusquedaCCSoapPortClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws02BusquedaCCSoapPortClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws02BusquedaCCSoapPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Geocom.GFO_02.ExecuteResponse Geocom.GFO_02.Ws02BusquedaCCSoapPort.Execute(Geocom.GFO_02.ExecuteRequest request) {
            return base.Channel.Execute(request);
        }
        
        public Geocom.GFO_02.SDT_DatosdePadronesCCSDT_DatosdePadronesCCItem[] Execute(string Tipopadron, Geocom.GFO_02.SDT_BusquedaCargadaSDT_BusquedaCargadaItem[] Sdtbusquedacargada, out string Codigoretorno, out string Mensajeretorno) {
            Geocom.GFO_02.ExecuteRequest inValue = new Geocom.GFO_02.ExecuteRequest();
            inValue.Tipopadron = Tipopadron;
            inValue.Sdtbusquedacargada = Sdtbusquedacargada;
            Geocom.GFO_02.ExecuteResponse retVal = ((Geocom.GFO_02.Ws02BusquedaCCSoapPort)(this)).Execute(inValue);
            Codigoretorno = retVal.Codigoretorno;
            Mensajeretorno = retVal.Mensajeretorno;
            return retVal.Sdtdatosdepadronescc;
        }
        
        public System.Threading.Tasks.Task<Geocom.GFO_02.ExecuteResponse> ExecuteAsync(Geocom.GFO_02.ExecuteRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
    }
}
