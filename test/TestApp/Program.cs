using System;
using System.Linq;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.CampaignRows;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes;
using MarketingBox.Affiliate.Service.Grpc.Models.CampaignBoxes.Requests;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Requests;
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
            var client = factory.GetPartnerService();
            var boxClient = factory.GetCampaignService();
            var campaignBoxClient = factory.GetCampaignBoxService();

            //var boxes = await boxClient.SearchAsync(new CampaignSearchRequest()
            //{
            //    TenantId = testTenant
            //});

            //var partners = await client.SearchAsync(new PartnerSearchRequest()
            //{
            //    TenantId = testTenant,
            //});

            //var check = await client.GetAsync(new PartnerGetRequest()
            //{
            //    AffiliateId = 0,
            //});

            var boxId = 4;
            var activityHours = Enum.GetValues<DayOfWeek>().Select(x => new ActivityHours()
            {
                To = new TimeSpan(23, 59, 59),
                Day = x,
                From = new TimeSpan(0, 0, 0),
                IsActive = true
            }).ToArray();

            var campaignBox1= await campaignBoxClient.CreateAsync(new CampaignBoxCreateRequest()
            {
                Sequence = 0,
                ActivityHours = activityHours,
                BoxId = boxId,
                CampaignId = 3,
                CapType = CapType.Lead,
                CountryCode = "UK",
                DailyCapValue = 20,
                EnableTraffic = true,
                Information = null,
                Priority = 1,
                Weight = 10
            });

            var campaignBox2 = await campaignBoxClient.CreateAsync(new CampaignBoxCreateRequest()
            {
                Sequence = 0,
                ActivityHours = activityHours,
                BoxId = boxId,
                CampaignId = 4,
                CapType = CapType.Lead,
                CountryCode = "UK",
                DailyCapValue = 20,
                EnableTraffic = true,
                Information = null,
                Priority = 1,
                Weight = 5
            });

            var campaignBox3 = await campaignBoxClient.CreateAsync(new CampaignBoxCreateRequest()
            {
                Sequence = 0,
                ActivityHours = activityHours,
                BoxId = boxId,
                CampaignId = 5,
                CapType = CapType.Lead,
                CountryCode = "UK",
                DailyCapValue = 20,
                EnableTraffic = true,
                Information = null,
                Priority = 1,
                Weight = 5
            });


            var request = new PartnerCreateRequest()
            {
                TenantId = testTenant,
                Company = new PartnerCompany()
                {
                    Address = "a1",
                    Name = "a2",
                    RegNumber = "a3",
                    VatId = "a4"
                },
                Bank = new PartnerBank()
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
            request.GeneralInfo = new PartnerGeneralInfo()
            {
                Currency = Currency.CHF,
                Email = "email123@email.com",
                Password = "qwerty_123456",
                Phone = "+79990999999",
                Role = PartnerRole.IntegrationManager,
                Skype = "skype",
                State = PartnerState.Active,
                Username = "SomeTestUser1",
                ZipCode = "414141",
                ApiKey = "123-456-789",
                CreatedAt = DateTime.Now
            };

            var partnerCreated = (await  client.CreateAsync(request)).Partner;

            Console.WriteLine(partnerCreated.AffiliateId);

            var partnerUpdated = (await client.UpdateAsync(new PartnerUpdateRequest()
            {
                AffiliateId = partnerCreated.AffiliateId,
                TenantId = partnerCreated.TenantId,
                Bank = request.Bank,
                Company = request.Company,
                GeneralInfo = request.GeneralInfo,
                Sequence = 1
            })).Partner;

            await client.DeleteAsync(new PartnerDeleteRequest()
            {
                AffiliateId = partnerUpdated.AffiliateId,
            });

            var shouldBeNull =await client.GetAsync(new PartnerGetRequest()
            {
                AffiliateId = partnerUpdated.AffiliateId,
            });

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
