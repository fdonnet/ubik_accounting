using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Commands;
using Ubik.Accounting.SalesOrVatTax.Contracts.AccountTaxRateConfigs.Results;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using FluentAssertions;
using Ubik.Accounting.Transaction.Contracts.Entries.Enums;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Api.Tests.Integration.Features.Accounting.Transaction.Txs
{
    public class TxsController_Test : BaseIntegrationTestAccountingSalesVatTax
    {

        private readonly string _baseUrlForV1;
        private readonly HttpClient _client;

        public TxsController_Test(IntegrationTestProxyFactory factory) : base(factory)
        {
            _baseUrlForV1 = "/accounting/api/v1";
            _client = Factory.CreateDefaultClient();
        }

        [Fact]
        public async Task Submit_Tx_WithRW_OK()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1000,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<TxSubmitted>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
            .NotBeNull()
                .And.BeOfType<TxSubmitted>()
                .And.Match<TxSubmitted>(x => x.Amount == command.Amount);
        }

        [Fact]
        public async Task Submit_Tx_WithRO_403()
        {
            var token = await GetAccessTokenAsync(TokenType.RO);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1000,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Submit_Tx_WithNoToken_401()
        {
            var command = new SubmitTxCommand()
            {
                Amount = 1000,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Submit_Tx_WithAdmin_403()
        {
            var token = await GetAccessTokenAsync(TokenType.MegaAdmin);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1000,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Submit_Tx_WithNoRole_403()
        {
            var token = await GetAccessTokenAsync(TokenType.NoRole);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1000,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Submit_Tx_WithOtherTenant_400()
        {
            var token = await GetAccessTokenAsync(TokenType.OtherTenant);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1000,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND_FOR_ENTRY");
        }

        [Fact]
        public async Task Submit_Tx_WithMoreThanOneMainEntry_400()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1000,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TX_COUNTAINS_MORE_THAN_ONE_MAIN_ENTRY");
        }

        [Theory]
        [InlineData(1001, 1000, 1000)]
        [InlineData(1000, 1001, 1000)]
        [InlineData(1000, 1000, 1001)]
        public async Task Submit_Tx_WithBadBalance_400(decimal total, decimal mainAmount, decimal cpAmount)
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = total,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = mainAmount,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = cpAmount,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TX_BALANCE_ERROR");
        }

        [Fact]
        public async Task Submit_Tx_WithExchangeRateAtZero_400()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1000,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1000,
                        AmountAdditionnalInfo = new SubmitTxEntryAdditionalAmountInfo()
                        {
                            ExchangeRate = 0,
                            OriginalAmount = 900,
                            OriginalCurrencyId = new Guid("248e0000-5dd4-0015-291d-08dcd98e55f8")
                        },
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1000,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "BAD_EXCHANGE_RATE_PARAMS_FOR_ENTRY");
        }

        [Fact]
        public async Task Submit_Tx_WithExchangeBadCalc_400()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1100,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1100,
                        AmountAdditionnalInfo = new SubmitTxEntryAdditionalAmountInfo()
                        {
                            ExchangeRate = 1.11m,
                            OriginalAmount = 1000,
                            OriginalCurrencyId = new Guid("248e0000-5dd4-0015-291d-08dcd98e55f8")
                        },
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1100,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "BAD_EXCHANGE_RATE_PARAMS_FOR_ENTRY");
        }

        [Fact]
        public async Task Submit_Tx_WithBadCurrencyId_400()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1100,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1100,
                        AmountAdditionnalInfo = new SubmitTxEntryAdditionalAmountInfo()
                        {
                            ExchangeRate = 1.1m,
                            OriginalAmount = 1000,
                            OriginalCurrencyId = new Guid("248e0000-5dd4-0015-291d-08dcd98e55f1")
                        },
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1100,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "BAD_CURRENCY_PARAMS_FOR_ENTRY");
        }

        [Fact]
        public async Task Submit_Tx_WithBadAccountId_400()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1100,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c71"),
                        Amount = 1100,
                        AmountAdditionnalInfo = new SubmitTxEntryAdditionalAmountInfo()
                        {
                            ExchangeRate = 1.1m,
                            OriginalAmount = 1000,
                            OriginalCurrencyId = new Guid("248e0000-5dd4-0015-291d-08dcd98e55f8")
                        },
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = null,
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1100,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "ACCOUNT_NOT_FOUND_FOR_ENTRY");
        }

        [Fact]
        public async Task Submit_Tx_WithAllInfo_OK()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1100,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1100,
                        AmountAdditionnalInfo = new SubmitTxEntryAdditionalAmountInfo()
                        {
                            ExchangeRate = 1.1m,
                            OriginalAmount = 1000,
                            OriginalCurrencyId = new Guid("248e0000-5dd4-0015-291d-08dcd98e55f8")
                        },
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = new SubmitTxEntryTaxInfo()
                        {
                            TaxAppliedRate = 8.1m,
                            TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                        },
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1100,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<TxSubmitted>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should()
            .NotBeNull()
                .And.BeOfType<TxSubmitted>()
                .And.Match<TxSubmitted>(x => x.Amount == command.Amount);
        }

        [Fact]
        public async Task Submit_Tx_WithMissingTaxRate_OK()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1100,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1100,
                        AmountAdditionnalInfo = new SubmitTxEntryAdditionalAmountInfo()
                        {
                            ExchangeRate = 1.1m,
                            OriginalAmount = 1000,
                            OriginalCurrencyId = new Guid("248e0000-5dd4-0015-291d-08dcd98e55f8")
                        },
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = new SubmitTxEntryTaxInfo()
                        {
                            TaxAppliedRate = 8.1m,
                            TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b911"),
                        },
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1100,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAX_RATE_NOT_FOUND_FOR_ENTRY");
        }

        [Fact]
        public async Task Submit_Tx_WithNotMatchingTaxRate_OK()
        {
            var token = await GetAccessTokenAsync(TokenType.RW);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var command = new SubmitTxCommand()
            {
                Amount = 1100,
                ValueDate = DateOnly.FromDateTime(DateTime.Now),
                Entries =
                [
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-c1ce-08dcd98b7c74"),
                        Amount = 1100,
                        AmountAdditionnalInfo = new SubmitTxEntryAdditionalAmountInfo()
                        {
                            ExchangeRate = 1.1m,
                            OriginalAmount = 1000,
                            OriginalCurrencyId = new Guid("248e0000-5dd4-0015-291d-08dcd98e55f8")
                        },
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Debit,
                        TaxInfo = new SubmitTxEntryTaxInfo()
                        {
                            TaxAppliedRate = 8.0m,
                            TaxRateId = new Guid("08740000-3c36-7456-6f96-08dcfb48b915"),
                        },
                        Type = EntryType.Main
                    },
                    new SubmitTxEntry()
                    {
                        AccountId = new Guid("248e0000-5dd4-0015-48fb-08dcd98b4a28"),
                        Amount = 1100,
                        AmountAdditionnalInfo = null,
                        Description = "Test",
                        Label = "Test",
                        Sign = DebitCredit.Credit,
                        TaxInfo = null,
                        Type = EntryType.Counterparty
                    }
                ]
            };

            //Act
            var response = await _client.PostAsJsonAsync($"{_baseUrlForV1}/txs/submit", command);
            var result = await response.Content.ReadFromJsonAsync<CustomProblemDetails>();

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should()
            .NotBeNull()
                .And.BeOfType<CustomProblemDetails>()
                .And.Match<CustomProblemDetails>(x => x.Errors.First().Code == "TAX_RATE_NOT_MATCH_FOR_ENTRY");
        }
    }
}
