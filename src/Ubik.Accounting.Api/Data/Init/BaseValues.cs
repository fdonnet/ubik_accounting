namespace Ubik.Accounting.Api.Data.Init
{
    public record BaseValuesForAccounts
    {
        public Guid AccountId1 { get; } = Guid.Parse("10070000-5d1a-0015-91f9-08dbfb1b9879");
        public Guid AccountId2 { get; } = Guid.Parse("10070000-5d1a-0015-b866-08dbfb1baa3b");
        public Guid AccountId3 { get; } = Guid.Parse("10070000-5d1a-0015-885e-08dbfb1bb84d");
        public Guid AccountIdForDel { get; } = Guid.Parse("10070000-5d1a-0015-70f2-08dbfb1bd313");
        public string AccountCode1 { get; } = "1000";
    }

    public record BaseValuesForAccountGroups
    {
        public Guid AccountGroupId1 { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22a");
        public Guid AccountGroupIdFirstLvl1 { get; } = Guid.Parse("1529991f-20dd-4888-88f8-428e59bbc22a");
        public Guid AccountGroupId2 { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22b");
        public Guid AccountGroupIdForDel { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22c");
        public Guid AccountGroupIdForDel2 { get; } = Guid.Parse("1624f11f-20dd-4888-88f8-428e59bbc22e");
        public Guid AccountGroupIdForDelWithClass { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbcddd");
        public string AccountGroupCode1 { get; } = "102";
    }

    public record BaseValuesForClassifications
    {
        public Guid ClassificationId1 { get; } = Guid.Parse("1524f188-20dd-4888-88f8-428e59bbc22a");
        public Guid ClassificationId2 { get; } = Guid.Parse("1524f189-20dd-4888-88f8-428e59bbc22a");
        public Guid ClassificationId3 { get; } = Guid.Parse("1524f189-20dd-4888-88f8-428e59bbcddd");
        public Guid ClassificationIdForDel { get; } = Guid.Parse("1524f190-20dd-4888-88f8-428e59bbc22c");
        public string ClassificationCode1 { get; } = "SWISSPLAN";
    }

    public record BaseValuesForTenants
    {
        public Guid TenantId { get; } = Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d");
    }

    public record BaseValuesForUsers
    {
        public Guid UserId1 { get; } = Guid.Parse("c9fe1b29-6d1b-420c-ac64-fc8f1a6153af");
    }

    public record BaseValuesGeneral
    {
        public DateTime GenerationTime { get; }  = DateTime.UtcNow;
    }

    public record BaseValuesForCurrencies
    {
        public Guid CurrencyId1 { get; } = Guid.Parse("248e0000-5dd4-0015-38c5-08dcd98e5b2d");
        public Guid CurrencyId2 { get; } = Guid.Parse("248e0000-5dd4-0015-291d-08dcd98e55f8");
    }
}
