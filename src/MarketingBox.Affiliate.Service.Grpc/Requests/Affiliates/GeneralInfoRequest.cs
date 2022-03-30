using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;

[DataContract]
public class GeneralInfoRequest : ValidatableEntity
{
    [DataMember(Order = 1), Required, StringLength(128,MinimumLength = 1)] public string Username { get; set; }

    [DataMember(Order = 2), Required, StringLength(128,MinimumLength = 1)]
    [LogMasked(PreserveLength = false)]
    public string Password { get; set; }

    [DataMember(Order = 3), Required, EmailAddress, StringLength(128,MinimumLength = 1)]
    [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
    public string Email { get; set; }

    [DataMember(Order = 4), Required, Phone, StringLength(20,MinimumLength = 7)]
    [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
    public string Phone { get; set; }

    [DataMember(Order = 5)]
    [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
    public string Skype { get; set; }

    [DataMember(Order = 6)]
    [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
    public string ZipCode { get; set; }

    [DataMember(Order = 7)] public State State { get; set; }

    [DataMember(Order = 8)] public Currency Currency { get; set; }
}