using System;

namespace VisaNet.LIF.WebApi.Areas.HelpPage.Samples
{
    public static class HardCodedSamples
    {

        public const string XmlGenericResponse = "Las respuestas de la API son siempre en formato json";

        #region CardData

        public const string CardDataInModel_urlencoded = "AppId=myAppId&Bin=424242";
        public const string CardDataInModel_json = "{\n\t'AppId': 'myAppId',\n\t'Bin': '424242',\n}";
        public const string CardDataInModel_xml = "<CardDataInModel>\n\t<AppId>myAppId</AppId>\n\t<Bin>424242</Bin>\n</CardDataInModel>";

        public const string CardDataOutModel_json = "{\n\t'Data': {\n\t\t'CardType': 'C',\n\t\t'Issuer': 'BROU',\n\t\t'IsLocal': true,\n\t\t'Bin': '424242',\n\t\t'Installments': [\n\t\t\t1,\n\t\t\t2,\n\t\t\t3\n\t\t]\n\t},\n\t'Code': 0\n}";
        //public const string CardDataOutModel_xml = "<CardDataOutModel_Sample>\n\t<Code>0</Code>\n\t<Data>\n\t\t<d2p1:Bin>424242</d2p1:Bin>\n\t\t<d2p1:CardType>C</d2p1:CardType>\n\t\t<d2p1>\n\t\t\t<d3p1:int>1</d3p1:int>\n\t\t\t<d3p1:int>2</d3p1:int>\n\t\t\t<d3p1:int>3</d3p1:int>\n\t\t</d2p1:Installments>\n\t\t<d2p1:IsLocal>true</d2p1:IsLocal>\n\t\t<d2p1:Issuer>BROU</d2p1:Issuer>\n\t</Data>\n</CardDataOutModel_Sample>";

        #endregion

        #region NationalData

        public const string NationalDataInModel_urlencoded = "AppId=myAppId";
        public const string NationalDataInModel_json = "{\n\t'AppId': 'myAppId'\n}";
        public const string NationalDataInModel_xml = "<CardDataInModel>\n\t<AppId>myAppId</AppId>\n</CardDataInModel>";

        public const string NationalDataOutModel_json = "{\n\t'Data': [{\n\t\t'Value': '424242'\n\t\t'CardType': 'C',\n\t\t'IssuingCompany': 'BROU',\n\t\t'National': true,\n\t\t'Installments': [\n\t\t\t1,\n\t\t\t2,\n\t\t\t3\n\t\t]\n\t},\n\t{\n\t\t'Value': '411111'\n\t\t'CardType': 'D',\n\t\t'IssuingCompany': 'BROU',\n\t\t'National': true,\n\t\t'Installments': [\n\t\t\t1,\n\t\t\t2\n\t\t]\n\t}\n\t],\n\t'Code': 0\n}";
        //public const string NationalDataOutModel_xml = "<NationalDataOutModel_Sample>\n\t<Code>1</Code>\n\t<Data>\n\t\t<d2p1:Bin>\n\t\t\t<d2p1:CardType>C</d2p1:CardType>\n\t\t\t<d2p1>\n\t\t\t\t<d4p1:int>1</d4p1:int>\n\t\t\t\t<d4p1:int>2</d4p1:int>\n\t\t\t\t<d4p1:int>3</d4p1:int>\n\t\t\t</d2p1:Installments>\n\t\t\t<d2p1:IssuingCompany>BROU</d2p1:IssuingCompany>\n\t\t\t<d2p1:National>true</d2p1:National>\n\t\t\t<d2p1:Value>424242</d2p1:Value>\n\t\t</d2p1:Bin>\n\t\t<d2p1:Bin>\n\t\t\t<d2p1:CardType>D</d2p1:CardType>\n\t\t\t<d2p1:Installments>\n\t\t\t\t<d4p1:int>1</d4p1:int>\n\t\t\t\t<d4p1:int>2</d4p1:int>\n\t\t\t</d2p1:Installments>\n\t\t\t<d2p1:IssuingCompany>BROU</d2p1:IssuingCompany>\n\t\t\t<d2p1:National>true</d2p1:National>\n\t\t\t<d2p1:Value>411111</d2p1:Value>\n\t\t</d2p1:Bin>\n\t</Data>\n</NationalDataOutModel_Sample>";

        #endregion

        #region DiscountCalculationApp

        public const string DiscountCalculationAppInModel_urlencoded = "AppId=myAppId&Bill.Amount=146179&Bill.Currency=UYU&Bill.IsFinalConsumer=true&Bill.LawId=6&Bill.TaxedAmount>146179&Bin=424242&OperationId=myOperationId";
        public const string DiscountCalculationAppInModel_json = "{\n\t'AppId': 'myAppId',\n\t'Bill': {\n\t\t'Amount': 146179,\n\t\t'Currency': 'UYU',\n\t\t'IsFinalConsumer': 'true',\n\t\t'LawId': 6,\n\t\t'TaxedAmount': 146179\n\t},\n\t'Bin': '424242',\n\t'OperationId': 'myOperationId'\n}";
        public const string DiscountCalculationAppInModel_xml = "<DiscountCalculationAppInModel>\n\t<AppId>myAppId</AppId>\n\t<Bill>\n\t\t<Amount>146179</Amount>\n\t\t<Currency>UYU</Currency>\n\t\t<IsFinalConsumer>true</IsFinalConsumer>\n\t\t<LawId>6</LawId>\n\t\t<TaxedAmount>146179</TaxedAmount>\n\t</Bill>\n\t<Bin>424242</Bin>\n\t<OperationId>myOperationId</OperationId>\n</DiscountCalculationAppInModel>";

        public const string DiscountCalculationAppOutModel_json = "{\n\t'Data': {\n\t\t'CardType': 'C',\n\t\t'IssuingCompany': 'BROU',\n\t\t'DiscountAmount': 7908,\n\t\t'AmountToCyberSource': 146179\n\t},\n\t'Code': 0\n}";
        //public const string DiscountCalculationAppOutModel_xml = "<DiscountCalculationAppOutModel_Sample>\n\t<Code>0</Code>\n\t<Data>\n\t\t<d2p1:AmountToCyberSource>1461,79</d2p1:AmountToCyberSource>\n\t\t<d2p1:CardType>C</d2p1:CardType>\n\t\t<d2p1:DiscountAmount>79,08</d2p1:DiscountAmount>\n\t\t<d2p1:IssuingCompany>BROU</d2p1:IssuingCompany>\n\t</Data>\n</DiscountCalculationAppOutModel_Sample>";

        #endregion

    }
}