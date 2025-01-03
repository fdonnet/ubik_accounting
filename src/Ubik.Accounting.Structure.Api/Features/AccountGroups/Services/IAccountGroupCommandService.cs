﻿using LanguageExt;
using Ubik.Accounting.Structure.Api.Models;
using Ubik.Accounting.Structure.Contracts.AccountGroups.Commands;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Structure.Api.Features.AccountGroups.Services
{
    public interface IAccountGroupCommandService
    {
        public Task<Either<IFeatureError, AccountGroup>> AddAsync(AddAccountGroupCommand command);
        public Task<Either<IFeatureError, AccountGroup>> UpdateAsync(UpdateAccountGroupCommand command);
        public Task<Either<IFeatureError, List<AccountGroup>>> DeleteAsync(Guid id);
    }
}
