using System.Runtime.Serialization;
using MarketingBox.BackOffice.Service.Domain.Models;

namespace MarketingBox.BackOffice.Service.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}
