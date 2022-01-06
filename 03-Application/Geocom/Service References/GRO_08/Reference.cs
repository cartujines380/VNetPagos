﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Geocom.GRO_08 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="GeoTribUy", ConfigurationName="GRO_08.Ws08CobradoDetalleSoapPort")]
    public interface Ws08CobradoDetalleSoapPort {
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS08COBRADODETALLE.Execute", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Geocom.GRO_08.ExecuteResponse Execute(Geocom.GRO_08.ExecuteRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS08COBRADODETALLE.Execute", ReplyAction="*")]
        System.Threading.Tasks.Task<Geocom.GRO_08.ExecuteResponse> ExecuteAsync(Geocom.GRO_08.ExecuteRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_TotalesCobros.SDT_TotalesCobrosItem", Namespace="GeoTribUy")]
    public partial class SDT_TotalesCobrosSDT_TotalesCobrosItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int numerodeCobroField;
        
        private double montodelCobroField;
        
        /// <remarks/>
        public int NumerodeCobro {
            get {
                return this.numerodeCobroField;
            }
            set {
                this.numerodeCobroField = value;
                this.RaisePropertyChanged("NumerodeCobro");
            }
        }
        
        /// <remarks/>
        public double MontodelCobro {
            get {
                return this.montodelCobroField;
            }
            set {
                this.montodelCobroField = value;
                this.RaisePropertyChanged("MontodelCobro");
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
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws08CobradoDetalle.Execute", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        public int Nrocobrodesde;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        public int Nrocobrohasta;
        
        public ExecuteRequest() {
        }
        
        public ExecuteRequest(int Nrocobrodesde, int Nrocobrohasta) {
            this.Nrocobrodesde = Nrocobrodesde;
            this.Nrocobrohasta = Nrocobrohasta;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws08CobradoDetalle.ExecuteResponse", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Geocom.GRO_08.SDT_TotalesCobrosSDT_TotalesCobrosItem[] Detalletotalescobros;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        public double Totalcobrado;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=2)]
        public int Cantidadcobros;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=3)]
        public string Codigoretorno;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=4)]
        public string Mensajeretorno;
        
        public ExecuteResponse() {
        }
        
        public ExecuteResponse(Geocom.GRO_08.SDT_TotalesCobrosSDT_TotalesCobrosItem[] Detalletotalescobros, double Totalcobrado, int Cantidadcobros, string Codigoretorno, string Mensajeretorno) {
            this.Detalletotalescobros = Detalletotalescobros;
            this.Totalcobrado = Totalcobrado;
            this.Cantidadcobros = Cantidadcobros;
            this.Codigoretorno = Codigoretorno;
            this.Mensajeretorno = Mensajeretorno;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Ws08CobradoDetalleSoapPortChannel : Geocom.GRO_08.Ws08CobradoDetalleSoapPort, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Ws08CobradoDetalleSoapPortClient : System.ServiceModel.ClientBase<Geocom.GRO_08.Ws08CobradoDetalleSoapPort>, Geocom.GRO_08.Ws08CobradoDetalleSoapPort {
        
        public Ws08CobradoDetalleSoapPortClient() {
        }
        
        public Ws08CobradoDetalleSoapPortClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Ws08CobradoDetalleSoapPortClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws08CobradoDetalleSoapPortClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws08CobradoDetalleSoapPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Geocom.GRO_08.ExecuteResponse Geocom.GRO_08.Ws08CobradoDetalleSoapPort.Execute(Geocom.GRO_08.ExecuteRequest request) {
            return base.Channel.Execute(request);
        }
        
        public Geocom.GRO_08.SDT_TotalesCobrosSDT_TotalesCobrosItem[] Execute(int Nrocobrodesde, int Nrocobrohasta, out double Totalcobrado, out int Cantidadcobros, out string Codigoretorno, out string Mensajeretorno) {
            Geocom.GRO_08.ExecuteRequest inValue = new Geocom.GRO_08.ExecuteRequest();
            inValue.Nrocobrodesde = Nrocobrodesde;
            inValue.Nrocobrohasta = Nrocobrohasta;
            Geocom.GRO_08.ExecuteResponse retVal = ((Geocom.GRO_08.Ws08CobradoDetalleSoapPort)(this)).Execute(inValue);
            Totalcobrado = retVal.Totalcobrado;
            Cantidadcobros = retVal.Cantidadcobros;
            Codigoretorno = retVal.Codigoretorno;
            Mensajeretorno = retVal.Mensajeretorno;
            return retVal.Detalletotalescobros;
        }
        
        public System.Threading.Tasks.Task<Geocom.GRO_08.ExecuteResponse> ExecuteAsync(Geocom.GRO_08.ExecuteRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
    }
}
