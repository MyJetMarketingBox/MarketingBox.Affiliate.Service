using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests
{
    [DataContract]
    public class CreateSubRequest
    {
        [DataMember(Order = 1)] public string MasterAffiliateId { get; set; }
        [DataMember(Order = 2)] public string MasterAffiliateApiKey { get; set; }
        [DataMember(Order = 3)] public string Username { get; set; }
        [DataMember(Order = 4)] public string Password { get; set; }
        [DataMember(Order = 5)] public string Email { get; set; }
        [DataMember(Order = 6)] public string LandingUrl { get; set; }
        [DataMember(Order = 7)] public SubEntity[] Sub { get; set; }
    }
    
    [DataContract]
    public class SubEntity
    {
        [DataMember(Order = 1)] public string SubName { get; set; }
        [DataMember(Order = 2)] public string SubValue { get; set; }
    }
}