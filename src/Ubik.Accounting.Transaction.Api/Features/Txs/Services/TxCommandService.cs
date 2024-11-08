﻿using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Transaction.Api.Data;
using Ubik.Accounting.Transaction.Api.Features.Txs.Errors;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;
using MassTransit;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.Accounting.Transaction.Api.Mappers;
using Ubik.Accounting.Transaction.Contracts.Entries.Enums;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Services
{
    public class TxCommandService(AccountingTxContext ctx
        , IPublishEndpoint publishEndpoint) : ITxCommandService
    {
        public async Task<Either<IFeatureError, TxSubmited>> SubmitTx(SubmitTxCommand command)
        {
            return await ValidateOnlyOneMainEntryAsync(command)
                .BindAsync(ValidateBalanceAsync)
                .BindAsync(ValidateEntriesInfoAsync) //Exchange rates, amounts and currencies
                .BindAsync(ValidateEntriesAccountsAsync)
                .BindAsync(PublishSubmittedAsync);
        }

        private async Task<Either<IFeatureError, TxSubmited>> PublishSubmittedAsync(SubmitTxCommand current)
        {
            //Publish that a tx has been submitted and checked for the ez validation
            var submited = current.ToTxSubmited();
            await publishEndpoint.Publish(submited, CancellationToken.None);
            await ctx.SaveChangesAsync();

            return submited;
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

                if (!ValidateEntriesAmounts(entry))
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

        private static bool ValidateEntriesAmounts(SubmitTxEntry entry)
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
    }
}
