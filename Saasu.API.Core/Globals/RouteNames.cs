using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saasu.API.Core.Globals
{
    public static class RouteNames
    {
        public const string ContactWithId = "~/" + ResourceNames.Contact + "/{id}";
        public const string ContactWithoutId = "~/" + ResourceNames.Contact;
        public const string Contacts = "~/" + ResourceNames.Contacts;

        public const string InvoiceWithId = "~/" + ResourceNames.Invoice + "/{id}";
        public const string InvoiceWithoutId = "~/" + ResourceNames.Invoice;
        public const string Invoices = "~/" + ResourceNames.Invoices;

        public const string PaymentWithId = "~/" + ResourceNames.Payment + "/{id}";
        public const string PaymentWithoutId = "~/" + ResourceNames.Payment;
        public const string Payments = "~/" + ResourceNames.Payments;

        public const string InvoiceAttachmentWithId = "~/" + ResourceNames.InvoiceAttachment + "/{id}";
        public const string InvoiceAttachmentWithoutId = "~/" + ResourceNames.InvoiceAttachment;
        public const string InvoiceAttachmentsWithoutId = "~/" + ResourceNames.InvoiceAttachments;
        public const string InvoiceAttachmentsWithId = "~/" + ResourceNames.InvoiceAttachments + "/{id}";

        public const string AccountWithId = "~/" + ResourceNames.Account + "/{id}";
        public const string AccountWithoutId = "~/" + ResourceNames.Account;
        public const string Accounts = "~/" + ResourceNames.Accounts;

        public const string FileIdentityWithId = "~/" + ResourceNames.FileIdentity + "/{id}";
        public const string FileIdentityWithoutId = "~/" + ResourceNames.FileIdentity;

        public const string TaxCodeWithId = "~/" + ResourceNames.TaxCode + "/{id}";
        public const string TaxCodeWithoutId = "~/" + ResourceNames.TaxCode;
        public const string TaxCodes = "~/" + ResourceNames.TaxCodes;

        public const string ItemWithId = "~/" + ResourceNames.Item + "/{id}";
        public const string ItemWithoutId = "~/" + ResourceNames.Item;
        public const string Items = "~/" + ResourceNames.Items;

        public const string CompanyWithId = "~/" + ResourceNames.Company+ "/{id}";
        public const string CompanyWithoutId = "~/" + ResourceNames.Company;
        public const string Companies = "~/" + ResourceNames.Companies;

        public const string Search = "~/" + ResourceNames.Search;
    }
}
