using Ubik.Accounting.Transaction.Contracts.Entries.Enums;

namespace Ubik.Accounting.Transaction.Api.Models
{
    //Ef core hack for owned type
    public class EntryLink
    {
        private LinkType? _linkType;

        public LinkType LinkType
        {
            get => _linkType ?? throw new NullReferenceException("Link type cannot be null");
            private set => _linkType = value;
        }

        private Guid? _entryId;

        public Guid EntryId
        {
            get => _entryId ?? throw new NullReferenceException("Entry Id cannot be null");
            private set => _entryId = value;
        }

        public EntryLink(LinkType linkType, Guid entryId)
        {
            _linkType = linkType;
            _entryId = entryId;
        }

        private EntryLink()
        {
        }
    }
}
