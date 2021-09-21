using System;
using MarketingBox.Affiliate.Service.Domain.Common;
using MarketingBox.Affiliate.Service.Domain.Partners;

namespace MarketingBox.Affiliate.Postgres.Entities.Partners
{
    public class PartnerGeneralInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Skype { get; set; }
        public string ZipCode { get; set; }
        public PartnerRole Role { get; set; }
        public PartnerState State { get; set; }
        public Currency Currency { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string ApiKey { get; set; }
    }
}