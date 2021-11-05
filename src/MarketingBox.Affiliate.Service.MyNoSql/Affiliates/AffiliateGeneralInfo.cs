using System;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Common;

namespace MarketingBox.Affiliate.Service.MyNoSql.Affiliates
{
    public class AffiliateGeneralInfo
    {
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }
        public string Skype { get; set; }
        
        public string ZipCode { get; set; }
        
        public AffiliateRole Role { get; set; }
        
        public AffiliateState State { get; set; }
        
        public Currency Currency { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public string ApiKey { get; set; }
    }
}