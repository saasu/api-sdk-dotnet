﻿namespace Saasu.API.Core.Globals
{
    public static class ApiConstants
    {
        public const string WsAccessKeyQueryArg = "wsaccesskey";
        public const string WsAccessKeyQueryArgDescriptiveName = "WsAccessKey";
        public const string FileQueryArg = "fileid";
        public const string FileQueryArgDescriptiveName = "FileId";
        public const string PageQueryArg = "page";
        public const string PageQueryArgDescriptiveName = "Page";
        public const string PageSizeQueryArg = "pagesize";
        public const string PageSizeQueryArgDescriptiveName = "PageSize";
        public const string OptionalQueryArgKey = "optionalargs";
        public const string OptionalQueryArgKeyDescriptiveName = "OptionalArgs";
        public const string OptionalQueryArgsTemplate = "{*" + OptionalQueryArgKey + "}";

        public const string IdQueryArgParameter = "{id}";
        public const string ResetPasswordUrlPath = "reset-password/";

        public const string GenerateInvoicePdfPath = "generate-pdf/";
        public const string EmailInvoiceToContactUrlPath = "email-contact/";
        public const string EmailInvoiceUrlPath = "email/";
        public const string BuildComboItemPath = "build/";

        public const string UserSessionContext_FileGroupUidKey = "FileGroupUid";
        public const string UserSessionContext_UserUidKey = "UserUid";
        public const string UserSessionContext_FileUidKey = "FileUid";

        // Filter options for arguments
        public const string FilterHelpDate = "YYYY-MM-DD";
        public const string FilterHelpNumber = "123";
        public const string FilterHelpText = "some text";
        public const string FilterHelpEmail = "email@somewhere.com";
        public const string FilterHelpBool = "1|0|true|false|yes|no";
        public const string FilterHelpInvoiceStatus = "I|Q|O";
        public const string FilterHelpPaymentStatus = "P|U|A";
        public const string FilterHelpTags = "tag1,tag2";
        public const string FilterHelpTagFilterType = "requireAll|requireAny|excludeAll|excludeAny";
        public const string FilterHelpTransactionTypeInvoices = "S|P";
        public const string FilterHelpTransactionTypePayments = "SP|PP";
        public const string FilterHelpResultSortString = "TransactionDate";
        public const string FilterHelpAccountType = "Income|Expense|Asset|Equity|Liability|OtherIncome|OtherExpense|CostOfSales";
        public const string FilterHelpHeaderAccountId = "111";
        public const string FilterSearchMethodString = "Contains|StartsWith";
        public const string FilterHelpItemTypes = "I|C";
        public const string FilterHelpActivityType = "Any tags with activity flag set to true";
        public const string FilterHelpActivityStatus = "todo|done|overdue";
        public const string FilterHelpActivityAttachedToType = "Contact|Employee|Sale|Purchase";
        public const string FilterHelpSearchText = "some text (max 128 characters)";
        public const string FilterDeletedEntitiesEntityTypes = "Sale|Purchase|SalePayment|PurchasePayment|Item|Contact|Journal";

        // Filtering query args
        public const int DefaultPageSize = 25;
        public const int DefaultPageNumber = 1;

        public const string FilterFromDate = "FromDate";
        public const string FilterExampleFromDate = FilterFromDate + "={" + FilterHelpDate + "}";
        public const string FilterToDate = "ToDate";
        public const string FilterExampleToDate = FilterToDate + "={" + FilterHelpDate + "}";
        public const string FilterLastModifiedDate = "LastModifiedDate";
        public const string FilterExampleLastModifiedDate = FilterLastModifiedDate + "={" + FilterHelpDate + "}";
        public const string FilterLastModifiedToDate = "LastModifiedToDate";
        public const string FilterExampleLastModifiedToDate = FilterLastModifiedToDate + "={" + FilterHelpDate + "}";
        public const string FilterLastModifiedFromDate = "LastModifiedFromDate";
        public const string FilterExampleLastModifiedFromDate = FilterLastModifiedFromDate + "={" + FilterHelpDate + "}";
        public const string FilterInvoiceNumber = "InvoiceNumber";
        public const string FilterExampleInvoiceNumber = FilterInvoiceNumber + "={" + FilterHelpNumber + "}";
        public const string FilterPurchaseOrderNumber = "PurchaseOrderNumber";
        public const string FilterExamplePurchaseOrderNumber = FilterPurchaseOrderNumber + "={" + FilterHelpNumber + "}";
        public const string FilterTransactionTypeInvoices = "TransactionType";
        public const string FilterExampleTransactionTypeInvoices = FilterTransactionTypeInvoices + "={" + FilterHelpTransactionTypeInvoices + "}";
        public const string FilterBillingContactId = "BillingContactId";
        public const string FilterExampleBillingContactId = FilterBillingContactId + "={" + FilterHelpNumber + "}";
        public const string FilterGivenName = "GivenName";
        public const string FilterExampleGivenName = FilterGivenName + "={" + FilterHelpText + "}";
        public const string FilterFamilyName = "FamilyName";
        public const string FilterExampleFamilyName = FilterFamilyName + "={" + FilterHelpText + "}";
        public const string FilterCompanyName = "CompanyName";
        public const string FilterExampleCompanyName = FilterCompanyName + "={" + FilterHelpText + "}";
        public const string FilterIsActive = "IsActive";
        public const string FilterExampleIsActive = FilterIsActive + "={" + FilterHelpBool + "}";
        public const string FilterIsCustomer = "IsCustomer";
        public const string FilterExampleIsCustomer = FilterIsCustomer + "={" + FilterHelpBool + "}";
        public const string FilterIsSupplier = "IsSupplier";
        public const string FilterExampleIsSupplier = FilterIsSupplier + "={" + FilterHelpBool + "}";
        public const string FilterIsContractor = "IsContractor";
        public const string FilterExampleIsContractor = FilterIsContractor + "={" + FilterHelpBool + "}";
        public const string FilterIsPartner = "IsPartner";
        public const string FilterExampleIsPartner = FilterIsPartner + "={" + FilterHelpBool + "}";
        public const string FilterTags = "Tags";
        public const string FilterExampleTags = FilterTags + "={" + FilterHelpTags + "}";
        public const string FilterTagFilterType = "TagSelection";
        public const string FilterExampleTagFilterType = FilterTagFilterType + "={" + FilterHelpTagFilterType + "}";
        public const string FilterEmail = "Email";
        public const string FilterExampleEmail = FilterEmail + "={" + FilterHelpEmail + "}";
        public const string FilterContactId = "ContactId";
        public const string FilterTemplateId = "TemplateId";
        public const string FilterExampleContactId = FilterContactId + "={" + FilterHelpText + "}";
        public const string FilterInvoiceFromDate = "InvoiceFromDate";
        public const string FilterExampleInvoiceFromDate = FilterInvoiceFromDate + "={" + FilterHelpDate + "}";
        public const string FilterInvoiceToDate = "InvoiceToDate";
        public const string FilterExampleInvoiceToDate = FilterInvoiceToDate + "={" + FilterHelpDate + "}";
        public const string FilterInvoiceStatus = "InvoiceStatus";
        public const string FilterExampleInvoiceStatus = FilterInvoiceStatus + "={" + FilterHelpInvoiceStatus + "}";
        public const string FilterPaymentStatus = "PaymentStatus";
        public const string FilterExamplePaymentStatus = FilterPaymentStatus + "={" + FilterHelpPaymentStatus + "}";

        public const string FilterJournalContactId = "JournalContactId";
        public const string FilterExampleJournalContactId = FilterJournalContactId + "={" + FilterHelpNumber + "}";
        public const string FilterEntityType = "EntityType";
        public const string FilterExampleEntityType = FilterEntityType + "={" + FilterHelpNumber + "}";
        public const string FilterUtcDeletedFromDate = "UtcDeletedFromDate";
        public const string FilterExampleUtcDeletedFromDate = FilterUtcDeletedFromDate + "={" + FilterHelpDate + "}";
        public const string FilterUtcDeletedToDate = "UtcDeletedToDate";
        public const string FilterExampleUtcDeletedToDate = FilterUtcDeletedToDate + "={" + FilterHelpDate + "}";



        // Payments specific filters options
        public const string FilterTransactionTypePayments = FilterTransactionTypeInvoices;  // yes they are the same and this is intentional
        public const string FilterExampleTransactionTypePayments = FilterTransactionTypePayments + "={" + FilterHelpTransactionTypePayments + "}";
        public const string FilterDateClearedFromDate = "ClearedFromDate";
        public const string FilterExampleDateClearedFromDate = FilterDateClearedFromDate + "={" + FilterHelpDate + "}";
        public const string FilterDateClearedToDate = "ClearedToDate";
        public const string FilterExampleDateClearedToDate = FilterDateClearedToDate + "={" + FilterHelpDate + "}";
        public const string FilterPaymentsForInvoiceId = "ForInvoiceId";
        public const string FilterExamplePaymentsForInvoiceId = FilterPaymentsForInvoiceId + "={" + FilterHelpNumber + "}";
        public const string FilterPaymentDateFrom = "PaymentFromDate";
        public const string FilterExamplePaymentDateFrom = FilterPaymentDateFrom + "={" + FilterHelpDate + "}";
        public const string FilterPaymentDateTo = "PaymentToDate";
        public const string FilterExamplePaymentDateTo = FilterPaymentDateTo + "={" + FilterHelpDate + "}";
        public const string FilterPaymentAccountId = "PaymentAccountId";
        public const string FilterExampleBankAccountId = FilterPaymentAccountId + "={" + FilterHelpNumber + "}";
        public const string FilterResultsSortString = "SortString";
        public const string FilterExampleResultsSortString =
            FilterResultsSortString + "={" + FilterHelpResultSortString + "}";

        //Accounts specific filter options.
        public const string FilterAccountType = "AccountType";
        public const string FilterExampleAccountType = FilterAccountType + "={" + FilterHelpAccountType + "}";
        public const string FilterIsBankAccount = "IsBankAccount";
        public const string FilterExampleIsBankAccount = FilterIsBankAccount + "={" + FilterHelpBool + "}";
        public const string FilterIncludeBuiltIn = "IncludeBuiltIn";
        public const string FilterExampleIncludeBuiltIn = FilterIncludeBuiltIn + "={" + FilterHelpBool + "}";
        public const string FilterHeaderAccountId = "HeaderAccountId";
        public const string FilterExampleHeaderAccountId = FilterHeaderAccountId + "={" + FilterHelpHeaderAccountId + "}";

        public const string FilterItemType = "ItemType";
        public const string FilterExampleItemType = FilterItemType + "={" + FilterHelpItemTypes + "}";
        public const string FilterSearchMethod = "SearchMethod";
        public const string FilterExampleSearchMethod = FilterSearchMethod + "={" + FilterSearchMethodString + "}";
        public const string FilterSearchText = "SearchText";
        public const string FilterExampleSearchText = FilterSearchText + "={" + FilterHelpText + "}";

        //Activity specific filter options.
        public const string FilterActivityType = "ActivityType";
        public const string FilterActivityTypeExmaple = FilterActivityType + "={" + FilterHelpActivityType + "}";

        public const string FilterActivityStatus = "ActivityStatus";
        public const string FilterActivityStatusExmaple = FilterActivityStatus + "={" + FilterHelpActivityStatus + "}";

        public const string FilterActivityOwnerEmail = "OwnerEmail";
        public const string FilterActivityOwnerEmailExmaple = FilterActivityOwnerEmail + "={" + FilterHelpEmail + "}";

        public const string FilterActivityAttachedToType = "AttachedToType";
        public const string FilterActivityAttachedToTypeExmaple = FilterActivityAttachedToType + "={" + FilterHelpActivityAttachedToType + "}";

        public const string FilterAttachedToId = "AttachedToId";
        public const string FilterAttachedToIdExample = FilterAttachedToId + IdQueryArgParameter;

        //Search (Jump) filter options
        public const string FilterKeywords = "Keywords";
        public const string FilterExampleKeywords = FilterKeywords + "={" + FilterHelpText + "}";

        public const string FilterHelpSearchScope = "All|Transactions|Contacts|InventoryItems";
        public const string FilterSearchScope = "Scope";
        public const string FilterExampleSearchScope = FilterSearchScope + "={" + FilterHelpSearchScope + "}";

        public const string FilterHelpSearchTransactionType = "Transactions.Sale|Transactions.Purchase|Transactions.Journal|Transactions.Payroll";
        public const string FilterSearchTransactionType = "TransactionType";
        public const string FilterExampleSearchTransactionType = FilterSearchTransactionType + "={" + FilterHelpSearchTransactionType + "}";

        public const string FilterIncludeSearchTermHighlights = "IncludeSearchTermHighlights";
        public const string FilterExampleIncludeSearchTermHighlights = FilterIncludeSearchTermHighlights + "={" + FilterHelpBool + "}";

    }

}
