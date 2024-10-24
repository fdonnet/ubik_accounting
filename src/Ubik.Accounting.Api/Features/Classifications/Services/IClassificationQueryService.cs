﻿using LanguageExt;
using Ubik.Accounting.Api.Features.Classifications.CustomPoco;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Errors;

namespace Ubik.Accounting.Api.Features.Classifications.Services
{
    public interface IClassificationQueryService
    {
        public Task<IEnumerable<Classification>> GetAllAsync();
        public Task<Either<IServiceAndFeatureError, Classification>> GetAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, IEnumerable<Account>>> GetClassificationAttachedAccountsAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, IEnumerable<Account>>> GetClassificationMissingAccountsAsync(Guid id);
        public Task<Either<IServiceAndFeatureError, ClassificationStatus>> GetClassificationStatusAsync(Guid id);
    }
}
