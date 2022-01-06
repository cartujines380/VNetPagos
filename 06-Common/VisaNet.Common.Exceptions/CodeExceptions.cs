namespace VisaNet.Common.Exceptions
{
    public class CodeExceptions
    {
        #region Users
        public static readonly string USER_NOT_EXIST = "USER_NOT_EXIST";
        public static readonly string USER_EMAIL_DUPLICATED = "USER_EMAIL_DUPLICATED";
        public static readonly string USER_USERNAME_DUPLICATED = "USER_USERNAME_DUPLICATED";
        public static readonly string USER_USERNAME_EMAIL_NOT_MATCH = "USER_USERNAME_EMAIL_NOT_MATCH";
        public static readonly string USER_CARD_NOT_MATCH = "USER_CARD_NOT_MATCH";
        #endregion

        #region ApplicationUsers
        public static readonly string APPLICATION_USER_NOT_EXIST = "APPLICATION_USER_NOT_EXIST";
        public static readonly string APPLICATION_USER_EMAIL_DUPLICATED = "APPLICATION_USER_EMAIL_DUPLICATED";
        public static readonly string APPLICATION_USER_IDENTITY_DUPLICATED = "APPLICATION_USER_IDENTITY_DUPLICATED";
        public static readonly string APPLICATION_USER_OLD_PASSWORD_NOT_MATCH = "APPLICATION_USER_OLD_PASSWORD_NOT_MATCH";
        public static readonly string APPLICATION_USER_MISSING_MEMBERSHIP_IDENTIFIER = "APPLICATION_USER_MISSING_MEMBERSHIP_IDENTIFIER";
        public static readonly string APPLICATION_USER_INCORRECT_PASSWORD = "APPLICATION_USER_INCORRECT_PASSWORD";
        public static readonly string APPLICATION_USER_DISABLED = "APPLICATION_USER_DISABLED";
        public static readonly string APPLICATION_USER_INVALID_TOKEN = "APPLICATION_USER_INVALID_TOKEN";
        public static readonly string APPLICATION_USER_INVALID_CONFIRMATION_TOKEN = "APPLICATION_USER_INVALID_CONFIRMATION_TOKEN";
        public static readonly string APPLICATION_USER_SERVICEASSOCIATED = "APPLICATION_USER_SERVICEASSOCIATED";
        #endregion

        #region AnonymousUser
        public static readonly string ANONYMOUS_USER_EMAIL_DUPLICATED = "ANONYMOUS_USER_EMAIL_DUPLICATED";
        public static readonly string ANONYMOUS_USER_CI_DUPLICATED = "ANONYMOUS_USER_CI_DUPLICATED";
        #endregion

        #region SystemUser
        public static readonly string SYSTEM_USER_NOT_EXIST = "SYSTEM_USER_NOT_EXIST";
        public static readonly string SYSTEM_USER_USERNAME_DUPLICATED = "SYSTEM_USER_USERNAME_DUPLICATED";
        public static readonly string SYSTEM_USER_DISABLED = "SYSTEM_USER_DISABLED";
        #endregion

        #region Roles
        public static readonly string ROLE_USERS_ASSOCIATED = "ROLE_USERS_ASSOCIATED";
        #endregion

        #region ServiceCategory
        public static readonly string SERVICECATEGORY_NAME_ALREADY_USED = "SERVICECATEGORY_NAME_ALREADY_USED";
        public static readonly string SERVICECATEGORY_IN_SERVICE = "SERVICECATEGORY_IN_SERVICE";
        #endregion

        #region Service
        public static readonly string SERVICE_NAME_ALREADY_USED = "SERVICE_NAME_ALREADY_USED";
        public static readonly string SERVICE_CYBERSOURCE_CODE_UNIQUE = "SERVICE_CYBERSOURCE_CODE_UNIQUE";
        public static readonly string SERVICE_BANRED_SISTARBANC_CODE_UNIQUE = "SERVICE_BANRED_SISTARBANC_CODE_UNIQUE";
        public static readonly string SERVICE_ASOCIATED = "SERVICE_ASOCIATED";
        public static readonly string SERVICE_PAYMENTS_ASSOCIATED = "SERVICE_PAYMENTS_ASSOCIATED";
        public static readonly string SERVICE_CARD_IS_DEFAULT = "SERVICE_CARD_IS_DEFAULT";
        public static readonly string SERVICE_CARD_IS_DEFAULT_MIGRATE_SERVICES = "SERVICE_CARD_IS_DEFAULT_MIGRATE_SERVICES";
        public static readonly string SERVICE_CARD_HAS_PAYMENTS = "SERVICE_CARD_HAS_PAYMENTS";
        public static readonly string SERVICE_DEFAULT_CARD_NOTACTIVE = "SERVICE_DEFAULT_CARD_NOTACTIVE";
        public static readonly string SERVICE_CARD_IS_USED_MIGRATE_SERVICES = "SERVICE_CARD_IS_USED_MIGRATE_SERVICES";
        public static readonly string SERVICE_CARD_IS_USED_MIGRATE_DEBITS = "SERVICE_CARD_IS_USED_MIGRATE_DEBITS";
        public static readonly string SERVICE_NOT_DELETED = "SERVICE_NOT_DELETED";

        public static readonly string SERVICE_IS_CONTAINER = "SERVICE_IS_CONTAINER";
        public static readonly string INVALID_CARD_ASOCIATION = "INVALID_CARD_ASOCIATION";

        public static readonly string SERVICE_MERCHANTID_DUPLICATED = "SERVICE_MERCHANTID_DUPLICATED";
        public static readonly string SERVICE_BINGROUP_REQUIRED = "SERVICE_BINGROUP_REQUIRED";
        public static readonly string SERVICE_MOMENTARILY_DISABLED = "SERVICE_MOMENTARILY_DISABLED";
        public static readonly string SERVICE_APPID_DUPLICATED = "SERVICE_APPID_DUPLICATED";
        public static readonly string SERVICE_REFERENCES_EMPTY = "SERVICE_REFERENCES_EMPTY";
        #endregion

        #region Subscriber
        public static readonly string SUBSCRIBER_EMAIL_ALREADY_USED = "SUBSCRIBER_EMAIL_ALREADY_USED";
        #endregion

        #region Faq
        public static readonly string FAQ_QUESTION_ALREADY_USED = "FAQ_QUESTION_ALREADY_USED";
        #endregion

        #region Authentication
        public static readonly string AUTH_LOGIN_ERROR = "AUTH_LOGIN_ERROR";
        #endregion

        #region Bin
        public static readonly string BIN_NAME_USED = "BIN_NAME_USED";
        public static readonly string BIN_VALUE_USED = "BIN_VALUE_USED";
        public static readonly string BIN_VALUE_FOR_SERVICE_NOT_VALID = "BIN_VALUE_FOR_SERVICE_NOT_VALID";
        public static readonly string BIN_VALUE_NOT_RECOGNIZED = "BIN_VALUE_NOT_RECOGNIZED";
        public static readonly string BIN_NOTVALID_FOR_SERVICE = "BIN_NOTVALID_FOR_SERVICE";
        public static readonly string BIN_NOTACTIVE = "BIN_NOTACTIVE";
        public static readonly string BIN_NOTACTIVE2 = "BIN_NOTACTIVE2";
        public static readonly string BIN_COUNTRY_MUST_BE_UY = "BIN_COUNTRY_MUST_BE_UY";
        public static readonly string BIN_COUNTRY_MUST_NOT_BE_UY = "BIN_COUNTRY_MUST_NOT_BE_UY";
        public static readonly string DEFAULT_BIN_CANNOT_BE_EDITED = "DEFAULT_BIN_CANNOT_BE_EDITED";
        public static readonly string DEFAULT_BIN_CANNOT_BE_DELETED = "DEFAULT_BIN_CANNOT_BE_DELETED";
        public static readonly string BIN_BANKID_AFFILIATIONCARD_BANKID = "BIN_BANKID_AFFILIATIONCARD_BANKID";
        public static readonly string BIN_NOTVALID_FOR_ALL = "BIN_NOTVALID_FOR_ALL";
        #endregion

        #region CardType

        public static readonly string CARDTYPE_NOT_DEFINED = "CARDTYPE_NOT_DEFINED";

        #endregion

        #region Sistarbanc
        public static readonly string SISTARBANC_NORESPONSE = "SISTARBANC_NORESPONSE";
        public static readonly string SISTARBANC_NROERROR_2 = "SISTARBANC_NROERROR_2";
        public static readonly string SISTARBANC_NROERROR_9 = "SISTARBANC_NROERROR_9";
        public static readonly string SISTARBANC_NROERROR_50 = "SISTARBANC_NROERROR_50";
        public static readonly string SISTARBANC_NROERROR_57 = "SISTARBANC_NROERROR_57";
        public static readonly string SISTARBANC_NROERROR_29 = "SISTARBANC_NROERROR_29";
        public static readonly string GENERAL_COMUNICATION_ERROR = "GENERAL_COMUNICATION_ERROR";

        #endregion

        #region Banred
        public static readonly string BANRED_NORESPONSE = "BANRED_NORESPONSE";
        public static readonly string BANRED_COMMUNICATION = "BANRED_COMMUNICATION";
        public static readonly string BANRED_FAULT = "BANRED_FAULT";
        public static readonly string BANRED_TIMEOUT = "BANRED_TIMEOUT";
        #endregion

        #region Payment

        public static readonly string PAYMENT_CARD_INVALID = "PAYMENT_CARD_INVALID";
        public static readonly string PAYMENT_NOT_FOUND = "PAYMENT_NOT_FOUND";
        public static readonly string PAYMENT_NO_BILL = "PAYMENT_NO_BILL";
        public static readonly string PAYMENT_USER_NOT_RELATED = "PAYMENT_USER_NOT_RELATED";
        public static readonly string PAYMENT_NOT_FOUND_NROTRNS = "PAYMENT_NOT_FOUND_NROTRNS";
        public static readonly string PAYMENT_STATUS_NOT_VALID = "PAYMENT_STATUS_NOT_VALID";
        public static readonly string BILL_ALREADY_PAID = "BILL_ALREADY_PAID";
        public static readonly string PAYMENT_FIELD48_WRONG = "PAYMENT_FIELD48_WRONG";
        #endregion

        #region cybersource
        public static readonly string CYBERSOURCE_SECRET_KEY = "CYBERSOURCE_SECRET_KEY";
        public static readonly string CYBERSOURCE_ACCESS_KEY = "CYBERSOURCE_ACCESS_KEY";
        public static readonly string CYBERSOURCE_COMMUNICATION = "CYBERSOURCE_COMMUNICATION";
        public static readonly string CYBERSOURCE_FAULT = "CYBERSOURCE_FAULT";
        public static readonly string CYBERSOURCE_TIMEOUT = "CYBERSOURCE_TIMEOUT";
        public static readonly string CYBERSOURCE_VOID_ERROR = "CYBERSOURCE_VOID_ERROR";
        public static readonly string CYBERSOURCE_REFUND_ERROR = "CYBERSOURCE_REFUND_ERROR";

        #endregion

        #region Sucive
        public static readonly string SUCIVE_NORESPONSE = "SUCIVE_NORESPONSE";
        public static readonly string SUCIVE_CONFIRMATION_ERROR = "SUCIVE_CONFIRMATION_ERROR";
        public static readonly string SUCIVE_MULTIPLEIDPADRON = "SUCIVE_MULTIPLEIDPADRON";
        public static readonly string SUCIVE_ERROR090 = "SUCIVE_ERROR090";
        #endregion

        #region Geocom
        public static readonly string GEOCOM_NORESPONSE = "GEOCOM_NORESPONSE";
        public static readonly string GEOCOM_CANTGETBILL = "GEOCOM_CANTGETBILL";
        public static readonly string GEOCOM_CONFIRMATION_ERROR = "GEOCOM_CONFIRMATION_ERROR";
        public static readonly string GEOCOM_MULTIPLEIDPADRON = "GEOCOM_MULTIPLEIDPADRON";
        #endregion

        #region Discounts
        public static readonly string DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL = "DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL";
        public static readonly string DISCOUNT_MULTIPLE_DISCOUNT_FOR_CARDTYPE = "DISCOUNT_MULTIPLE_DISCOUNT_FOR_CARDTYPE";
        public static readonly string DISCOUNT_INVALID_MODEL = "DISCOUNT_INVALID_MODEL";
        public static readonly string DISCOUNT_INVALID_LAW_ID = "DISCOUNT_INVALID_LAW_ID";
        #endregion

        #region Quotations
        public static readonly string QUOTATION_NO_CONFIGURED_FOR_UI__INTERNAL = "QUOTATION_NO_CONFIGURED_FOR_UI__INTERNAL";
        #endregion

        #region Tc33
        public static readonly string TC33_NOT_FOUND = "TC33_NOT_FOUND";
        #endregion

        public static readonly string GENERAL_ERROR = "GENERAL_ERROR";

        public static readonly string BILL_ACCOUNT_NOT_FOUND = "BILL_ACCOUNT_NOT_FOUND";
        public static readonly string ANONYMOUS_USER_MISSING = "ANONYMOUS_USER_MISSING";
        public static readonly string ID_NOT_FOUND = "ID_NOT_FOUND";

        #region Mailgun
        public static readonly string MAILGUN_ADD_ERROR = "MAILGUN_ADD_ERROR";
        public static readonly string MAILGUN_DELETE_ERROR = "MAILGUN_DELETE_ERROR";
        public static readonly string MAILGUN_EMAILNOTFOUND = "MAILGUN_EMAILNOTFOUND";
        #endregion

        #region WebHook
        public static readonly string OPERATION_ID_REPETED = "OPERATION_ID_REPETED";
        public static readonly string WEBBHOOKREGISTRATION_ACCESSTOKEN_EXPIRED = "WEBBHOOKREGISTRATION_ACCESSTOKEN_EXPIRED";
        public static readonly string WEBBHOOKREGISTRATION_ACCESSTOKEN_BAD_IDAPP = "WEBBHOOKREGISTRATION_ACCESSTOKEN_BAD_IDAPP";
        #endregion

        #region Email
        public static readonly string EMAIL_STATUS_CANNOT_BE_CANCELED = "EMAIL_STATUS_CANNOT_BE_CANCELED";

        #endregion

        #region Interpeter
        public static readonly string INTERPRETER_NAME_USED = "INTERPRETER_NAME_USED";
        #endregion

        #region Bin goups

        public static readonly string BINGROUP_NAME_USED = "BINGROUP_NAME_USED";

        public static readonly string BINGROUP_DEFAULT_CANNOT_BE_DELETED = "BINGROUP_DEFAULT_CANNOT_BE_DELETED";
        public static readonly string BINGROUP_DEFAULT_CANNOT_BE_RENAMED = "BINGROUP_DEFAULT_CANNOT_BE_RENAMED";

        #endregion

        #region BANK

        public static readonly string BANK_NAME_USED = "BANK_NAME_USED";
        public static readonly string BANK_CODE_USED = "BANK_CODE_USED";
        public static readonly string BANK_QUOTA_EMPTY = "BANK_QUOTA_EMPTY";
        public static readonly string BANK_FIRST_QUOTA_NOT_SELECTED = "BANK_FIRST_QUOTA_NOT_SELECTED";
        public static readonly string BANK_BROU_MUSTEXISTS = "BANK_BROU_MUSTEXISTS";
        #endregion

        #region Quota
        public static readonly string BANK_DONOT_ALLOW_QUOTA = "BANK_DONOT_ALLOW_QUOTA";
        public static readonly string CARDTYPE_NOT_ALLOWED_QUOTA_PAYMENT = "CARDTYPE_NOT_ALLOWED_QUOTA_PAYMENT";
        public static readonly string SERVICE_NOT_ALLOWED_QUOTA_PAYMENT = "SERVICE_NOT_ALLOWED_QUOTA_PAYMENT";
        #endregion

        #region AUTOMATIC PAYMENT
        public static readonly string AUTOMATIC_PAYMENT_AMOUNT_LESS_THAN_ONE = "AUTOMATIC_PAYMENT_AMOUNT_LESS_THAN_ONE";
        public static readonly string AUTOMATIC_PAYMENT_DAYS_WRONG = "AUTOMATIC_PAYMENT_DAYS_WRONG";
        #endregion

        #region Bill
        public static readonly string BILL_AMOUNT_NOT_ALLOW = "BILL_AMOUNT_NOT_ALLOW";
        public static readonly string BILL_TAXED_AMOUNT_NOT_ALLOW = "BILL_TAXED_AMOUNT_NOT_ALLOW";
        public static readonly string BILL_EXPIRED = "BILL_EXPIRED";
        #endregion

        #region CARD
        public static readonly string CARD_DELETE_ERROR_SERVICEASSOCIATED_WITHOUT_CARD = "CARD_DELETE_ERROR_SERVICEASSOCIATED_WITHOUT_CARD";
        public static readonly string AFFILIATIONCARD_CODE_REPETED = "AFFILIATIONCARD_CODE_REPETED";
        #endregion

        #region CUSTOMER SITE
        public static readonly string USER_EMAIL_MISSING = "USER_EMAIL_MISSING";
        public static readonly string USER_MOBILEPHONE_MISSING = "USER_MOBILEPHONE_MISSING";
        #endregion

        #region ACCESS TOKEN
        public static readonly string ACCESS_TOKEN_INVALID_STATE = "ACCESS_TOKEN_INVALID_STATE";
        public static readonly string ACCESS_TOKEN_EXPIRED = "ACCESS_TOKEN_EXPIRED";
        #endregion

        #region FILES UPLOAD

        public static readonly string INVALID_IMAGE_FORMAT = "INVALID_IMAGE_FORMAT";
        public static readonly string CONNECTION_FAILED = "CONNECTION_FAILED";

        #endregion
    }
}