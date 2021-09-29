using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Affiliate.Service.Grpc.Models.Boxes.Messages;
using MarketingBox.Affiliate.Service.Grpc.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Messages;
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
            var boxClient = factory.GetBoxService();

            var boxes = await boxClient.SearchAsync(new BoxSearchRequest()
            {
                TenantId = testTenant
            });

            var partners = await client.SearchAsync(new PartnerSearchRequest()
            {
                TenantId = testTenant,
            });

            var check = await client.GetAsync(new PartnerGetRequest()
            {
                AffiliateId = 0,
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
                Role = PartnerRole.BrandManager,
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
