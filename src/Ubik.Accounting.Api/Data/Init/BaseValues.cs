namespace Ubik.Accounting.Api.Data.Init
{
    public record BaseValuesForAccounts
    {
        public Guid AccountId1 { get; } = Guid.Parse("248e0000-5dd4-0015-ebad-08dcd98b0949");
        public Guid AccountId2 { get; } = Guid.Parse("248e0000-5dd4-0015-f110-08dcd98b20af");
        public Guid AccountId3 { get; } = Guid.Parse("dc610000-5dd4-0015-83b1-08dcda5647f2");
        public Guid AccountIdForDel { get; } = Guid.Parse("ec860000-5dd4-0015-3815-08dcda1dad9e");
        public Guid AccountForAttach { get; } = Guid.Parse("14320000-5dd4-0015-8c30-08dcdb1c487d");
        public Guid AccountForAttach2 { get; } = Guid.Parse("14320000-5dd4-0015-e9ab-08dcdb1cd8fc");
        public string AccountCode1 { get; } = "1000";
    }

    public record BaseValuesForAccountGroups
    {
        public Guid AccountGroupId1 { get; } = Guid.Parse("cc100000-5dd4-0015-ca0c-08dcd967caca");
        public Guid AccountGroupIdFirstLvl1 { get; } = Guid.Parse("cc100000-5dd4-0015-78d2-08dcd9666d91");
        public Guid AccountGroupId2 { get; } = Guid.Parse("b0650000-5dd4-0015-30cd-08dcda65fa89");
        public Guid AccountGroupIdForDel { get; } = Guid.Parse("b0650000-5dd4-0015-db5d-08dcda5b664b");
        public Guid AccountGroupIdForDel2 { get; } = Guid.Parse("b0650000-5dd4-0015-3edb-08dcda5bf6b7");
        public Guid AccountGroupIdForDelWithClass { get; } = Guid.Parse("34980000-5dd4-0015-9ae6-08dcdaf2ae72");
        public Guid AccountGroupForAttach { get; } = Guid.Parse("4c470000-5dd4-0015-ddd1-08dcdb1e7283");
        public Guid AccountGroupForAttach2 { get; } = Guid.Parse("4c470000-5dd4-0015-4d88-08dcdb1e7a54");


        public string AccountGroupCode1 { get; } = "100";
    }

    public record BaseValuesForClassifications
    {
        public Guid ClassificationId1 { get; } = Guid.Parse("cc100000-5dd4-0015-d910-08dcd9665e79");
        public Guid ClassificationId2 { get; } = Guid.Parse("4c470000-5dd4-0015-f70f-08dcdb1e6d00");
        public Guid ClassificationId3 { get; } = Guid.Parse("1524f189-20dd-4888-88f8-428e59bbcddd");
        public Guid ClassificationIdForDel { get; } = Guid.Parse("1524f190-20dd-4888-88f8-428e59bbc22c");
        public string ClassificationCode1 { get; } = "SWISSPLAN-FULL";
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
