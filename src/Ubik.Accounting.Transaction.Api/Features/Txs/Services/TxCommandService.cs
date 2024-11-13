using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Transaction.Api.Data;
using Ubik.Accounting.Transaction.Api.Features.Txs.Errors;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;
using MassTransit;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.Accounting.Transaction.Api.Mappers;
using Ubik.Accounting.Transaction.Contracts.Entries.Enums;
using Ubik.Accounting.Transaction.Api.Models;
using Ubik.ApiService.Common.Services;
using Ubik.ApiService.Common.Configure;
using Ubik.Accounting.Transaction.Contracts.Txs.Enums;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Services
{
    //TODO: implement a better error management for transaction. Return a special errors payload
    //with all the details for each error, the user needs to receive ONE error payload.
    //or maybe not... return when 1 error or test for all errors...
    public class TxCommandService(AccountingTxContext ctx
        , IPublishEndpoint publishEndpoint) : ITxCommandService
    {
        public async Task<Either<IFeatureError, TxSubmitted>> SubmitTxAsync(SubmitTxCommand command)
        {
            return await ValidateOnlyOneMainEntryAsync(command)
                .BindAsync(ValidateBalanceAsync)
                .BindAsync(ValidateEntriesInfoAsync) //Exchange rates, amounts and currencies
                .BindAsync(ValidateEntriesAccountsAsync)
                .BindAsync(ValidateEntriesTaxRatesInfoAsync)
                .BindAsync(AddSubmittedTxInDbContextAsync)
                .BindAsync(SaveAndPublishSubmittedAsync);
        }

        public async Task<Either<IFeatureError, Tx>> ChangeTxStateAsync(ChangeTxStateCommand command)
        {
            return await GetTxAsync(command.TxId)
                .BindAsync(t => ChangeTxStateInDbContextAsync(t, new TxStateInfo
                {
                    State = command.State,
                    Reason = command.Reason
                } ))
                .BindAsync(SaveNewTxStateAsync);
        }

        public async Task SendTaxValidationRequest(TxSubmitted tx)
        {
            await publishEndpoint.Publish(new TxTaxValidationRequestSent
            {
                Id = tx.Id,
                Tx = tx
            }, CancellationToken.None);
        }

        public bool CheckIfTxNeedTaxValidation(TxSubmitted tx)
        {
            return tx.Entries.Any(e => e.TaxInfo != null);
        }

        private async Task<Either<IFeatureError, Tx>> SaveNewTxStateAsync(Tx tx)
        {
            await ctx.SaveChangesAsync();
            return tx;
        }

        private async Task<Either<IFeatureError, Tx>> GetTxAsync(Guid id)
        {
            var result = await ctx.Txs.FindAsync(id);

            if (result == null)
                return new ResourceNotFoundError("Tx", "Id", id.ToString());
            else
                return result;
        }

        private async Task<Either<IFeatureError, Tx>> ChangeTxStateInDbContextAsync(Tx current, TxStateInfo newState)
        {
            current.State = newState;
            ctx.Entry(current).State = EntityState.Modified;

            ctx.SetAuditAndSpecialFields();

            await Task.CompletedTask;
            return current;
        }

        private async Task<Either<IFeatureError, TxSubmitted>> AddSubmittedTxInDbContextAsync(SubmitTxCommand command)
        {
            var newTx = command.ToTx();
            ctx.Txs.Add(newTx);

            var txEntries = command.Entries.Select(e => e.ToEntry(newTx.Id));

            foreach (var entry in txEntries)
            {
                ctx.Entries.Add(entry);
            }

            ctx.SetAuditAndSpecialFields();

            var submittedTx = newTx.ToTxSubmitted(txEntries);

            await Task.CompletedTask;
            return submittedTx;
        }

        private async Task<Either<IFeatureError, TxSubmitted>> SaveAndPublishSubmittedAsync(TxSubmitted current)
        {
            await publishEndpoint.Publish(current, CancellationToken.None);
            await ctx.SaveChangesAsync();

            return current;
        }

        private static async Task<Either<IFeatureError, SubmitTxCommand>> ValidateOnlyOneMainEntryAsync(SubmitTxCommand tx)
        {
            var count = tx.Entries.Count(e => e.Type == EntryType.Main);

            await Task.CompletedTask;

            return count == 1
                ? tx
                : new MoreThanOneMainEntryError();
        }

        private async Task<Either<IFeatureError, SubmitTxCommand>> ValidateBalanceAsync(SubmitTxCommand tx)
        {
            var totalDebit = tx.Entries.Where(e => e.Sign == DebitCredit.Debit).Sum(e => e.Amount);
            var totalCredit = tx.Entries.Where(e => e.Sign == DebitCredit.Credit).Sum(e => e.Amount);

            await Task.CompletedTask;

            return totalDebit == totalCredit
                && totalCredit == tx.Amount
                && totalDebit == tx.Amount
                ? tx
                : new BalanceError(tx.Amount, totalDebit, totalCredit);
        }

        private async Task<Either<IFeatureError, SubmitTxCommand>> ValidateEntriesInfoAsync(SubmitTxCommand tx)
        {
            var errExchangeRates = new List<SubmitTxEntry>();
            var errAmounts = new List<SubmitTxEntry>();
            var errCurrencies = new List<SubmitTxEntry>();

            foreach (var entry in tx.Entries)
            {
                if (!ValidateExchangeRatesInfo(entry))
                    errExchangeRates.Add(entry);

                if (!ValidateEntryAmounts(entry))
                    errAmounts.Add(entry);

                if (!await ValidateCurrencyAsync(entry))
                    errCurrencies.Add(entry);
            }

            await Task.CompletedTask;

            if (errExchangeRates.Count == 0
                && errAmounts.Count == 0
                && errCurrencies.Count == 0)

                return tx;
            else
            {
                var dic = new Dictionary<EntryErrorType, List<SubmitTxEntry>>();
                if (errExchangeRates.Count > 0)
                    dic[EntryErrorType.ExchangeRate] = errExchangeRates;
                if (errAmounts.Count > 0)
                    dic[EntryErrorType.Amount] = errAmounts;
                if (errCurrencies.Count > 0)
                    dic[EntryErrorType.Currency] = errCurrencies;

                return new EntriesInfoErrors(dic);
            }
        }

        private static bool ValidateEntryAmounts(SubmitTxEntry entry)
        {
            return entry.Amount > 0
                  && (entry.AmountAdditionnalInfo == null || entry.AmountAdditionnalInfo.OriginalAmount > 0);
        }

        private static bool ValidateExchangeRatesInfo(SubmitTxEntry entry)
        {
            if (entry.AmountAdditionnalInfo != null)
            {
                if (entry.AmountAdditionnalInfo.ExchangeRate <= 0)
                    return false;

                var calculatedAmount = entry.AmountAdditionnalInfo.OriginalAmount * entry.AmountAdditionnalInfo.ExchangeRate;
                if (calculatedAmount != entry.Amount)
                    return false;
            }
            return true;
        }

        private async Task<bool> ValidateCurrencyAsync(SubmitTxEntry entry)
        {
            if (entry.AmountAdditionnalInfo != null)
            {
                var exist = await ctx.Currencies.FindAsync(entry.AmountAdditionnalInfo.OriginalCurrencyId);

                if (exist == null)
                    return false;
            }

            return true;
        }

        private async Task<Either<IFeatureError, SubmitTxCommand>> ValidateEntriesAccountsAsync(SubmitTxCommand tx)
        {
            //Check all the accounts at the same time
            var targetAccountIds = tx.Entries.Select(e => e.AccountId).Distinct().ToList();

            var results = await ctx.Accounts
                   .Where(a => targetAccountIds.Contains(a.Id))
                   .Select(a => a.Id)
                   .ToListAsync();

            var missingAccounts = targetAccountIds.Except(results.Select(r => r)).ToList();

            if (missingAccounts.Count == 0)
                return tx;
            else
            {
                var badEntries = tx.Entries.Where(e => missingAccounts.Contains(e.AccountId));
                return new AccountAreNotFoundErrors(badEntries);
            }
        }

        private async Task<Either<IFeatureError, SubmitTxCommand>> ValidateEntriesTaxRatesInfoAsync(SubmitTxCommand tx)
        {
            //All entries with tax info
            var taxRateEntries = tx.Entries
                .Where(e => e.TaxInfo != null)
                .Distinct()
                .ToList();

            //All tax rates used
            var foundTaxRates = await ctx.TaxRates
                .Where(tr => taxRateEntries.Select(t => t.TaxInfo!.TaxRateId).Contains(tr.Id))
                .ToListAsync();

            //Missing tax rates ids
            var missingTaxRateIds = taxRateEntries.Select(t => t.TaxInfo!.TaxRateId).Except(foundTaxRates.Select(f => f.Id)).ToList();

            //If missing tax rates
            if (missingTaxRateIds.Count > 0)
            {
                var badEntriesMissingRate = tx.Entries
                    .Where(e => e.TaxInfo != null && missingTaxRateIds.Contains(e.TaxInfo!.TaxRateId))
                    .ToList();
                return new TaxRatesNotFoundError(badEntriesMissingRate);
            }

            //If tax rates found not respect the configured rate
            var badEntriesTaxRateNotMatch = new List<SubmitTxEntry>();
            foreach (var entry in taxRateEntries)
            {
                var taxRate = foundTaxRates.First(tr => tr.Id == entry.TaxInfo!.TaxRateId);
                if (taxRate.Rate != entry.TaxInfo!.TaxAppliedRate)
                {
                    badEntriesTaxRateNotMatch.Add(entry);
                }
            }

            return badEntriesTaxRateNotMatch.Count > 0
                ? new TaxRatesNotMatchError(badEntriesTaxRateNotMatch)
                : tx;
        }
    }
}
