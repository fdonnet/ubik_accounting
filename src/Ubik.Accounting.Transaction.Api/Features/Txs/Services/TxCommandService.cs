using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Transaction.Api.Data;
using Ubik.Accounting.Transaction.Api.Features.Txs.Errors;
using Ubik.Accounting.Transaction.Contracts.Txs.Commands;
using Ubik.ApiService.Common.Errors;
using MassTransit;
using Ubik.Accounting.Transaction.Contracts.Txs.Events;
using Ubik.Accounting.Transaction.Api.Mappers;

namespace Ubik.Accounting.Transaction.Api.Features.Txs.Services
{
    public class TxCommandService(AccountingTxContext ctx
        , IPublishEndpoint publishEndpoint) : ITxCommandService
    {
        public async Task<Either<IFeatureError, TxSubmited>> SubmitTx(SubmitTxCommand command)
        {
            return await ValidateEntriesAccountsAsync(command)
                .BindAsync(ValidateEntriesAmountsAsync)
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
                return new AccountsInEntriesAreNotFoundError(badEntries);
            }
        }

        private async Task<Either<IFeatureError, SubmitTxCommand>> ValidateEntriesAmountsAsync(SubmitTxCommand tx)
        {
            var errEntries = new List<SubmitTxEntry>();
            foreach (var entry in tx.Entries)
            {
                if (entry.Amount <= 0)
                {
                    errEntries.Add(entry);
                }

                if (entry.AmountAdditionnalInfo != null)
                {
                    if (entry.AmountAdditionnalInfo.OriginalAmount <= 0)
                    {
                        errEntries.Add(entry);
                    }

                    //TODO: make a special case for that
                    var expectedAmount = entry.AmountAdditionnalInfo.OriginalAmount * entry.AmountAdditionnalInfo.ExchangeRate;
                    if (expectedAmount != entry.Amount)
                    {
                        errEntries.Add(entry);
                    }
                }
            }
            if(errEntries.Count > 0)
                return new EntriesAmountsError(errEntries);
            else
            {
                //Maybe we will need to be async... (very dirty check for now)
                await Task.CompletedTask;
                return tx;
            }
        }
    }
}
