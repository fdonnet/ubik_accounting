namespace Ubik.Accounting.Api.Features.AccountGroupClassifications
{
    public interface IAccountGroupClassificationService
    {
        Task<bool> IfExistsAsync(Guid id);
    }
}
