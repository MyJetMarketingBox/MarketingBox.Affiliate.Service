using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Offers;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Grpc.Requests.Offers;
using MarketingBox.Affiliate.Service.MyNoSql.Offer;
using MarketingBox.Affiliate.Service.Repositories.Interfaces;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;

namespace MarketingBox.Affiliate.Service.Services
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IMyNoSqlServerDataWriter<OfferNoSql> _myNoSqlServerDataWriter;
        private readonly ILogger<OfferService> _logger;

        public OfferService(
            IOfferRepository offerRepository,
            ILogger<OfferService> logger,
            IMyNoSqlServerDataWriter<OfferNoSql> myNoSqlServerDataWriter)
        {
            _offerRepository = offerRepository;
            _logger = logger;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
        }

        public async Task<Response<Offer>> CreateAsync(OfferCreateRequest request)
        {
            try
            {
                request.ValidateEntity();

                var result = await _offerRepository.CreateAsync(request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(OfferNoSql.Create(result));
                return new Response<Offer>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<Offer>();
            }
        }

        public async Task<Response<Offer>> UpdateAsync(OfferUpdateRequest request)
        {
            try
            {
                request.ValidateEntity();

                var result = await _offerRepository.UpdateAsync(request);

                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(OfferNoSql.Create(result));
                return new Response<Offer>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<Offer>();
            }
        }

        public async Task<Response<Offer>> GetAsync(OfferRequestById request)
        {
            try
            {
                request.ValidateEntity();

                var result = await _offerRepository.GetAsync(request.Id.Value, request.AffiliateId.Value);
                return new Response<Offer>
                {
                    Status = ResponseStatus.Ok,
                    Data = result
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<Offer>();
            }
        }

        public async Task<Response<bool>> DeleteAsync(OfferRequestById request)
        {
            try
            {
                request.ValidateEntity();

                await _offerRepository.DeleteAsync(request.Id.Value, request.AffiliateId.Value);

                await _myNoSqlServerDataWriter.DeleteAsync(
                    OfferNoSql.GeneratePartitionKey(),
                    OfferNoSql.GenerateRowKey(request.Id.Value));
                return new Response<bool>
                {
                    Status = ResponseStatus.Ok,
                    Data = true
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<bool>();
            }
        }

        public async Task<Response<IReadOnlyCollection<Offer>>> SearchAsync(OfferSearchRequest request)
        {
            try
            {
                request.ValidateEntity();

                var (result, total) = await _offerRepository.SearchAsync(request);
                return new Response<IReadOnlyCollection<Offer>>
                {
                    Status = ResponseStatus.Ok,
                    Data = result,
                    Total = total
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<IReadOnlyCollection<Offer>>();
            }
        }

        public async Task<Response<string>> GetUrlAsync(GetUrlRequest request)
        {
            try
            {
                request.ValidateEntity();
                
                var result = await _offerRepository.GetUrlAsync(request.OfferId.Value, request.AffiliateId.Value);
                return new Response<string>()
                {
                    Data = result,
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception e)
            {
                return e.FailedResponse<string>();
            }
        }
    }
}