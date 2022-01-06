﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sucive.AR_08 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="GeoTribUy", ConfigurationName="AR_08.Ws08bCobradoDetalleSoapPort")]
    public interface Ws08bCobradoDetalleSoapPort {
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS08BCOBRADODETALLE.Execute", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Sucive.AR_08.ExecuteResponse Execute(Sucive.AR_08.ExecuteRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS08BCOBRADODETALLE.Execute", ReplyAction="*")]
        System.Threading.Tasks.Task<Sucive.AR_08.ExecuteResponse> ExecuteAsync(Sucive.AR_08.ExecuteRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_WS08bTotalesCobros.SDT_WS08bTotalesCobrosItem", Namespace="GeoTribUy")]
    public partial class SDT_WS08bTotalesCobrosSDT_WS08bTotalesCobrosItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int numerodeCobroField;
        
        private double montodelCobroField;
        
        private sbyte codigoCobroField;
        
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
        
        /// <remarks/>
        public sbyte CodigoCobro {
            get {
                return this.codigoCobroField;
            }
            set {
                this.codigoCobroField = value;
                this.RaisePropertyChanged("CodigoCobro");
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
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws08bCobradoDetalle.Execute", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        public long Nrocobrodesde;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        public long Nrocobrohasta;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="date")]
        public System.DateTime Fechacobro;
        
        public ExecuteRequest() {
        }
        
        public ExecuteRequest(long Nrocobrodesde, long Nrocobrohasta, System.DateTime Fechacobro) {
            this.Nrocobrodesde = Nrocobrodesde;
            this.Nrocobrohasta = Nrocobrohasta;
            this.Fechacobro = Fechacobro;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws08bCobradoDetalle.ExecuteResponse", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Sucive.AR_08.SDT_WS08bTotalesCobrosSDT_WS08bTotalesCobrosItem[] Sdttotales;
        
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
        
        public ExecuteResponse(Sucive.AR_08.SDT_WS08bTotalesCobrosSDT_WS08bTotalesCobrosItem[] Sdttotales, double Totalcobrado, int Cantidadcobros, string Codigoretorno, string Mensajeretorno) {
            this.Sdttotales = Sdttotales;
            this.Totalcobrado = Totalcobrado;
            this.Cantidadcobros = Cantidadcobros;
            this.Codigoretorno = Codigoretorno;
            this.Mensajeretorno = Mensajeretorno;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Ws08bCobradoDetalleSoapPortChannel : Sucive.AR_08.Ws08bCobradoDetalleSoapPort, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Ws08bCobradoDetalleSoapPortClient : System.ServiceModel.ClientBase<Sucive.AR_08.Ws08bCobradoDetalleSoapPort>, Sucive.AR_08.Ws08bCobradoDetalleSoapPort {
        
        public Ws08bCobradoDetalleSoapPortClient() {
        }
        
        public Ws08bCobradoDetalleSoapPortClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Ws08bCobradoDetalleSoapPortClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws08bCobradoDetalleSoapPortClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws08bCobradoDetalleSoapPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Sucive.AR_08.ExecuteResponse Sucive.AR_08.Ws08bCobradoDetalleSoapPort.Execute(Sucive.AR_08.ExecuteRequest request) {
            return base.Channel.Execute(request);
        }
        
        public Sucive.AR_08.SDT_WS08bTotalesCobrosSDT_WS08bTotalesCobrosItem[] Execute(long Nrocobrodesde, long Nrocobrohasta, System.DateTime Fechacobro, out double Totalcobrado, out int Cantidadcobros, out string Codigoretorno, out string Mensajeretorno) {
            Sucive.AR_08.ExecuteRequest inValue = new Sucive.AR_08.ExecuteRequest();
            inValue.Nrocobrodesde = Nrocobrodesde;
            inValue.Nrocobrohasta = Nrocobrohasta;
            inValue.Fechacobro = Fechacobro;
            Sucive.AR_08.ExecuteResponse retVal = ((Sucive.AR_08.Ws08bCobradoDetalleSoapPort)(this)).Execute(inValue);
            Totalcobrado = retVal.Totalcobrado;
            Cantidadcobros = retVal.Cantidadcobros;
            Codigoretorno = retVal.Codigoretorno;
            Mensajeretorno = retVal.Mensajeretorno;
            return retVal.Sdttotales;
        }
        
        public System.Threading.Tasks.Task<Sucive.AR_08.ExecuteResponse> ExecuteAsync(Sucive.AR_08.ExecuteRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
    }
}
