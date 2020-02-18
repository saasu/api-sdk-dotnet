using System;
using System.Collections.Generic;
using Ola.RestClient.Dto;
using Saasu.API.Client.Proxies;
using Saasu.API.Core.Globals;
using Saasu.API.Core.Models.Invoices;
using Saasu.API.Core.Framework;
using Ola.RestClient.Proxies;
using System.Collections.Specialized;
using System.Configuration;
using Xunit;
using Saasu.API.Core.Models.Payments;
using InvoiceProxy = Saasu.API.Client.Proxies.InvoiceProxy;

namespace Saasu.API.Client.IntegrationTests
{	
	public class PaymentTests 
	{		
		#region test data properties		
		private static int _assetInventory;
		private static int _coSHardware;
		private static int _incomeHardwareSales;
		private static int _incomeShipping;
		private static int _expenseMisc;
	    private static int _bankAccount01Id;
	    private static int _bankAccount02Id;
		private static int AssetInventory
		{
			get
			{
				if (_assetInventory == 0)
				{
					return CreateTransactionCategory("Asset", "Inventory");
				}
				else
				{
					return _assetInventory;
				}
			}
		}
		private static int CoSHardware{ get
			{
				if (_coSHardware == 0)
				{
					return CreateTransactionCategory("Cost of Sales", "Hardware");
				}
				else
				{
					return _coSHardware;
				}
			}} 
		private static int IncomeHardwareSales
		{
			get
			{
				if (_incomeHardwareSales == 0)
				{
					return CreateTransactionCategory("Income", "Hardware Sales");
				}
				else
				{
					return _incomeHardwareSales;
				}
			}
		}
		private static int IncomeShipping
		{
			get
			{
				if (_incomeShipping == 0)
				{
					return CreateTransactionCategory("Income", "Shipping");
				}
				else
				{
					return _incomeShipping;
				}
			}
		}
		private static int ExpenseMisc
		{
			get
			{
				if (_expenseMisc == 0)
				{
					return CreateTransactionCategory("Expense", "Misc");
				}
				else
				{
					return _expenseMisc;
				}
			}
		}
		#endregion

		public PaymentTests()
		{			
		    SetupBankAccounts();
		}

	    [Fact]        
	    public void InsertAndGetPaymentForSingleInvoice()
	    {
	        var invoice = GetInvoiceTransaction01();
	        var invoiceProxy = new InvoiceProxy();
	        var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

	        Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

	        var invoicePayment = new PaymentTransaction
	                             {
	                                 TransactionDate = DateTime.Now,
	                                 TransactionType = "SP",
	                                 Currency = "AUD",
	                                 Summary =
	                                     string.Format("Test Payment insert for Inv# {0}",
	                                         insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
	                                 PaymentAccountId = _bankAccount01Id,
	                                 PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem()
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 110.00M
	                                                           }
	                                                       }
	                             };

	        var invoicePaymentProxy = new PaymentProxy();
	        var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment);

	        Assert.NotNull(insertInvoicePaymentResponse);
	        Assert.True(insertInvoicePaymentResponse.IsSuccessfull);
	        Assert.NotNull(insertInvoicePaymentResponse.RawResponse);

            var insertInvoicePaymentResult = insertInvoicePaymentResponse.DataObject;


	        Assert.True(insertInvoicePaymentResult.InsertedEntityId > 0, string.Format("There was an error creating the invoice payment for Invoice Id {0}", insertInvoiceResult.DataObject.InsertedEntityId));

	        var getInvoicePaymentResponse = invoicePaymentProxy.GetPayment(insertInvoicePaymentResult.InsertedEntityId);
            var getInvoicePaymentResult = getInvoicePaymentResponse.DataObject;

            Assert.NotNull(getInvoicePaymentResult);
            Assert.True(getInvoicePaymentResult.TransactionId == insertInvoicePaymentResult.InsertedEntityId, "Incorrect payment transaction ID");
            Assert.Equal(110M, getInvoicePaymentResult.TotalAmount);
            Assert.True(getInvoicePaymentResult.PaymentItems.Count == 1, "Incorrect number of payment items.");
            Assert.Null(getInvoicePaymentResult.ClearedDate);

