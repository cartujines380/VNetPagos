﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Geocom.GRO_04 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="GeoTribUy", ConfigurationName="GRO_04.Ws04BConsultaDeudaSoapPort")]
    public interface Ws04BConsultaDeudaSoapPort {
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS04BCONSULTADEUDA.Execute", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Geocom.GRO_04.ExecuteResponse Execute(Geocom.GRO_04.ExecuteRequest request);
        
        // CODEGEN: Generating message contract since the operation has multiple return values.
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS04BCONSULTADEUDA.Execute", ReplyAction="*")]
        System.Threading.Tasks.Task<Geocom.GRO_04.ExecuteResponse> ExecuteAsync(Geocom.GRO_04.ExecuteRequest request);
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_ColeccionImpresionMensajes.SDT_ColeccionImpresionMensajesItem", Namespace="GeoTribUy")]
    public partial class SDT_ColeccionImpresionMensajesSDT_ColeccionImpresionMensajesItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string mensajeFinalField;
        
        /// <remarks/>
        public string MensajeFinal {
            get {
                return this.mensajeFinalField;
            }
            set {
                this.mensajeFinalField = value;
                this.RaisePropertyChanged("MensajeFinal");
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
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_ColeccionImpresionCabezal.SDT_ColeccionImpresionCabezalItem", Namespace="GeoTribUy")]
    public partial class SDT_ColeccionImpresionCabezalSDT_ColeccionImpresionCabezalItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string mensajeCabezalField;
        
        /// <remarks/>
        public string MensajeCabezal {
            get {
                return this.mensajeCabezalField;
            }
            set {
                this.mensajeCabezalField = value;
                this.RaisePropertyChanged("MensajeCabezal");
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
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_ColeccionTotales.SDT_ColeccionTotalesItem", Namespace="GeoTribUy")]
    public partial class SDT_ColeccionTotalesSDT_ColeccionTotalesItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string descripcionConceptoField;
        
        private string montoConceptoField;
        
        /// <remarks/>
        public string DescripcionConcepto {
            get {
                return this.descripcionConceptoField;
            }
            set {
                this.descripcionConceptoField = value;
                this.RaisePropertyChanged("DescripcionConcepto");
            }
        }
        
        /// <remarks/>
        public string MontoConcepto {
            get {
                return this.montoConceptoField;
            }
            set {
                this.montoConceptoField = value;
                this.RaisePropertyChanged("MontoConcepto");
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
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="SDT_ColeccionDetalle.SDT_ColeccionDetalleItem", Namespace="GeoTribUy")]
    public partial class SDT_ColeccionDetalleSDT_ColeccionDetalleItem : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string anioField;
        
        private string vencimientoField;
        
        private string codigoConceptoField;
        
        private string conceptoField;
        
        private string cuotaField;
        
        private string importeField;
        
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
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws04BConsultaDeuda.Execute", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        public int Auxiliarcobro;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Geocom.GRO_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] Sdtcoleccioncobranza;
        
        public ExecuteRequest() {
        }
        
        public ExecuteRequest(int Auxiliarcobro, Geocom.GRO_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] Sdtcoleccioncobranza) {
            this.Auxiliarcobro = Auxiliarcobro;
            this.Sdtcoleccioncobranza = Sdtcoleccioncobranza;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ws04BConsultaDeuda.ExecuteResponse", WrapperNamespace="GeoTribUy", IsWrapped=true)]
    public partial class ExecuteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Geocom.GRO_04.SDT_ColeccionDetalleSDT_ColeccionDetalleItem[] Sdtcolecciondetalle;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=1)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Geocom.GRO_04.SDT_ColeccionTotalesSDT_ColeccionTotalesItem[] Sdtcolecciontotales;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=2)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Geocom.GRO_04.SDT_ColeccionImpresionCabezalSDT_ColeccionImpresionCabezalItem[] Sdtcoleccioncabezalimpresion;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=3)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable=false)]
        public Geocom.GRO_04.SDT_ColeccionImpresionMensajesSDT_ColeccionImpresionMensajesItem[] Sdtcoleccionimpresionmensaje;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=4)]
        public int Numeroprefactura;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=5)]
        public double Montofinal;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=6)]
        public string Codigoretorno;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="GeoTribUy", Order=7)]
        public string Mensajeretorno;
        
        public ExecuteResponse() {
        }
        
        public ExecuteResponse(Geocom.GRO_04.SDT_ColeccionDetalleSDT_ColeccionDetalleItem[] Sdtcolecciondetalle, Geocom.GRO_04.SDT_ColeccionTotalesSDT_ColeccionTotalesItem[] Sdtcolecciontotales, Geocom.GRO_04.SDT_ColeccionImpresionCabezalSDT_ColeccionImpresionCabezalItem[] Sdtcoleccioncabezalimpresion, Geocom.GRO_04.SDT_ColeccionImpresionMensajesSDT_ColeccionImpresionMensajesItem[] Sdtcoleccionimpresionmensaje, int Numeroprefactura, double Montofinal, string Codigoretorno, string Mensajeretorno) {
            this.Sdtcolecciondetalle = Sdtcolecciondetalle;
            this.Sdtcolecciontotales = Sdtcolecciontotales;
            this.Sdtcoleccioncabezalimpresion = Sdtcoleccioncabezalimpresion;
            this.Sdtcoleccionimpresionmensaje = Sdtcoleccionimpresionmensaje;
            this.Numeroprefactura = Numeroprefactura;
            this.Montofinal = Montofinal;
            this.Codigoretorno = Codigoretorno;
            this.Mensajeretorno = Mensajeretorno;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Ws04BConsultaDeudaSoapPortChannel : Geocom.GRO_04.Ws04BConsultaDeudaSoapPort, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Ws04BConsultaDeudaSoapPortClient : System.ServiceModel.ClientBase<Geocom.GRO_04.Ws04BConsultaDeudaSoapPort>, Geocom.GRO_04.Ws04BConsultaDeudaSoapPort {
        
        public Ws04BConsultaDeudaSoapPortClient() {
        }
        
        public Ws04BConsultaDeudaSoapPortClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Ws04BConsultaDeudaSoapPortClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws04BConsultaDeudaSoapPortClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws04BConsultaDeudaSoapPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Geocom.GRO_04.ExecuteResponse Geocom.GRO_04.Ws04BConsultaDeudaSoapPort.Execute(Geocom.GRO_04.ExecuteRequest request) {
            return base.Channel.Execute(request);
        }
        
        public Geocom.GRO_04.SDT_ColeccionDetalleSDT_ColeccionDetalleItem[] Execute(int Auxiliarcobro, Geocom.GRO_04.SDT_ColeccionCobranzaSDT_ColeccionCobranzaItem[] Sdtcoleccioncobranza, out Geocom.GRO_04.SDT_ColeccionTotalesSDT_ColeccionTotalesItem[] Sdtcolecciontotales, out Geocom.GRO_04.SDT_ColeccionImpresionCabezalSDT_ColeccionImpresionCabezalItem[] Sdtcoleccioncabezalimpresion, out Geocom.GRO_04.SDT_ColeccionImpresionMensajesSDT_ColeccionImpresionMensajesItem[] Sdtcoleccionimpresionmensaje, out int Numeroprefactura, out double Montofinal, out string Codigoretorno, out string Mensajeretorno) {
            Geocom.GRO_04.ExecuteRequest inValue = new Geocom.GRO_04.ExecuteRequest();
            inValue.Auxiliarcobro = Auxiliarcobro;
            inValue.Sdtcoleccioncobranza = Sdtcoleccioncobranza;
            Geocom.GRO_04.ExecuteResponse retVal = ((Geocom.GRO_04.Ws04BConsultaDeudaSoapPort)(this)).Execute(inValue);
            Sdtcolecciontotales = retVal.Sdtcolecciontotales;
            Sdtcoleccioncabezalimpresion = retVal.Sdtcoleccioncabezalimpresion;
            Sdtcoleccionimpresionmensaje = retVal.Sdtcoleccionimpresionmensaje;
            Numeroprefactura = retVal.Numeroprefactura;
            Montofinal = retVal.Montofinal;
            Codigoretorno = retVal.Codigoretorno;
            Mensajeretorno = retVal.Mensajeretorno;
            return retVal.Sdtcolecciondetalle;
        }
        
        public System.Threading.Tasks.Task<Geocom.GRO_04.ExecuteResponse> ExecuteAsync(Geocom.GRO_04.ExecuteRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
    }
}
