using Microsoft.EntityFrameworkCore;
using Ubik.Accounting.Api.Data;
using Ubik.Accounting.Api.Features.AccountGroups.Exceptions;
using Ubik.Accounting.Api.Features.Classifications.Exceptions;
using Ubik.Accounting.Api.Models;
using Ubik.ApiService.Common.Exceptions;

namespace Ubik.Accounting.Api.Features.Classifications
{
    public class ClassificationService : IClassificationService
    {
        private readonly AccountingContext _context;
        public ClassificationService(AccountingContext ctx)
        {
            _context = ctx;

        }

        public async Task<bool> IfExistsAsync(Guid id)
        {
            return await _context.AccountGroupClassifications.AnyAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Classification>> GetAllAsync()
        {
            var result = await _context.AccountGroupClassifications.ToListAsync();

            return result;
        }

        public async Task<ResultT<Classification>> GetAsync(Guid id)
        {
            var result = await _context.AccountGroupClassifications.FirstOrDefaultAsync(a => a.Id == id);

            return result == null
                ? new ResultT<Classification>() { IsSuccess = false, Exception = new ClassificationNotFoundException(id) }
                : new ResultT<Classification>() { IsSuccess = true, Result = result };
        }
    }
}
