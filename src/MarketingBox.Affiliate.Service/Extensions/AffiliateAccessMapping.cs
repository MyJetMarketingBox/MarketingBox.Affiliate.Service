using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Postgres.Entities.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses;
using MarketingBox.Affiliate.Service.Messages.Affiliates;

namespace MarketingBox.Affiliate.Service.Extensions
{
    public static class AffiliateAccessMapping
    {
        public static AffiliateAccessResponse MapToGrpc(AffiliateAccessEntity affiliateAccessEntity)
        {
            return new AffiliateAccessResponse()
            {
                AffiliateAccess = MapToGrpcInner(affiliateAccessEntity)
            };
        }

        public static Grpc.Models.AffiliateAccesses.AffiliateAccess MapToGrpcInner(AffiliateAccessEntity affiliateAccessEntity)
        {
            return new Grpc.Models.AffiliateAccesses.AffiliateAccess()
            {
                AffiliateId = affiliateAccessEntity.AffiliateId,
                MasterAffiliateId = affiliateAccessEntity.MasterAffiliateId
            };
        }

        public static AffiliateAccessUpdated MapToMessage(AffiliateAccessEntity affiliateAccessEntity)
        {
            return new AffiliateAccessUpdated()
            {
                AffiliateId = affiliateAccessEntity.AffiliateId,
                MasterAffiliateId = affiliateAccessEntity.MasterAffiliateId
            };
        }
    }
}
