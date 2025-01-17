﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sucive.RN_06 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="GeoTribUy", ConfigurationName="RN_06.Ws06ReversoSoapPort")]
    public interface Ws06ReversoSoapPort {
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS06REVERSO.Execute", ReplyAction="*")]
        Sucive.RN_06.ExecuteResponse Execute(Sucive.RN_06.ExecuteRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="GeoTribUyaction/AWS06REVERSO.Execute", ReplyAction="*")]
        System.Threading.Tasks.Task<Sucive.RN_06.ExecuteResponse> ExecuteAsync(Sucive.RN_06.ExecuteRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ExecuteRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Ws06Reverso.Execute", Namespace="GeoTribUy", Order=0)]
        public Sucive.RN_06.ExecuteRequestBody Body;
        
        public ExecuteRequest() {
        }
        
        public ExecuteRequest(Sucive.RN_06.ExecuteRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="GeoTribUy")]
    public partial class ExecuteRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=0)]
        public long Numeroprefactura;
        
        [System.Runtime.Serialization.DataMemberAttribute(Order=1)]
        public int Idpadcod;
        
        public ExecuteRequestBody() {
        }
        
        public ExecuteRequestBody(long Numeroprefactura, int Idpadcod) {
            this.Numeroprefactura = Numeroprefactura;
            this.Idpadcod = Idpadcod;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ExecuteResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Ws06Reverso.ExecuteResponse", Namespace="GeoTribUy", Order=0)]
        public Sucive.RN_06.ExecuteResponseBody Body;
        
        public ExecuteResponse() {
        }
        
        public ExecuteResponse(Sucive.RN_06.ExecuteResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="GeoTribUy")]
    public partial class ExecuteResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string Codigoretorno;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string Mensajeretorno;
        
        public ExecuteResponseBody() {
        }
        
        public ExecuteResponseBody(string Codigoretorno, string Mensajeretorno) {
            this.Codigoretorno = Codigoretorno;
            this.Mensajeretorno = Mensajeretorno;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Ws06ReversoSoapPortChannel : Sucive.RN_06.Ws06ReversoSoapPort, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Ws06ReversoSoapPortClient : System.ServiceModel.ClientBase<Sucive.RN_06.Ws06ReversoSoapPort>, Sucive.RN_06.Ws06ReversoSoapPort {
        
        public Ws06ReversoSoapPortClient() {
        }
        
        public Ws06ReversoSoapPortClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Ws06ReversoSoapPortClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws06ReversoSoapPortClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Ws06ReversoSoapPortClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Sucive.RN_06.ExecuteResponse Sucive.RN_06.Ws06ReversoSoapPort.Execute(Sucive.RN_06.ExecuteRequest request) {
            return base.Channel.Execute(request);
        }
        
        public string Execute(long Numeroprefactura, int Idpadcod, out string Mensajeretorno) {
            Sucive.RN_06.ExecuteRequest inValue = new Sucive.RN_06.ExecuteRequest();
            inValue.Body = new Sucive.RN_06.ExecuteRequestBody();
            inValue.Body.Numeroprefactura = Numeroprefactura;
            inValue.Body.Idpadcod = Idpadcod;
            Sucive.RN_06.ExecuteResponse retVal = ((Sucive.RN_06.Ws06ReversoSoapPort)(this)).Execute(inValue);
            Mensajeretorno = retVal.Body.Mensajeretorno;
            return retVal.Body.Codigoretorno;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Sucive.RN_06.ExecuteResponse> Sucive.RN_06.Ws06ReversoSoapPort.ExecuteAsync(Sucive.RN_06.ExecuteRequest request) {
            return base.Channel.ExecuteAsync(request);
        }
        
        public System.Threading.Tasks.Task<Sucive.RN_06.ExecuteResponse> ExecuteAsync(long Numeroprefactura, int Idpadcod) {
            Sucive.RN_06.ExecuteRequest inValue = new Sucive.RN_06.ExecuteRequest();
            inValue.Body = new Sucive.RN_06.ExecuteRequestBody();
            inValue.Body.Numeroprefactura = Numeroprefactura;
            inValue.Body.Idpadcod = Idpadcod;
            return ((Sucive.RN_06.Ws06ReversoSoapPort)(this)).ExecuteAsync(inValue);
        }
    }
}
