namespace Ubik.Accounting.Transaction.Api.Models
{
    public class AmountAdditionalInfo
    {
        private decimal? _originalAmount;
        public decimal OriginalAmount
        {
            get => _originalAmount ?? throw new NullReferenceException("Original amount cannot be null");
            private set => _originalAmount = value;
        }

        private Guid? _originalCurrencyId;
        public Guid OriginalCurrencyId
        {
            get => _originalCurrencyId ?? throw new NullReferenceException("Original amount cannot be null");
            private set => _originalCurrencyId = value;
        }

        private decimal? _exchangeRate;
        public decimal ExchangeRate
        {
            get => _exchangeRate ?? throw new NullReferenceException("Echange rate cannot be null");
            private set => _exchangeRate = value;
        }

        public AmountAdditionalInfo(decimal originalAmount, Guid originalCurrencyId, decimal exchangeRate)
        {
            _originalAmount = originalAmount;
            _originalCurrencyId = originalCurrencyId;
            _exchangeRate = exchangeRate;
        }

        private AmountAdditionalInfo()
        {
        }
    }
}
