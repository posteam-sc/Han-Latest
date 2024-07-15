﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POS.POSInterfaceServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://STJCRSPOSAPP/wsSSI", ConfigurationName="POSInterfaceServiceReference.wsSSIWebServiceSoap")]
    public interface wsSSIWebServiceSoap {
        
        // CODEGEN: Generating message contract since message ws_SSI_SendDataRQRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="https://STJCRSPOSAPP/wsSSI/ws_SSI_SendDataRQ", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        POS.POSInterfaceServiceReference.ws_SSI_SendDataRQResponse ws_SSI_SendDataRQ(POS.POSInterfaceServiceReference.ws_SSI_SendDataRQRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://STJCRSPOSAPP/wsSSI/ws_SSI_SendDataRQ", ReplyAction="*")]
        System.Threading.Tasks.Task<POS.POSInterfaceServiceReference.ws_SSI_SendDataRQResponse> ws_SSI_SendDataRQAsync(POS.POSInterfaceServiceReference.ws_SSI_SendDataRQRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://STJCRSPOSAPP/wsSSI")]
    public partial class wsSSIAuthentication : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string applicationKeyField;
        
        private string encryptedKeyField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string applicationKey {
            get {
                return this.applicationKeyField;
            }
            set {
                this.applicationKeyField = value;
                this.RaisePropertyChanged("applicationKey");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string encryptedKey {
            get {
                return this.encryptedKeyField;
            }
            set {
                this.encryptedKeyField = value;
                this.RaisePropertyChanged("encryptedKey");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
                this.RaisePropertyChanged("AnyAttr");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://STJCRSPOSAPP/wsSSI")]
    public partial class ws_SSI_SendDataRS : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string transactionIDField;
        
        private string returnStatusField;
        
        private int recordsReceivedField;
        
        private int recordsImportedField;
        
        private string errorDetailsField;
        
        private string defectiveRowNosField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string TransactionID {
            get {
                return this.transactionIDField;
            }
            set {
                this.transactionIDField = value;
                this.RaisePropertyChanged("TransactionID");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string ReturnStatus {
            get {
                return this.returnStatusField;
            }
            set {
                this.returnStatusField = value;
                this.RaisePropertyChanged("ReturnStatus");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public int RecordsReceived {
            get {
                return this.recordsReceivedField;
            }
            set {
                this.recordsReceivedField = value;
                this.RaisePropertyChanged("RecordsReceived");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public int RecordsImported {
            get {
                return this.recordsImportedField;
            }
            set {
                this.recordsImportedField = value;
                this.RaisePropertyChanged("RecordsImported");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string ErrorDetails {
            get {
                return this.errorDetailsField;
            }
            set {
                this.errorDetailsField = value;
                this.RaisePropertyChanged("ErrorDetails");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string DefectiveRowNos {
            get {
                return this.defectiveRowNosField;
            }
            set {
                this.defectiveRowNosField = value;
                this.RaisePropertyChanged("DefectiveRowNos");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://STJCRSPOSAPP/wsSSI")]
    public partial class Transactions : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string col_01Field;
        
        private string col_02Field;
        
        private string col_03Field;
        
        private string col_04Field;
        
        private string col_05Field;
        
        private string col_06Field;
        
        private string col_07Field;
        
        private string col_08Field;
        
        private string col_09Field;
        
        private string col_10Field;
        
        private string col_11Field;
        
        private string col_12Field;
        
        private string col_13Field;
        
        private string col_14Field;
        
        private string col_15Field;
        
        private string col_16Field;
        
        private string col_17Field;
        
        private string col_18Field;
        
        private string col_19Field;
        
        private string col_20Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Col_01 {
            get {
                return this.col_01Field;
            }
            set {
                this.col_01Field = value;
                this.RaisePropertyChanged("Col_01");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Col_02 {
            get {
                return this.col_02Field;
            }
            set {
                this.col_02Field = value;
                this.RaisePropertyChanged("Col_02");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Col_03 {
            get {
                return this.col_03Field;
            }
            set {
                this.col_03Field = value;
                this.RaisePropertyChanged("Col_03");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string Col_04 {
            get {
                return this.col_04Field;
            }
            set {
                this.col_04Field = value;
                this.RaisePropertyChanged("Col_04");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string Col_05 {
            get {
                return this.col_05Field;
            }
            set {
                this.col_05Field = value;
                this.RaisePropertyChanged("Col_05");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string Col_06 {
            get {
                return this.col_06Field;
            }
            set {
                this.col_06Field = value;
                this.RaisePropertyChanged("Col_06");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string Col_07 {
            get {
                return this.col_07Field;
            }
            set {
                this.col_07Field = value;
                this.RaisePropertyChanged("Col_07");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public string Col_08 {
            get {
                return this.col_08Field;
            }
            set {
                this.col_08Field = value;
                this.RaisePropertyChanged("Col_08");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public string Col_09 {
            get {
                return this.col_09Field;
            }
            set {
                this.col_09Field = value;
                this.RaisePropertyChanged("Col_09");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public string Col_10 {
            get {
                return this.col_10Field;
            }
            set {
                this.col_10Field = value;
                this.RaisePropertyChanged("Col_10");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=10)]
        public string Col_11 {
            get {
                return this.col_11Field;
            }
            set {
                this.col_11Field = value;
                this.RaisePropertyChanged("Col_11");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=11)]
        public string Col_12 {
            get {
                return this.col_12Field;
            }
            set {
                this.col_12Field = value;
                this.RaisePropertyChanged("Col_12");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=12)]
        public string Col_13 {
            get {
                return this.col_13Field;
            }
            set {
                this.col_13Field = value;
                this.RaisePropertyChanged("Col_13");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=13)]
        public string Col_14 {
            get {
                return this.col_14Field;
            }
            set {
                this.col_14Field = value;
                this.RaisePropertyChanged("Col_14");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=14)]
        public string Col_15 {
            get {
                return this.col_15Field;
            }
            set {
                this.col_15Field = value;
                this.RaisePropertyChanged("Col_15");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=15)]
        public string Col_16 {
            get {
                return this.col_16Field;
            }
            set {
                this.col_16Field = value;
                this.RaisePropertyChanged("Col_16");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=16)]
        public string Col_17 {
            get {
                return this.col_17Field;
            }
            set {
                this.col_17Field = value;
                this.RaisePropertyChanged("Col_17");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=17)]
        public string Col_18 {
            get {
                return this.col_18Field;
            }
            set {
                this.col_18Field = value;
                this.RaisePropertyChanged("Col_18");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=18)]
        public string Col_19 {
            get {
                return this.col_19Field;
            }
            set {
                this.col_19Field = value;
                this.RaisePropertyChanged("Col_19");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=19)]
        public string Col_20 {
            get {
                return this.col_20Field;
            }
            set {
                this.col_20Field = value;
                this.RaisePropertyChanged("Col_20");
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://STJCRSPOSAPP/wsSSI")]
    public partial class Parameters : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string application_IDField;
        
        private string col_01Field;
        
        private string col_02Field;
        
        private string col_03Field;
        
        private string timeStampField;
        
        private Transactions[] columnsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string Application_ID {
            get {
                return this.application_IDField;
            }
            set {
                this.application_IDField = value;
                this.RaisePropertyChanged("Application_ID");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Col_01 {
            get {
                return this.col_01Field;
            }
            set {
                this.col_01Field = value;
                this.RaisePropertyChanged("Col_01");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Col_02 {
            get {
                return this.col_02Field;
            }
            set {
                this.col_02Field = value;
                this.RaisePropertyChanged("Col_02");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string Col_03 {
            get {
                return this.col_03Field;
            }
            set {
                this.col_03Field = value;
                this.RaisePropertyChanged("Col_03");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string TimeStamp {
            get {
                return this.timeStampField;
            }
            set {
                this.timeStampField = value;
                this.RaisePropertyChanged("TimeStamp");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=5)]
        public Transactions[] Columns {
            get {
                return this.columnsField;
            }
            set {
                this.columnsField = value;
                this.RaisePropertyChanged("Columns");
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
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ws_SSI_SendDataRQ", WrapperNamespace="https://STJCRSPOSAPP/wsSSI", IsWrapped=true)]
    public partial class ws_SSI_SendDataRQRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="https://STJCRSPOSAPP/wsSSI")]
        public POS.POSInterfaceServiceReference.wsSSIAuthentication wsSSIAuthentication;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://STJCRSPOSAPP/wsSSI", Order=0)]
        public POS.POSInterfaceServiceReference.Parameters ParamData;
        
        public ws_SSI_SendDataRQRequest() {
        }
        
        public ws_SSI_SendDataRQRequest(POS.POSInterfaceServiceReference.wsSSIAuthentication wsSSIAuthentication, POS.POSInterfaceServiceReference.Parameters ParamData) {
            this.wsSSIAuthentication = wsSSIAuthentication;
            this.ParamData = ParamData;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ws_SSI_SendDataRQResponse", WrapperNamespace="https://STJCRSPOSAPP/wsSSI", IsWrapped=true)]
    public partial class ws_SSI_SendDataRQResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://STJCRSPOSAPP/wsSSI", Order=0)]
        public POS.POSInterfaceServiceReference.ws_SSI_SendDataRS ws_SSI_SendDataRQResult;
        
        public ws_SSI_SendDataRQResponse() {
        }
        
        public ws_SSI_SendDataRQResponse(POS.POSInterfaceServiceReference.ws_SSI_SendDataRS ws_SSI_SendDataRQResult) {
            this.ws_SSI_SendDataRQResult = ws_SSI_SendDataRQResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface wsSSIWebServiceSoapChannel : POS.POSInterfaceServiceReference.wsSSIWebServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class wsSSIWebServiceSoapClient : System.ServiceModel.ClientBase<POS.POSInterfaceServiceReference.wsSSIWebServiceSoap>, POS.POSInterfaceServiceReference.wsSSIWebServiceSoap {
        
        public wsSSIWebServiceSoapClient() {
        }
        
        public wsSSIWebServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public wsSSIWebServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsSSIWebServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public wsSSIWebServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        POS.POSInterfaceServiceReference.ws_SSI_SendDataRQResponse POS.POSInterfaceServiceReference.wsSSIWebServiceSoap.ws_SSI_SendDataRQ(POS.POSInterfaceServiceReference.ws_SSI_SendDataRQRequest request) {
            return base.Channel.ws_SSI_SendDataRQ(request);
        }
        
        public POS.POSInterfaceServiceReference.ws_SSI_SendDataRS ws_SSI_SendDataRQ(POS.POSInterfaceServiceReference.wsSSIAuthentication wsSSIAuthentication, POS.POSInterfaceServiceReference.Parameters ParamData) {
            POS.POSInterfaceServiceReference.ws_SSI_SendDataRQRequest inValue = new POS.POSInterfaceServiceReference.ws_SSI_SendDataRQRequest();
            inValue.wsSSIAuthentication = wsSSIAuthentication;
            inValue.ParamData = ParamData;
            POS.POSInterfaceServiceReference.ws_SSI_SendDataRQResponse retVal = ((POS.POSInterfaceServiceReference.wsSSIWebServiceSoap)(this)).ws_SSI_SendDataRQ(inValue);
            return retVal.ws_SSI_SendDataRQResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<POS.POSInterfaceServiceReference.ws_SSI_SendDataRQResponse> POS.POSInterfaceServiceReference.wsSSIWebServiceSoap.ws_SSI_SendDataRQAsync(POS.POSInterfaceServiceReference.ws_SSI_SendDataRQRequest request) {
            return base.Channel.ws_SSI_SendDataRQAsync(request);
        }
        
        public System.Threading.Tasks.Task<POS.POSInterfaceServiceReference.ws_SSI_SendDataRQResponse> ws_SSI_SendDataRQAsync(POS.POSInterfaceServiceReference.wsSSIAuthentication wsSSIAuthentication, POS.POSInterfaceServiceReference.Parameters ParamData) {
            POS.POSInterfaceServiceReference.ws_SSI_SendDataRQRequest inValue = new POS.POSInterfaceServiceReference.ws_SSI_SendDataRQRequest();
            inValue.wsSSIAuthentication = wsSSIAuthentication;
            inValue.ParamData = ParamData;
            return ((POS.POSInterfaceServiceReference.wsSSIWebServiceSoap)(this)).ws_SSI_SendDataRQAsync(inValue);
        }
    }
}
