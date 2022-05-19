using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Destructurama.Attributed;
using MarketingBox.Affiliate.Service.Domain.Models.OfferAffiliates;
using MarketingBox.Sdk.Common.Enums;

namespace MarketingBox.Affiliate.Service.Domain.Models.Affiliates
{
    [DataContract]
    public class Affiliate
    {
        [DataMember(Order = 1)] 
        public string TenantId { get; set; }
        
        [DataMember(Order = 2)] 
        public long Id { get; set; }
        
        [DataMember(Order = 3)]
        public string Username { get; set; }

        [DataMember(Order = 4)] [LogMasked(PreserveLength = false)]
        public string Password { get; set; }

        [DataMember(Order = 5)] [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
        public string Email { get; set; }

        [DataMember(Order = 6)] [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
        public string Phone { get; set; }

        [DataMember(Order = 7)] [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
        public string Skype { get; set; }

        [DataMember(Order = 8)] [LogMasked(PreserveLength = true, ShowFirst = 1, ShowLast = 1)]
        public string ZipCode { get; set; }

        [DataMember(Order = 9)] public State State { get; set; }
        
        [DataMember(Order = 10)] public Currency Currency { get; set; }
        
        [DataMember(Order = 11)]
        public string ApiKey { get; set; }
        
        [DataMember(Order = 12)]
        public DateTime CreatedAt { get; set; }
        
        [DataMember(Order = 14)]
        public Company Company { get; set; }
        
        [DataMember(Order = 16)]
        public Bank Bank { get; set; }
        
        [DataMember(Order = 17)]
        public long? CreatedBy { get; set; }

        [DataMember(Order = 18)]
        public List<AffiliatePayout> Payouts { get; set; } = new ();

        [DataMember(Order = 19)]
        public List<OfferAffiliate> OfferAffiliates { get; set; } = new ();
        
        public AffiliateMessage MapToMessage()
        {
            var bank = Bank is null
                ? null
                : new Bank()
                {
                    Address = Bank.Address,
                    Iban = Bank.Iban,
                    Name = Bank.Name,
                    Swift = Bank.Swift,
                    AccountNumber = Bank.AccountNumber,
                    BeneficiaryAddress = Bank.BeneficiaryAddress,
                    BeneficiaryName = Bank.BeneficiaryName,
                };
            var company = Company is null
                ? null
                : new Company()
                {
                    Address = Company.Address,
                    Name = Company.Name,
                    RegNumber = Company.RegNumber,
                    VatId = Company.VatId
                };
            return new AffiliateMessage
            {
                AffiliateId = Id,
                TenantId = TenantId,
                Bank = bank,
                Company = company,
                GeneralInfo = new GeneralInfo
                {
                    Currency = Currency,
                    Email = Email,
                    Phone = Phone,
                    Skype = Skype,
                    State = State,
                    Username = Username,
                    ApiKey = ApiKey,
                    CreatedAt = CreatedAt,
                    ZipCode = ZipCode
                },
            };
        }
    }
}
