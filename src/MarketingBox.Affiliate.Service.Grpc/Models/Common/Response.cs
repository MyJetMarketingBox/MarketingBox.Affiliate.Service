using System.Runtime.Serialization;

namespace MarketingBox.Affiliate.Service.Grpc.Models.Common;

[DataContract]
public class Response<T>
{
    [DataMember(Order = 1)] public T Data { get; set; }
    [DataMember(Order = 2)] public Error Error { get; set; }
}