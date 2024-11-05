namespace Ubik.Accounting.Transaction.Api.Models
{
    public class TaxInfo
    {
        private decimal? _taxAppliedRate { get;  set; }
        public decimal TaxAppliedRate
        {
            get => _taxAppliedRate ?? 0;
            set => _taxAppliedRate = value;
        }

        private Guid? _taxRateId;
        public Guid TaxRateId { get; set; }

       
    }
}
}
