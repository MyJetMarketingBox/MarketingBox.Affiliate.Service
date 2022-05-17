using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Requests.Affiliates;
using MarketingBox.Sdk.Common.Enums;
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
            var affiliateCreateRequestMaster = new AffiliateCreateRequest()
            {
                TenantId = testTenant,
                Company = new Company()
                {
                    Address = "a1",
                    Name = "a2",
                    RegNumber = "a3",
                    VatId = "a4"
                },
                Bank = new Bank()
                {
                    AccountNumber = "a1",
                    Address = "a1",
                    Name = "a1",
                    BeneficiaryAddress = "a1",
                    BeneficiaryName = "a1",
                    Iban = "a1",
                    Swift = "a1"
                }
            };

            affiliateCreateRequestMaster.GeneralInfo = new GeneralInfoRequest()
            {
                Currency = Currency.CHF,
                Email = "email1235678_1@email.com",
                Password = "qwerty_123456",
                Phone = "+79990999999",
                Skype = "skype",
                State = State.Active,
                Username = "SomeTestUser123X",
                ZipCode = "414141"
            };

            var affiliateCreateRequest = new AffiliateCreateRequest()
            {
                TenantId = testTenant,
                Company = new Company()
                {
                    Address = "a1",
                    Name = "a2",
                    RegNumber = "a3",
                    VatId = "a4"
                },
                Bank = new Bank()
                {
                    AccountNumber = "a1",
                    Address = "a1",
                    Name = "a1",
                    BeneficiaryAddress = "a1",
                    BeneficiaryName = "a1",
                    Iban = "a1",
                    Swift = "a1"
                }
            };

            affiliateCreateRequest.GeneralInfo = new GeneralInfoRequest()
            {
                Currency = Currency.CHF,
                Email = "email12356789_2@email.com",
                Password = "qwerty_123456",
                Phone = "+79990999999",
                Skype = "skype",
                State = State.Active,
                Username = "SomeTestUser12345X",
                ZipCode = "414141"
            };

            var masterAffiliate = (await affiliateService.CreateAsync(affiliateCreateRequestMaster)).Data;
            var affiliate = (await affiliateService.CreateAsync(affiliateCreateRequest)).Data;

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

            // const int boxId = 1;
            // List<ActivityHours> activityHours = Enum.GetValues<DayOfWeek>().Select(x => new ActivityHours()
            // {
            //     To = new TimeSpan(23, 59, 59),
            //     Day = x,
            //     From = new TimeSpan(0, 0, 0),
            //     IsActive = true
            // }).ToList();

            // var campaignBox1= await campaignBoxClient.CreateAsync(new CampaignRowCreateRequest()
            // {
            //     Sequence = 0,
            //     ActivityHours = activityHours,
            //     CampaignId = boxId,
            //     BrandId = 1,
            //     CapType = CapType.Lead,
            //     CountryCode = "UA",
            //     DailyCapValue = 1000,
            //     EnableTraffic = true,
            //     Information = null,
            //     Priority = 1,
            //     Weight = 5
            // });
            //
            // var campaignBox2 = await campaignBoxClient.CreateAsync(new CampaignRowCreateRequest()
            // {
            //     Sequence = 0,
            //     ActivityHours = activityHours,
            //     CampaignId = boxId,
            //     BrandId = 2,
            //     CapType = CapType.Lead,
            //     CountryCode = "UA",
            //     DailyCapValue = 1000,
            //     EnableTraffic = true,
            //     Information = null,
            //     Priority = 1,
            //     Weight = 2
            // });
            //
            // var campaignBox3 = await campaignBoxClient.CreateAsync(new CampaignRowCreateRequest()
            // {
            //     Sequence = 0,
            //     ActivityHours = activityHours,
            //     CampaignId = boxId,
            //     BrandId = 3,
            //     CapType = CapType.Lead,
            //     CountryCode = "UA",
            //     DailyCapValue = 1000,
            //     EnableTraffic = true,
            //     Information = null,
            //     Priority = 1,
            //     Weight = 1
            // });


            var request = new AffiliateCreateRequest()
            {
                TenantId = testTenant,
                Company = new Company()
                {
                    Address = "a1",
                    Name = "a2",
                    RegNumber = "a3",
                    VatId = "a4"
                },
                Bank = new Bank()
                {
                    AccountNumber = "a1",
                    Address = "a1",
                    Name = "a1",
                    BeneficiaryAddress = "a1",
                    BeneficiaryName = "a1",
                    Iban = "a1",
                    Swift = "a1"
                }
            };
            request.GeneralInfo = new GeneralInfoRequest()
            {
                Currency = Currency.CHF,
                Email = "email123@email.com",
                Password = "qwerty_123456",
                Phone = "+79990999999",
                Skype = "skype",
                State = State.Active,
                Username = "SomeTestUser1",
                ZipCode = "414141"
            };

            var partnerCreated = (await  affiliateService.CreateAsync(request)).Data;

            Console.WriteLine(partnerCreated.Id);

            var partnerUpdated = (await affiliateService.UpdateAsync(new AffiliateUpdateRequest()
            {
                AffiliateId = partnerCreated.Id,
                TenantId = partnerCreated.TenantId,
                Bank = request.Bank,
                Company = request.Company,
                GeneralInfo = request.GeneralInfo
            })).Data;

            //await affiliateService.DeleteAsync(new AffiliateDeleteRequest()
            //{
            //    AffiliateId = partnerUpdated.AffiliateId,
            //});

            var shouldBeNull = await affiliateService.GetAsync(new AffiliateByIdRequest()
            {
                AffiliateId = partnerUpdated.Id,
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
