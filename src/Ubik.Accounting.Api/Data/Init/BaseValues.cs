namespace Ubik.Accounting.Api.Data.Init
{
    public record BaseValuesForAccounts
    {
        public Guid AccountId1 { get; } = Guid.Parse("7777f11f-20dd-4888-88f8-428e59bbc537");
        public Guid AccountId2 { get; } = Guid.Parse("9524f11f-20dd-4888-88f8-428e59bbc229");
        public Guid AccountId3 { get; } = Guid.Parse("9524f11f-20dd-4888-88f8-428e59bbc230");
        public Guid AccountIdForDel { get; } = Guid.Parse("9524f11f-20dd-4888-88f8-428e59bbc300");
        public string AccountCode1 { get; } = "1020";
    }

    public record BaseValuesForAccountGroups
    {
        public Guid AccountGroupId1 { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22a");
        public Guid AccountGroupIdFirstLvl1 { get; } = Guid.Parse("1529991f-20dd-4888-88f8-428e59bbc22a");
        public Guid AccountGroupId2 { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22b");
        public Guid AccountGroupIdForDel { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22c");
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
        public Guid CurrencyId1 { get; } = Guid.Parse("ccfe1b29-6d1b-420c-ac64-fc8f1a6153a1");
        public Guid CurrencyId2 { get; } = Guid.Parse("cc1e1b29-6d1b-420c-ac64-fc8f1a6153a2");
    }
}