	        foreach (var paymentItem in getInvoicePaymentResult.PaymentItems)
	        {
	            Assert.Equal(paymentItem.InvoiceTransactionId, insertInvoiceResult.DataObject.InsertedEntityId);
                Assert.Equal(paymentItem.AmountPaid, 110M);
	        }
	    }

		[Fact]
	    public void InsertAndGetPaymentForMultipleInvoiecs()
	    {
            var invoice = GetInvoiceTransaction01();
            var invoiceProxy = new InvoiceProxy();
            var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

            Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

	        var invoice01TransctionId = insertInvoiceResult.DataObject.InsertedEntityId; 

            invoice = GetInvoiceTransaction02();
            invoiceProxy = new InvoiceProxy();
            insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

            Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

	        var invoice02TransactionId = insertInvoiceResult.DataObject.InsertedEntityId;

            var invoicePayment = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "SP",
                Currency = "AUD",
                Summary =
                    string.Format("Test Payment insert for multiple invoices"),
                PaymentAccountId = _bankAccount01Id,
                PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem
	                                                           {
	                                                               InvoiceTransactionId = invoice01TransctionId,	                                                                  
	                                                               AmountPaid = 110.00M
	                                                           },

                                                               new PaymentItem
                                                               {
                                                                   InvoiceTransactionId = invoice02TransactionId,
                                                                   AmountPaid = 150.00M
                                                               }

	                                                       }


            };

            var invoicePaymentProxy = new PaymentProxy();
            var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment);

            Assert.NotNull(insertInvoicePaymentResponse);
            Assert.True(insertInvoicePaymentResponse.IsSuccessfull);
            Assert.NotNull(insertInvoicePaymentResponse.RawResponse);

            var insertInvoicePaymentResult = insertInvoicePaymentResponse.DataObject;


            Assert.True(insertInvoicePaymentResult.InsertedEntityId > 0, string.Format("There was an error creating the invoice payment for Invoice Id {0}", insertInvoiceResult.DataObject.InsertedEntityId));

            var getInvoicePaymentResponse = invoicePaymentProxy.GetPayment(insertInvoicePaymentResult.InsertedEntityId);
            var getInvoicePaymentResult = getInvoicePaymentResponse.DataObject;

            Assert.NotNull(getInvoicePaymentResult);
            Assert.True(getInvoicePaymentResult.TransactionId == insertInvoicePaymentResult.InsertedEntityId, "Incorrect payment transaction ID");
            Assert.Equal(260M, getInvoicePaymentResult.TotalAmount);
            Assert.True(getInvoicePaymentResult.PaymentItems.Count == 2, "Incorrect number of payment items.");

	        var invoicePaymentItem01 =
	            getInvoicePaymentResult.PaymentItems.Find(i => i.InvoiceTransactionId == invoice01TransctionId);
	        var invoicePaymentItem02 =
	            getInvoicePaymentResult.PaymentItems.Find(i => i.InvoiceTransactionId == invoice02TransactionId);

	        Assert.NotNull(invoicePaymentItem01);
            Assert.NotNull(invoicePaymentItem02);
            Assert.True(invoicePaymentItem01.AmountPaid == 110M, $"Incorrect amount paid in payment item for invoice transaction Id {invoice01TransctionId}");
            Assert.True(150M == invoicePaymentItem02.AmountPaid,$"Incorrect amount paid in payment item for invoice transaction Id {invoice02TransactionId}");
                   
	    }

        [Fact]
        public void InsertAndGetInvoicePaymentForMultiCcyInvoice()
        {
            var invoice = GetInvoiceTransaction01();
            invoice.Currency = "USD";
            invoice.AutoPopulateFxRate = true;
            var invoiceProxy = new InvoiceProxy();
            var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

            Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

            var insertedInvoiceFromDb = invoiceProxy.GetInvoice(insertInvoiceResult.DataObject.InsertedEntityId);
            var insertedInvoiceAutoPopulatedFxRate = insertedInvoiceFromDb.DataObject.FxRate;

            var invoicePayment = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "SP",
                Currency = "USD",
                AutoPopulateFxRate = true,
                Summary =
                    string.Format("Test Payment insert for Inv# {0}",
                        insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
                PaymentAccountId = _bankAccount01Id,
                PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 110.00M
	                                                           }
	                                                       }
            };

            var invoicePaymentProxy = new PaymentProxy();
            var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment);

            Assert.NotNull(insertInvoicePaymentResponse);
            Assert.True(insertInvoicePaymentResponse.IsSuccessfull);
            Assert.NotNull(insertInvoicePaymentResponse.RawResponse);

            var insertInvoicePaymentResult = insertInvoicePaymentResponse.DataObject;


            Assert.True(insertInvoicePaymentResult.InsertedEntityId > 0, string.Format("There was an error creating the invoice payment for Invoice Id {0}", insertInvoiceResult.DataObject.InsertedEntityId));

            var getInvoicePaymentResponse = invoicePaymentProxy.GetPayment(insertInvoicePaymentResult.InsertedEntityId);
            var getInvoicePaymentResult = getInvoicePaymentResponse.DataObject;

            Assert.NotNull(getInvoicePaymentResult);
            Assert.True(getInvoicePaymentResult.TransactionId == insertInvoicePaymentResult.InsertedEntityId, "Incorrect payment transaction ID");
            Assert.Equal(110M, getInvoicePaymentResult.TotalAmount);
            Assert.True(getInvoicePaymentResult.AutoPopulateFxRate, "Incorrect auto populate Fx Rate status.");
            Assert.Equal(insertedInvoiceAutoPopulatedFxRate, getInvoicePaymentResult.FxRate);
            Assert.True(getInvoicePaymentResult.PaymentItems.Count == 1, "Incorrect number of payment items.");
        }

	    [Fact]
	    public void UpdatePaymentForSingleInvoice()
	    {
            var invoice = GetInvoiceTransaction01();
            var invoiceProxy = new InvoiceProxy();
            var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

            Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

            var invoicePayment = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "SP",
                Currency = "AUD",
                Summary =
                    string.Format("Test Update Payment for Inv# {0}",
                        insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
                PaymentAccountId = _bankAccount01Id,
                PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 30.00M
	                                                           }
	                                                       }
            };

            var invoicePaymentProxy = new PaymentProxy();
            var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment);

            Assert.NotNull(insertInvoicePaymentResponse);
            Assert.True(insertInvoicePaymentResponse.IsSuccessfull);
            Assert.NotNull(insertInvoicePaymentResponse.RawResponse);

	        var insertedInvoicePaymentEntityId = insertInvoicePaymentResponse.DataObject.InsertedEntityId;
	        var insertedInvoicePaymentLastUpdatedId = insertInvoicePaymentResponse.DataObject.LastUpdatedId; 
            Assert.True(insertedInvoicePaymentEntityId > 0, string.Format("There was an error creating the invoice payment for Invoice Id {0}", insertInvoiceResult.DataObject.InsertedEntityId));

            invoicePayment = new PaymentTransaction
            {
                TransactionDate = DateTime.Now.Date,
                ClearedDate = DateTime.Now.Date.AddDays(2).Date,
                TransactionType = "SP",
                Currency = "AUD",
                Summary =
                    string.Format("Test Update Payment for Inv# {0}",
                        insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
                PaymentAccountId = _bankAccount02Id,
                PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 60.00M
	                                                           }
	                                                       },
                RequiresFollowUp = true,
                Notes = "Update payment amount to $60", 
                LastUpdatedId = insertedInvoicePaymentLastUpdatedId
            };

	        var updateInvoicePaymentResponse = invoicePaymentProxy.UpdateInvoicePayment(insertedInvoicePaymentEntityId,
	            invoicePayment);
            Assert.NotNull(updateInvoicePaymentResponse);
            Assert.True(updateInvoicePaymentResponse.IsSuccessfull);

            var getInvoicePaymentResponse = invoicePaymentProxy.GetPayment(insertedInvoicePaymentEntityId);
            var getInvoicePaymentResult = getInvoicePaymentResponse.DataObject;

            Assert.NotNull(getInvoicePaymentResult);
            Assert.True(getInvoicePaymentResult.TransactionId == insertedInvoicePaymentEntityId, "Incorrect payment transaction ID.");
            Assert.Equal(DateTime.Now.Date, getInvoicePaymentResult.TransactionDate);
            Assert.Equal(DateTime.Now.Date.AddDays(2), getInvoicePaymentResult.ClearedDate);
            Assert.Equal("AUD", getInvoicePaymentResult.Currency);
            Assert.Equal("SP", getInvoicePaymentResult.TransactionType);
            Assert.Equal(string.Format("Test Update Payment for Inv# {0}", insertInvoiceResult.DataObject.GeneratedInvoiceNumber), getInvoicePaymentResult.Summary);
            Assert.Equal(60M, getInvoicePaymentResult.TotalAmount);
            Assert.True(getInvoicePaymentResult.PaymentItems.Count == 1, "Incorrect number of payment items.");
            Assert.Equal(_bankAccount02Id, getInvoicePaymentResult.PaymentAccountId);
            Assert.Equal("Update payment amount to $60", getInvoicePaymentResult.Notes);
            Assert.True(getInvoicePaymentResult.PaymentItems.Count == 1, "Incorrect number of payment items.");
            Assert.True(getInvoicePaymentResult.RequiresFollowUp, "Incorrect requires follow up status.");
	        var paymentItem =
	            getInvoicePaymentResult.PaymentItems.Find(
	                i => i.InvoiceTransactionId == insertInvoiceResult.DataObject.InsertedEntityId);
            Assert.True(paymentItem !=null, $"No payment item for invoice transaction Id {insertInvoiceResult.DataObject.InsertedEntityId}");
            Assert.True(60M == paymentItem.AmountPaid, $"Incorrect amount paid in payment item for invoie transaction Id {paymentItem.InvoiceTransactionId}");
	    }

	    [Fact]
	    public void DeleteInvoicePayment()
	    {
	        var invoice = GetInvoiceTransaction01();
	        var invoiceProxy = new InvoiceProxy();
	        var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

	        Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

	        var invoicePayment = new PaymentTransaction
	                             {
	                                 TransactionDate = DateTime.Now,
	                                 TransactionType = "SP",
	                                 Currency = "AUD",
	                                 Summary =
	                                     string.Format("Test Update Payment for Inv# {0}",
	                                         insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
	                                 PaymentAccountId = _bankAccount01Id,
	                                 PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 30.00M
	                                                           }
	                                                       }
	                             };

	        var invoicePaymentProxy = new PaymentProxy();
	        var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment);

	        Assert.NotNull(insertInvoicePaymentResponse);
	        Assert.True(insertInvoicePaymentResponse.IsSuccessfull);
	        Assert.NotNull(insertInvoicePaymentResponse.RawResponse);

	        var deleteInvoicePaymentResponse =
	            invoicePaymentProxy.DeleteInvoicePayment(insertInvoicePaymentResponse.DataObject.InsertedEntityId);

	        Assert.NotNull(deleteInvoicePaymentResponse);
	        Assert.True(deleteInvoicePaymentResponse.IsSuccessfull, "Invoice payment was not deleted successfully.");
	    }

        [Fact]
        public void CannotSavePaymentWithInvalidPaymentItemAmount()
        {
            var invoice = GetInvoiceTransaction01();
            var invoiceProxy = new InvoiceProxy();
            var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

            Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

            var invoicePayment = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "SP",
                Currency = "AUD",
                Summary =
                    string.Format("Test Invalid Payment Amount for Inv# {0}",
                        insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
                PaymentAccountId = _bankAccount01Id,
                PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 0.00M
	                                                           }
	                                                       }
            };

            var invoicePaymentProxy = new PaymentProxy();
            var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment);

            Assert.NotNull(insertInvoicePaymentResponse);
            Assert.False(insertInvoicePaymentResponse.IsSuccessfull);
        }

        [Fact]
        public void CannotApplyPaymentToIncorrectInvoiceTransactionType()
        {
            var invoice = GetInvoiceTransaction01();
            var invoiceProxy = new InvoiceProxy();
            var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

            Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

            var invoicePayment = new PaymentTransaction
            {
                TransactionDate = DateTime.Now,
                TransactionType = "PP",
                Currency = "AUD",
                Summary =
                    string.Format("Test Incorrect Payment Type for Inv# {0}",
                        insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
                PaymentAccountId = _bankAccount01Id,
                PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 110.00M
	                                                           }
	                                                       }
            };

            var invoicePaymentProxy = new PaymentProxy();
            var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment);

            Assert.NotNull(insertInvoicePaymentResponse);
            Assert.False(insertInvoicePaymentResponse.IsSuccessfull);
        }

        [Fact]
        public void ShouldRetreivePaymentSummaryUsingTransactionType()
        {
            var paymentsResponse = new PaymentsProxy().GetPayments(null, null, null, null, "SP", null, null, null, null, null, null,
                null);
            Assert.NotNull(paymentsResponse);
            Assert.True(paymentsResponse.IsSuccessfull, "Payments response was not successful.");
            Assert.NotNull(paymentsResponse.DataObject.PaymentTransactions);
            Assert.True(paymentsResponse.DataObject.PaymentTransactions.Count > 0, "No payment summaries found for transaction type SP");
            foreach (var payment in paymentsResponse.DataObject.PaymentTransactions)
            {
                Assert.True(payment.TransactionType.Equals("SP"), "Incorrect payment transaction type in payment summary.");
            }
        }

        [Fact(Skip="Needs to be fixed.")]
        public void ShouldRetrievePaymentSummaryUsingPaymentAccountId()
        {
            var paymentsResponse = new PaymentsProxy().GetPayments(null, null, null, _bankAccount01Id, null, null, null, null, null, null, null,
               null);
            Assert.NotNull(paymentsResponse);
            Assert.True(paymentsResponse.IsSuccessfull, "Payments response was not successful.");
            Assert.NotNull(paymentsResponse.DataObject.PaymentTransactions);
            Assert.True(paymentsResponse.DataObject.PaymentTransactions.Count > 0, "No payment summaries found for transaction type.");

            foreach (var payment in paymentsResponse.DataObject.PaymentTransactions)
            {
                Assert.Equal(_bankAccount01Id, payment.PaymentAccountId);
            }
        }

        [Fact]
        public void ShouldRetrievePaymentSummaryUsingPaymentDateRange()
        {
            var paymentsResponse = new PaymentsProxy().GetPayments(null, null, null, null, null, null, DateTime.Now.Date.AddDays(-30), DateTime.Now.Date, null, null, null,
               null);
            Assert.NotNull(paymentsResponse);
            Assert.True(paymentsResponse.IsSuccessfull, "Payments response was not successful.");
            Assert.NotNull(paymentsResponse.DataObject.PaymentTransactions);
            Assert.True(paymentsResponse.DataObject.PaymentTransactions.Count > 0, "No payment summaries found for transaction type.");            
        }

        [Fact]
        public void ShouldRetreivePaymentSummaryForInvoiceId()
        {
            var invoice = GetInvoiceTransaction01();
            var invoiceProxy = new InvoiceProxy();
            var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

            Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the first invoice for payment test - ShouldRetreivePaymentSummaryForInvoiceId.");

	        var invoice01TransctionId = insertInvoiceResult.DataObject.InsertedEntityId; 

            invoice = GetInvoiceTransaction02();
            invoiceProxy = new InvoiceProxy();
            insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

            Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the second invoice for payment test - ShouldRetreivePaymentSummaryForInvoiceId.");

	        var invoice02TransactionId = insertInvoiceResult.DataObject.InsertedEntityId;

            var invoicePayment = new PaymentTransaction
                                 {
                                     TransactionDate = DateTime.Now,
                                     TransactionType = "SP",
                                     Currency = "AUD",
                                     Summary =
                                         string.Format("Test Payment insert for multiple invoices"),
                                     PaymentAccountId = _bankAccount01Id,
                                     PaymentItems = new List<PaymentItem>
                                                    {
                                                        new PaymentItem
                                                        {
                                                            InvoiceTransactionId = invoice01TransctionId,
                                                            AmountPaid = 110.00M
                                                        },

                                                        new PaymentItem
                                                        {
                                                            InvoiceTransactionId = invoice02TransactionId,
                                                            AmountPaid = 150.00M
                                                        }

                                                    }
                                 };

            var invoicePaymentProxy = new PaymentProxy();
            var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment);

            Assert.NotNull(insertInvoicePaymentResponse);
            Assert.True(insertInvoicePaymentResponse.IsSuccessfull);
            Assert.NotNull(insertInvoicePaymentResponse.RawResponse);

            var paymentsResponse = new PaymentsProxy().GetPayments(null, null, invoice02TransactionId, null, null, null,
                null, null, null, null, null, null);

            Assert.NotNull(paymentsResponse);
            Assert.True(paymentsResponse.IsSuccessfull, "Payments response was not successful.");
            Assert.NotNull(paymentsResponse.DataObject.PaymentTransactions);
            Assert.True(paymentsResponse.DataObject.PaymentTransactions.Count > 0, "No payment summaries found for transaction type.");
            Assert.Equal(150M, paymentsResponse.DataObject.PaymentTransactions[0].TotalAmount);
        }

		[Fact]
		public void TestPaging()
		{
			var invoice = GetInvoiceTransaction01();
			var invoiceProxy = new InvoiceProxy();
			var insertInvoiceResult = invoiceProxy.InsertInvoice(invoice);

			Assert.True(insertInvoiceResult.DataObject.InsertedEntityId > 0, "There was an error creating the invoice for payment test.");

			var invoicePayment1 = new PaymentTransaction
			{
				TransactionDate = DateTime.Now,
				TransactionType = "SP",
				Currency = "AUD",
				Summary =
					string.Format("Test Payment insert for Inv# {0}",
						insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
				PaymentAccountId = _bankAccount01Id,
				PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem()
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 50.00M
	                                                           }
	                                                       }
			};


			var invoicePaymentProxy = new PaymentProxy();
			var insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment1);

			Assert.NotNull(insertInvoicePaymentResponse);
			Assert.True(insertInvoicePaymentResponse.IsSuccessfull);
			Assert.NotNull(insertInvoicePaymentResponse.RawResponse);

			var invoicePayment2 = new PaymentTransaction
			{
				TransactionDate = DateTime.Now,
				TransactionType = "SP",
				Currency = "AUD",
				Summary =
					string.Format("Test Payment insert for Inv# {0}",
						insertInvoiceResult.DataObject.GeneratedInvoiceNumber),
				PaymentAccountId = _bankAccount01Id,
				PaymentItems = new List<PaymentItem>
	                                                       {
	                                                           new PaymentItem()
	                                                           {
	                                                               InvoiceTransactionId =
	                                                                   insertInvoiceResult.DataObject.InsertedEntityId,
	                                                               AmountPaid = 60.00M
	                                                           }
	                                                       }
			};

			insertInvoicePaymentResponse = invoicePaymentProxy.InsertInvoicePayment(invoicePayment2);

			Assert.NotNull(insertInvoicePaymentResponse);
			Assert.True(insertInvoicePaymentResponse.IsSuccessfull);
			Assert.NotNull(insertInvoicePaymentResponse.RawResponse);

			var paymentsProxy = new PaymentsProxy();

			var paymentsPage1 = paymentsProxy.GetPayments(1, 1, insertInvoiceResult.DataObject.InsertedEntityId, null, "SP", null, null, null, null, null, null, null);

			Assert.NotNull(paymentsPage1);
			Assert.NotNull(paymentsPage1.DataObject);
			Assert.Equal(paymentsPage1.DataObject.PaymentTransactions.Count, 1);

			var paymentsPage2 = paymentsProxy.GetPayments(2, 1, insertInvoiceResult.DataObject.InsertedEntityId, null, "SP", null, null, null, null, null, null, null);

			Assert.NotNull(paymentsPage2);
			Assert.NotNull(paymentsPage2.DataObject);
			Assert.Equal(paymentsPage2.DataObject.PaymentTransactions.Count, 1);
			Assert.NotEqual(paymentsPage1.DataObject.PaymentTransactions[0].TransactionId, paymentsPage2.DataObject.PaymentTransactions[0].TransactionId);

			//Test number of rows returned for page.
			var paymentsPage3 = paymentsProxy.GetPayments(1, 2, insertInvoiceResult.DataObject.InsertedEntityId, null, "SP", null, null, null, null, null, null, null);

			Assert.NotNull(paymentsPage3);
			Assert.NotNull(paymentsPage3.DataObject);
			Assert.Equal(paymentsPage3.DataObject.PaymentTransactions.Count, 2);
		}

		#region Test Data
       		
	    private InvoiceTransactionDetail GetInvoiceTransaction01()
	    {
            var invDetail = new InvoiceTransactionDetail
            {
                LineItems = new List<InvoiceTransactionLineItem>
                            {
                               new InvoiceTransactionLineItem
                               {
                                   AccountId = IncomeHardwareSales,
                                   TotalAmount = 110M 
                               }
                            },                         
                InvoiceNumber = "<Auto Number>",
                InvoiceType =  "Tax Invoice",
                TransactionType = "S",
                Layout = "S",
                Summary = "Test Invoice For Payment",                                                                
                TransactionDate = DateTime.Now                                              
            };

	        return invDetail;
	    }

        private InvoiceTransactionDetail GetInvoiceTransaction02()
        {
            var invDetail = new InvoiceTransactionDetail
            {
                LineItems = new List<InvoiceTransactionLineItem>
                            {
                               new InvoiceTransactionLineItem
                               {
                                   AccountId = IncomeHardwareSales,                                   
                                   TotalAmount = 150M 
                               }
                            },
                InvoiceNumber = "<Auto Number>",
                InvoiceType = "Tax Invoice",
                TransactionType = "S",
                Layout = "S",
                Summary = "Test Invoice For Payment",
                TransactionDate = DateTime.Now
            };

            return invDetail;
        }		
		
		private static void SetupBankAccounts()
		{			
			CrudProxy bankAccount = new BankAccountProxy();

			var bankAccountDto = new Ola.RestClient.Dto.BankAccountDto()
			{
				Type = AccountType.Asset,
				Name = "TestBank " + System.Guid.NewGuid().ToString(),
				DisplayName = "TestBank " + System.Guid.NewGuid().ToString(),
				BSB = "111-111",
				AccountNumber = "12345-6789"				
			};
			bankAccount.Insert(bankAccountDto);
		    _bankAccount01Id = bankAccountDto.Uid;

            bankAccountDto = new Ola.RestClient.Dto.BankAccountDto()
            {
                Type = AccountType.Asset,
                Name = "TestBank " + System.Guid.NewGuid().ToString(),
                DisplayName = "TestBank " + System.Guid.NewGuid().ToString(),
                BSB = "222-222",
                AccountNumber = "2345-6789"
            };
			bankAccount.Insert(bankAccountDto);
		    _bankAccount02Id = bankAccountDto.Uid;
		}
		
		private static int CreateTransactionCategory(string accountType, string accountName)
		{
		    var dto = new Ola.RestClient.Dto.TransactionCategoryDto()
		              {
		                  Type = accountType,
		                  Name = accountName + " " + System.Guid.NewGuid().ToString()
		              };

			new Ola.RestClient.Proxies.TransactionCategoryProxy().Insert(dto);

			var combinedName = string.Concat(accountType.Replace(" ", string.Empty), "|", accountName.Replace(" ", string.Empty));

			switch (combinedName)
			{
				case "Asset|Inventory":
					_assetInventory = dto.Uid;
					break;
				case "Income|HardwareSales":
					_incomeHardwareSales = dto.Uid;
					break;
				case "CostOfSales|Hardware":
					_coSHardware = dto.Uid;
					break;
				case "Expense|Misc":
					_expenseMisc = dto.Uid;
					break;
				case "Income|Shipping":
					_incomeShipping = dto.Uid;
					break;
			}

			return dto.Uid;
		}

		#endregion
	}
}
