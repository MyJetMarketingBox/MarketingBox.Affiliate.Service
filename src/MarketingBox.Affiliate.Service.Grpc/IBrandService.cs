﻿using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands;
using MarketingBox.Affiliate.Service.Grpc.Models.Brands.Requests;
using MyJetWallet.Sdk.Common.Models;

namespace MarketingBox.Affiliate.Service.Grpc
{
    [ServiceContract]
    public interface IBrandService
    {
        [OperationContract]
        Task<Response<Brand>> CreateAsync(BrandCreateRequest request);

        [OperationContract]
        Task<Response<Brand>> UpdateAsync(BrandUpdateRequest request);

        [OperationContract]
        Task<Response<Brand>> GetAsync(BrandGetRequest request);

        [OperationContract]
        Task<Response<bool>> DeleteAsync(BrandDeleteRequest request);

        [OperationContract]
        Task<Response<IReadOnlyCollection<Brand>>> SearchAsync(BrandSearchRequest request);
    }
}