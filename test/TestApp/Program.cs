﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.AffiliateAccesses.Requests;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignRows.Requests;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using ProtoBuf.Grpc.Client;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();
            var testTenant = "default-tenant-id";
            var factory = new AffiliateServiceClientFactory("http://localhost:12347");
            var affiliateService = factory.GetAffiliateService();
            var affiliateAccessService = factory.GetAffiliateAccessService();
            var boxClient = factory.GetCampaignService();
            var campaignBoxClient = factory.GetCampaignRowService();

            var affiliateCreateRequestMaster = new AffiliateCreateRequest()
            {
                TenantId = testTenant,
                Company = new AffiliateCompany()
                {
                    Address = "a1",
                    Name = "a2",
                    RegNumber = "a3",
                    VatId = "a4"
                },
                Bank = new AffiliateBank()
                {
                    AccountNumber = "a1",
                    BankAddress = "a1",
                    BankName = "a1",
                    BeneficiaryAddress = "a1",
                    BeneficiaryName = "a1",
                    Iban = "a1",
                    Swift = "a1"
                }
            };

            affiliateCreateRequestMaster.GeneralInfo = new AffiliateGeneralInfo()
            {
                Currency = Currency.CHF,
                Email = "email1235678_1@email.com",
                Password = "qwerty_123456",
                Phone = "+79990999999",
                Role = AffiliateRole.MasterAffiliate,
                Skype = "skype",
                State = AffiliateState.Active,
                Username = "SomeTestUser123X",
                ZipCode = "414141",
                ApiKey = "123-456-789",
                CreatedAt = DateTime.Now
            };

            var affiliateCreateRequest = new AffiliateCreateRequest()
            {
                TenantId = testTenant,
                Company = new AffiliateCompany()
                {
                    Address = "a1",
                    Name = "a2",
                    RegNumber = "a3",
                    VatId = "a4"
                },
                Bank = new AffiliateBank()
                {
                    AccountNumber = "a1",
                    BankAddress = "a1",
                    BankName = "a1",
                    BeneficiaryAddress = "a1",
                    BeneficiaryName = "a1",
                    Iban = "a1",
                    Swift = "a1"
                }
            };

            affiliateCreateRequest.GeneralInfo = new AffiliateGeneralInfo()
            {
                Currency = Currency.CHF,
                Email = "email12356789_2@email.com",
                Password = "qwerty_123456",
                Phone = "+79990999999",
                Role = AffiliateRole.Affiliate,
                Skype = "skype",
                State = AffiliateState.Active,
                Username = "SomeTestUser12345X",
                ZipCode = "414141",
                ApiKey = "123-456-789",
                CreatedAt = DateTime.Now
            };

            var masterAffiliate = (await affiliateService.CreateAsync(affiliateCreateRequestMaster)).Affiliate;
            var affiliate = (await affiliateService.CreateAsync(affiliateCreateRequest)).Affiliate;

            //var access = await affiliateAccessService.CreateAsync(new AffiliateAccessCreateRequest()
            //{
            //    TenantId = testTenant,
            //    AffiliateId = affiliate.AffiliateId,
            //    MasterAffiliateId = masterAffiliate.AffiliateId
            //});

            var search = await affiliateService.SearchAsync(new AffiliateSearchRequest()
            {
                TenantId = testTenant,
                MasterAffiliateId = 9
            });

            //var boxes = await boxClient.SearchAsync(new CampaignSearchRequest()
            //{
            //    TenantId = testTenant
            //});

            //var partners = await client.SearchAsync(new AffiliateSearchRequest()
            //{
            //    TenantId = testTenant,
            //});

            //var check = await client.GetAsync(new AffiliateGetRequest()
            //{
            //    AffiliateId = 0,
            //});

            var boxId = 1;
            var activityHours = Enum.GetValues<DayOfWeek>().Select(x => new ActivityHours()
            {
                To = new TimeSpan(23, 59, 59),
                Day = x,
                From = new TimeSpan(0, 0, 0),
                IsActive = true
            }).ToArray();

            var campaignBox1= await campaignBoxClient.CreateAsync(new CampaignRowCreateRequest()
            {
                Sequence = 0,
                ActivityHours = activityHours,
                CampaignId = boxId,
                BrandId = 1,
                CapType = CapType.Lead,
                CountryCode = "UA",
                DailyCapValue = 1000,
                EnableTraffic = true,
                Information = null,
                Priority = 1,
                Weight = 5
            });

            var campaignBox2 = await campaignBoxClient.CreateAsync(new CampaignRowCreateRequest()
            {
                Sequence = 0,
                ActivityHours = activityHours,
                CampaignId = boxId,
                BrandId = 2,
                CapType = CapType.Lead,
                CountryCode = "UA",
                DailyCapValue = 1000,
                EnableTraffic = true,
                Information = null,
                Priority = 1,
                Weight = 2
            });

            var campaignBox3 = await campaignBoxClient.CreateAsync(new CampaignRowCreateRequest()
            {
                Sequence = 0,
                ActivityHours = activityHours,
                CampaignId = boxId,
                BrandId = 3,
                CapType = CapType.Lead,
                CountryCode = "UA",
                DailyCapValue = 1000,
                EnableTraffic = true,
                Information = null,
                Priority = 1,
                Weight = 1
            });


            var request = new AffiliateCreateRequest()
            {
                TenantId = testTenant,
                Company = new AffiliateCompany()
                {
                    Address = "a1",
                    Name = "a2",
                    RegNumber = "a3",
                    VatId = "a4"
                },
                Bank = new AffiliateBank()
                {
                    AccountNumber = "a1",
                    BankAddress = "a1",
                    BankName = "a1",
                    BeneficiaryAddress = "a1",
                    BeneficiaryName = "a1",
                    Iban = "a1",
                    Swift = "a1"
                }
            };
            request.GeneralInfo = new AffiliateGeneralInfo()
            {
                Currency = Currency.CHF,
                Email = "email123@email.com",
                Password = "qwerty_123456",
                Phone = "+79990999999",
                Role = AffiliateRole.MasterAffiliate,
                Skype = "skype",
                State = AffiliateState.Active,
                Username = "SomeTestUser1",
                ZipCode = "414141",
                ApiKey = "123-456-789",
                CreatedAt = DateTime.Now
            };

            var partnerCreated = (await  affiliateService.CreateAsync(request)).Affiliate;

            Console.WriteLine(partnerCreated.AffiliateId);

            var partnerUpdated = (await affiliateService.UpdateAsync(new AffiliateUpdateRequest()
            {
                AffiliateId = partnerCreated.AffiliateId,
                TenantId = partnerCreated.TenantId,
                Bank = request.Bank,
                Company = request.Company,
                GeneralInfo = request.GeneralInfo,
                Sequence = 1
            })).Affiliate;

            await affiliateService.DeleteAsync(new AffiliateDeleteRequest()
            {
                AffiliateId = partnerUpdated.AffiliateId,
            });

            var shouldBeNull =await affiliateService.GetAsync(new AffiliateGetRequest()
            {
                AffiliateId = partnerUpdated.AffiliateId,
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
