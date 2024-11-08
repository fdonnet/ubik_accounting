namespace Ubik.Accounting.Transaction.Api.Models
{
    //EF core hack to have a nullable tax info owned entity but when it's present, fields are mandatory
    public class TaxInfo
    {
        private decimal? _taxAppliedRate;
        public decimal TaxAppliedRate
        {
            get => _taxAppliedRate ?? throw new NullReferenceException("Tax rate cannot be null");
            private set => _taxAppliedRate = value;
        }

        private Guid? _taxRateId;
        public Guid TaxRateId
        {
            get => _taxRateId ?? throw new NullReferenceException("Tax rate cannot be null");
            private set => _taxRateId = value;
        }

        public TaxInfo(decimal taxAppliedRate, Guid taxRateId)
        {
            TaxAppliedRate = taxAppliedRate;
            TaxRateId = taxRateId;
        }

        private TaxInfo()
        {
        }
    }
}
