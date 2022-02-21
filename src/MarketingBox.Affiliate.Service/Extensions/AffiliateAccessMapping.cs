using MarketingBox.Affiliate.Service.Domain.Models.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Messages.AffiliateAccesses;

namespace MarketingBox.Affiliate.Service.Extensions
{
    public static class AffiliateAccessMapping
    {
        public static AffiliateAccess MapToGrpcInner(AffiliateAccessEntity affiliateAccessEntity)
        {
            return new AffiliateAccess()
            {
                AffiliateId = affiliateAccessEntity.AffiliateId,
                MasterAffiliateId = affiliateAccessEntity.MasterAffiliateId,
                Id = affiliateAccessEntity.Id
            };
        }

        public static AffiliateAccessUpdated MapToMessage(AffiliateAccessEntity affiliateAccessEntity)
        {
            return new AffiliateAccessUpdated()
            {
                Id = affiliateAccessEntity.Id,
                AffiliateId = affiliateAccessEntity.AffiliateId,
                MasterAffiliateId = affiliateAccessEntity.MasterAffiliateId
            };
        }
    }
}
