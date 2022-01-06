﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sucive.PA_05 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="GeoTribUy", ConfigurationName="PA_05.Ws05ConfirmacionSoapPort")]
    public interface Ws05ConfirmacionSoapPort {
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS05CONFIRMACION.Execute", ReplyAction="*")]
        Sucive.PA_05.ExecuteResponse Execute(Sucive.PA_05.ExecuteRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS05CONFIRMACION.Execute", ReplyAction="*")]
        System.Threading.Tasks.Task<Sucive.PA_05.ExecuteResponse> ExecuteAsync(Sucive.PA_05.ExecuteRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ExecuteRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Ws05Confirmacion.Execute", Namespace="GeoTribUy", Order=0)]
        public Sucive.PA_05.ExecuteRequestBody Body;
        
        public ExecuteRequest() {
        }
        
        public ExecuteRequest(Sucive.PA_05.ExecuteRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="GeoTribUy")]
    public partial class ExecuteRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long Numeroprefactura;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string Sucursal;
        
        public ExecuteRequestBody() {
        }
        
        public ExecuteRequestBody(long Numeroprefactura, string Sucursal) {
            this.Numeroprefactura = Numeroprefactura;
            this.Sucursal = Sucursal;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ExecuteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Ws05Confirmacion.ExecuteResponse", Namespace="GeoTribUy", Order=0)]
        public Sucive.PA_05.ExecuteResponseBody Body;
        
        public ExecuteResponse() {
        }
        
        public ExecuteResponse(Sucive.PA_05.ExecuteResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="GeoTribUy")]
    public partial class ExecuteResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long Numerocobro;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string Codigoretorno;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string Mensajeretorno;
        
        public ExecuteResponseBody() {
        }
        
        public ExecuteResponseBody(long Numerocobro, string Codigoretorno, string Mensajeretorno) {
            this.Numerocobro = Numerocobro;
            this.Codigoretorno = Codigoretorno;
            this.Mensajeretorno = Mensajeretorno;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Ws05ConfirmacionSoapPortChannel : Sucive.PA_05.Ws05ConfirmacionSoapPort, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Ws05ConfirmacionSoapPortClient : System.ServiceModel.ClientBase<Sucive.PA_05.Ws05ConfirmacionSoapPort>, Sucive.PA_05.Ws05ConfirmacionSoapPort {
        
        public Ws05ConfirmacionSoapPortClient() {
        }
        
        public Ws05ConfirmacionSoapPortClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Ws05ConfirmacionSoapPortClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws05ConfirmacionSoapPortClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws05ConfirmacionSoapPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Sucive.PA_05.ExecuteResponse Sucive.PA_05.Ws05ConfirmacionSoapPort.Execute(Sucive.PA_05.ExecuteRequest request) {
            return base.Channel.Execute(request);
        }
        
        public long Execute(long Numeroprefactura, string Sucursal, out string Codigoretorno, out string Mensajeretorno) {
            Sucive.PA_05.ExecuteRequest inValue = new Sucive.PA_05.ExecuteRequest();
            inValue.Body = new Sucive.PA_05.ExecuteRequestBody();
            inValue.Body.Numeroprefactura = Numeroprefactura;
            inValue.Body.Sucursal = Sucursal;
            Sucive.PA_05.ExecuteResponse retVal = ((Sucive.PA_05.Ws05ConfirmacionSoapPort)(this)).Execute(inValue);
            Codigoretorno = retVal.Body.Codigoretorno;
            Mensajeretorno = retVal.Body.Mensajeretorno;
            return retVal.Body.Numerocobro;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Sucive.PA_05.ExecuteResponse> Sucive.PA_05.Ws05ConfirmacionSoapPort.ExecuteAsync(Sucive.PA_05.ExecuteRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
        
        public System.Threading.Tasks.Task<Sucive.PA_05.ExecuteResponse> ExecuteAsync(long Numeroprefactura, string Sucursal) {
            Sucive.PA_05.ExecuteRequest inValue = new Sucive.PA_05.ExecuteRequest();
            inValue.Body = new Sucive.PA_05.ExecuteRequestBody();
            inValue.Body.Numeroprefactura = Numeroprefactura;
            inValue.Body.Sucursal = Sucursal;
            return ((Sucive.PA_05.Ws05ConfirmacionSoapPort)(this)).ExecuteAsync(inValue);
        }
    }
}
