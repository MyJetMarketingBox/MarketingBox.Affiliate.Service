using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Messages.Integrations
{
    [DataContract]
    public class IntegrationRemoved
    {
        [DataMember(Order = 1)]
        public long IntegrationId { get; set; }

        [DataMember(Order = 2)]
        public string TenantId { get; set; }

        [DataMember(Order = 3)]
        public long Sequence { get; set; }

    }
}
