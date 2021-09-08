using System;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Client;
using MarketingBox.Affiliate.Service.Grpc.Models;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners;
using MarketingBox.Affiliate.Service.Grpc.Models.Partners.Messages;
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


            var factory = new AffiliateServiceClientFactory("http://localhost:12347");
            var client = factory.GetPartnerService();

            var resp = await  client.CreateAsync(new PartnerCreateRequest()
            {
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
                },
                GeneralInfo = new PartnerGeneralInfo()
                {
                    Currency = Currency.CHF,
                    Email = "email@email.com",
                    Password = "sadadadwad",
                    Phone = "+79990999999",
                    Role = PartnerRole.BrandManager,
                    Skype = "skype",
                    State = PartnerState.Active,
                    Username = "User",
                    ZipCode = "414141"
                }
            });
            Console.WriteLine(resp?.AffiliateId);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
