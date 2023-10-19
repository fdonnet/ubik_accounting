namespace Ubik.Accounting.Api.Data.Init
{
    public record BaseValuesForAccounts
    {
        public Guid AccountId1 { get; } = Guid.Parse("7777f11f-20dd-4888-88f8-428e59bbc537");
        public Guid AccountId2 { get; } = Guid.Parse("9524f11f-20dd-4888-88f8-428e59bbc229");
        public Guid AccountIdForDel { get; } = Guid.Parse("9524f11f-20dd-4888-88f8-428e59bbc300");
        public string AccountCode1 { get; } = "1020";
    }

    public record BaseValuesForAccountGroups
    {
        public Guid AccountGroupId1 { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22a");
        public Guid AccountGroupIdFirstLvl1 { get; } = Guid.Parse("1529991f-20dd-4888-88f8-428e59bbc22a");
        public Guid AccountGroupId2 { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22b");
        public Guid AccountGroupIdForDel { get; } = Guid.Parse("1524f11f-20dd-4888-88f8-428e59bbc22c");
        public string AccountGroupCode1 { get; } = "102";
    }

    public record BaseValuesForAccountGroupClassifications
    {
        public Guid AccountGroupClassificationId1 { get; } = Guid.Parse("1524f188-20dd-4888-88f8-428e59bbc22a");
        public Guid AccountGroupClassificationId2 { get; } = Guid.Parse("1524f189-20dd-4888-88f8-428e59bbc22a");
        public Guid AccountGroupClassificationIdForDel { get; } = Guid.Parse("1524f190-20dd-4888-88f8-428e59bbc22c");
        public string AccountGroupClassificationCode1 { get; } = "SWISSPLAN";
    }

    public record BaseValuesForTenants
    {
        public Guid TenantId { get; } = Guid.Parse("727449e8-e93c-49e6-a5e5-1bf145d3e62d");
    }

    public record BaseValuesForUsers
    {
        public Guid UserId1 { get; } = Guid.Parse("9124f11f-20dd-4888-88f8-428e59bbc53e");
    }

    public record BaseValuesGeneral
    {
        public DateTime GenerationTime { get; }  = DateTime.UtcNow;
    }
}
