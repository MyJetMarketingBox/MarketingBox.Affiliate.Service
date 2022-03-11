﻿using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Domain.Extensions;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Auth.Service.Grpc;
using MarketingBox.Auth.Service.Grpc.Models.Users.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MarketingBox.Affiliate.Service.Domain.Models.Affiliates;
using MarketingBox.Affiliate.Service.Domain.Models.Common;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates;
using MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.Requests;
using MarketingBox.Affiliate.Service.Messages.Affiliates;
using MarketingBox.Affiliate.Service.MyNoSql.Affiliates;
using MarketingBox.Auth.Service.Domain.Models.Users;
using MarketingBox.Sdk.Common.Exceptions;
using MarketingBox.Sdk.Common.Extensions;
using MarketingBox.Sdk.Common.Models.Grpc;
using MyJetWallet.Sdk.ServiceBus;
using AffiliateBank = MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.AffiliateBank;
using AffiliateCompany = MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.AffiliateCompany;
using AffiliateGeneralInfo = MarketingBox.Affiliate.Service.Grpc.Models.Affiliates.AffiliateGeneralInfo;
using AffiliateRole = MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliateRole;
using AffiliateState = MarketingBox.Affiliate.Service.Domain.Models.Affiliates.AffiliateState;

namespace MarketingBox.Affiliate.Service.Services
{
    public class AffiliateService : IAffiliateService
    {
        private readonly ILogger<AffiliateService> _logger;
        private readonly DatabaseContextFactory _databaseContextFactory;
        private readonly IServiceBusPublisher<AffiliateUpdated> _publisherPartnerUpdated;
        private readonly IMyNoSqlServerDataWriter<AffiliateNoSql> _myNoSqlServerDataWriter;
        private readonly IUserService _userService;

        private static AffiliateEntity GetAffiliateEntity(AffiliateCreateRequest request)
        {
            var affiliateEntity = new AffiliateEntity()
            {
                TenantId = request.TenantId,
                BankAccountNumber = request.Bank?.AccountNumber,
                BankAddress = request.Bank?.BankAddress,
                BankName = request.Bank?.BankName,
                BankBeneficiaryAddress = request.Bank?.BeneficiaryAddress,
                BankBeneficiaryName = request.Bank?.BeneficiaryName,
                BankIban = request.Bank?.Iban,
                BankSwift = request.Bank?.Swift,
                CompanyAddress = request.Company?.Address,
                CompanyName = request.Company?.Name,
                CompanyRegNumber = request.Company?.RegNumber,
                CompanyVatId = request.Company?.VatId,
                CreatedAt = DateTime.UtcNow,
                GeneralInfoCurrency = request.GeneralInfo.Currency.MapEnum<Currency>(),
                GeneralInfoRole = request.GeneralInfo.Role.MapEnum<AffiliateRole>(),
                GeneralInfoSkype = request.GeneralInfo.Skype,
                GeneralInfoState = request.GeneralInfo.State.MapEnum<AffiliateState>(),
                GeneralInfoUsername = request.GeneralInfo.Username,
                GeneralInfoZipCode = request.GeneralInfo.ZipCode,
                GeneralInfoEmail = request.GeneralInfo.Email,
                GeneralInfoPassword = request.GeneralInfo.Password,
                GeneralInfoPhone = request.GeneralInfo.Phone,
                GeneralInfoApiKey = request.GeneralInfo.ApiKey,
                AccessIsGivenById = request.MasterAffiliateId ?? 0,
                LandingUrl = request.LandingUrl
            };
            return affiliateEntity;
        }

        private async Task PushEvent(AffiliateEntity affiliateEntity, AffiliateUpdatedEventType eventType)
        {
            await _publisherPartnerUpdated.PublishAsync(MapToMessage(affiliateEntity, eventType));
        }
        
        private async Task CreateOrUpdateUser(string tenantId, AffiliateEntity affiliateEntity)
        {
            var existingUsers = await _userService.GetAsync(new GetUserRequest()
            {
                ExternalUserId = affiliateEntity.AffiliateId.ToString(),
                TenantId = tenantId
            });

            if (existingUsers.Error != null)
                throw new InvalidOperationException($"{existingUsers.Error.Message} - {existingUsers.Error.ErrorType}");

            if (existingUsers.User == null ||
                !existingUsers.User.Any())
            {
                var response = await _userService.CreateAsync(new CreateUserRequest()
                {
                    Email = affiliateEntity.GeneralInfoEmail,
                    ExternalUserId = affiliateEntity.AffiliateId.ToString(),
                    Password = affiliateEntity.GeneralInfoPassword,
                    TenantId = affiliateEntity.TenantId,
                    Username = affiliateEntity.GeneralInfoUsername,
                    Role = affiliateEntity.GeneralInfoRole switch
                    {
                        AffiliateRole.Affiliate => UserRole.Affiliate,
                        AffiliateRole.AffiliateManager => UserRole.AffiliateManager,
                        AffiliateRole.IntegrationManager => UserRole.Admin,
                        AffiliateRole.MasterAffiliate => UserRole.MasterAffiliate,
                        AffiliateRole.MasterAffiliateReferral => UserRole.MasterAffiliateReferral,
                        _ => throw new ArgumentOutOfRangeException(nameof(affiliateEntity.GeneralInfoRole), affiliateEntity.GeneralInfoRole, null)
                    }
                });

                if (response.Error != null)
                    throw new InvalidOperationException($"{response.Error.Message} - {response.Error.ErrorType}");
            }
            else
            {
                var response = await _userService.UpdateAsync(new UpdateUserRequest()
                {
                    Email = affiliateEntity.GeneralInfoEmail,
                    ExternalUserId = affiliateEntity.AffiliateId.ToString(),
                    Password = affiliateEntity.GeneralInfoPassword,
                    TenantId = affiliateEntity.TenantId,
                    Username = affiliateEntity.GeneralInfoUsername,
                    Role = affiliateEntity.GeneralInfoRole switch
                    {
                        AffiliateRole.Affiliate => UserRole.Affiliate,
                        AffiliateRole.AffiliateManager => UserRole.AffiliateManager,
                        AffiliateRole.IntegrationManager => UserRole.Admin,
                        AffiliateRole.MasterAffiliate => UserRole.MasterAffiliate,
                        AffiliateRole.MasterAffiliateReferral => UserRole.MasterAffiliateReferral,
                        _ => throw new ArgumentOutOfRangeException(nameof(affiliateEntity.GeneralInfoRole), affiliateEntity.GeneralInfoRole, null)
                    }
                });

                if (response.Error != null)
                    throw new InvalidOperationException($"{response.Error.Message} - {response.Error.ErrorType}");
            }
        }

        private static Grpc.Models.Affiliates.Affiliate MapToGrpcInner(AffiliateEntity affiliateEntity)
        {
            return new Grpc.Models.Affiliates.Affiliate()
            {
                TenantId = affiliateEntity.TenantId,
                AffiliateId = affiliateEntity.AffiliateId,
                Company = new AffiliateCompany()
                {
                    Address = affiliateEntity.CompanyAddress,
                    Name = affiliateEntity.CompanyName,
                    RegNumber = affiliateEntity.CompanyRegNumber,
                    VatId = affiliateEntity.CompanyVatId,
                },
                Bank = new AffiliateBank()
                {
                    AccountNumber = affiliateEntity.BankAccountNumber,
                    BankAddress = affiliateEntity.BankAddress,
                    BankName = affiliateEntity.BankName,
                    BeneficiaryAddress = affiliateEntity.BankBeneficiaryAddress,
                    BeneficiaryName = affiliateEntity.BankBeneficiaryName,
                    Iban = affiliateEntity.BankIban,
                    Swift = affiliateEntity.BankSwift
                },
                GeneralInfo = new AffiliateGeneralInfo()
                {
                    Currency = affiliateEntity.GeneralInfoCurrency.MapEnum<Domain.Models.Common.Currency>(),
                    CreatedAt = affiliateEntity.CreatedAt.UtcDateTime,
                    Email = affiliateEntity.GeneralInfoEmail,
                    Password = affiliateEntity.GeneralInfoPassword,
                    Phone = affiliateEntity.GeneralInfoPhone,
                    Role = affiliateEntity.GeneralInfoRole.MapEnum<Domain.Models.Affiliates.AffiliateRole>(),
                    Skype = affiliateEntity.GeneralInfoSkype,
                    State = affiliateEntity.GeneralInfoState.MapEnum<Domain.Models.Affiliates.AffiliateState>(),
                    Username = affiliateEntity.GeneralInfoUsername,
                    ZipCode = affiliateEntity.GeneralInfoZipCode,
                    ApiKey = affiliateEntity.GeneralInfoApiKey
                },
                Sequence = affiliateEntity.Sequence,
                AccessIsGivenById = affiliateEntity.AccessIsGivenById,
                LandingUrl = affiliateEntity.LandingUrl
            };
        }

        private static AffiliateUpdated MapToMessage(AffiliateEntity affiliateEntity, AffiliateUpdatedEventType type)
        {
            return new AffiliateUpdated()
            {
                TenantId = affiliateEntity.TenantId,
                AffiliateId = affiliateEntity.AffiliateId,
                Company = new Messages.Affiliates.AffiliateCompany()
                {
                    Address = affiliateEntity.CompanyAddress,
                    Name = affiliateEntity.CompanyName,
                    RegNumber = affiliateEntity.CompanyRegNumber,
                    VatId = affiliateEntity.CompanyVatId,
                },
                Bank = new Messages.Affiliates.AffiliateBank()
                {
                    AccountNumber = affiliateEntity.BankAccountNumber,
                    BankAddress = affiliateEntity.BankAddress,
                    BankName = affiliateEntity.BankName,
                    BeneficiaryAddress = affiliateEntity.BankBeneficiaryAddress,
                    BeneficiaryName = affiliateEntity.BankBeneficiaryName,
                    Iban = affiliateEntity.BankIban,
                    Swift = affiliateEntity.BankSwift
                },
                GeneralInfo = new Messages.Affiliates.AffiliateGeneralInfo()
                {
                    Currency = affiliateEntity.GeneralInfoCurrency.MapEnum<Domain.Models.Common.Currency>(),
                    CreatedAt = affiliateEntity.CreatedAt.UtcDateTime,
                    Email = affiliateEntity.GeneralInfoEmail,
                    //Password = affiliateEntity.GeneralInfo.Password,
                    Phone = affiliateEntity.GeneralInfoPhone,
                    Role = affiliateEntity.GeneralInfoRole.MapEnum<Domain.Models.Affiliates.AffiliateRole>(),
                    Skype = affiliateEntity.GeneralInfoSkype,
                    State = affiliateEntity.GeneralInfoState.MapEnum<Domain.Models.Affiliates.AffiliateState>(),
                    Username = affiliateEntity.GeneralInfoUsername,
                    ZipCode = affiliateEntity.GeneralInfoZipCode,
                    ApiKey = affiliateEntity.GeneralInfoApiKey
                },
                EventType = type
            };
        }

        private static AffiliateNoSql MapToNoSql(AffiliateEntity affiliateEntity)
        {
            return AffiliateNoSql.Create(
                affiliateEntity.TenantId,
                affiliateEntity.AffiliateId,
                new MyNoSql.Affiliates.AffiliateGeneralInfo()
                {
                    Currency = affiliateEntity.GeneralInfoCurrency.MapEnum<Domain.Models.Common.Currency>(),
                    CreatedAt = affiliateEntity.CreatedAt.UtcDateTime,
                    Email = affiliateEntity.GeneralInfoEmail,
                    //Password = affiliateEntity.GeneralInfo.Password,
                    Phone = affiliateEntity.GeneralInfoPhone,
                    Role = affiliateEntity.GeneralInfoRole.MapEnum<Domain.Models.Affiliates.AffiliateRole>(),
                    Skype = affiliateEntity.GeneralInfoSkype,
                    State = affiliateEntity.GeneralInfoState.MapEnum<Domain.Models.Affiliates.AffiliateState>(),
                    Username = affiliateEntity.GeneralInfoUsername,
                    ZipCode = affiliateEntity.GeneralInfoZipCode,
                    ApiKey = affiliateEntity.GeneralInfoApiKey
                },
                new MyNoSql.Affiliates.AffiliateCompany()
                {
                    Address = affiliateEntity.CompanyAddress,
                    Name = affiliateEntity.CompanyName,
                    RegNumber = affiliateEntity.CompanyRegNumber,
                    VatId = affiliateEntity.CompanyVatId,
                },
                new MyNoSql.Affiliates.AffiliateBank()
                {
                    AccountNumber = affiliateEntity.BankAccountNumber,
                    BankAddress = affiliateEntity.BankAddress,
                    BankName = affiliateEntity.BankName,
                    BeneficiaryAddress = affiliateEntity.BankBeneficiaryAddress,
                    BeneficiaryName = affiliateEntity.BankBeneficiaryName,
                    Iban = affiliateEntity.BankIban,
                    Swift = affiliateEntity.BankSwift
                },
                affiliateEntity.Sequence);
        }
        
        public AffiliateService(ILogger<AffiliateService> logger,
            IServiceBusPublisher<AffiliateUpdated> publisherPartnerUpdated,
            IMyNoSqlServerDataWriter<AffiliateNoSql> myNoSqlServerDataWriter,
            IUserService userService,
            DatabaseContextFactory databaseContextFactory)
        {
            _logger = logger;
            _publisherPartnerUpdated = publisherPartnerUpdated;
            _myNoSqlServerDataWriter = myNoSqlServerDataWriter;
            _userService = userService;
            _databaseContextFactory = databaseContextFactory;
        }
        
        public static string GeneratePassword(int size = 16)
        {       
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray(); 
            var data = new byte[4*size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            var result = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }
            return result.ToString();
        }
        
        public async Task<Response<bool>> SetAffiliateStateAsync(SetAffiliateStateRequest request)
        {
            _logger.LogInformation("SetAffiliateStateAsync {@context}", request);
            try
            {
                await using var ctx = _databaseContextFactory.Create();
                var affiliate = ctx.Affiliates.FirstOrDefault(e => e.AffiliateId == request.AffiliateId);

                if (affiliate == null)
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                var newState = request.State.MapEnum<AffiliateState>();
                affiliate.GeneralInfoState = newState;
                
                _logger.LogInformation($"SetAffiliateStateAsync change affiliate({request.AffiliateId}) state to {newState}");
                await ctx.SaveChangesAsync();

                return new Response<bool>
                {
                    Status = ResponseStatus.Ok
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ex.FailedResponse<bool>();
            }
        }

        public async Task<Response<Grpc.Models.Affiliates.Affiliate>> CreateSubAsync(CreateSubRequest request)
        {
            _logger.LogInformation("Creating new Sub Affiliate {@context}", request);
            try
            {
                await using var ctx = _databaseContextFactory.Create();
                var masterAffiliate = await ctx.Affiliates
                    .FirstOrDefaultAsync(e => e.AffiliateId == request.MasterAffiliateId);

                if (masterAffiliate == null ||
                    !masterAffiliate.GeneralInfoApiKey.Equals(request.MasterAffiliateApiKey))
                {
                    throw new UnauthorizedException("Incorrect master affiliate credentials.");
                }
                if (masterAffiliate.GeneralInfoRole != AffiliateRole.MasterAffiliate &&
                    masterAffiliate.GeneralInfoRole != AffiliateRole.MasterAffiliateReferral)
                {
                    throw new UnauthorizedException("Not enough rights to complete the transaction.");
                }

                if (string.IsNullOrWhiteSpace(request.Password))
                    request.Password = GeneratePassword();
                
                var createRequest = new AffiliateCreateRequest()
                {
                    GeneralInfo = new AffiliateGeneralInfo()
                    {
                        CreatedAt = DateTime.UtcNow,
                        Email = request.Email,
                        Password = request.Password,
                        Username = request.Username,
                        Role = AffiliateRole.Affiliate,
                        State = AffiliateState.NotActive,
                        ApiKey = Guid.NewGuid().ToString("N")
                    },
                    MasterAffiliateId = masterAffiliate.AffiliateId,
                    TenantId = masterAffiliate.TenantId,
                    LandingUrl = request.LandingUrl,
                    IsSubAffiliate = true
                };
                var createResponse = await CreateAsync(createRequest);
                
                if (createResponse.Status != ResponseStatus.Ok)
                    return createResponse;
                
                if (createResponse?.Data != null &&
                    request.Sub != null &&
                    request.Sub.Any())
                {
                    await ctx.AddNewAffiliateSubParam(request.Sub.Select(e => new AffiliateSubParamEntity()
                    {
                        AffiliateId = createResponse.Data.AffiliateId,
                        ParamName = e.SubName,
                        ParamValue = e.SubValue
                    }));
                }
                return createResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ex.FailedResponse<Grpc.Models.Affiliates.Affiliate>();
            }
        }

        public async Task<Response<Grpc.Models.Affiliates.Affiliate>> CreateAsync(AffiliateCreateRequest request)
        {
            _logger.LogInformation("Creating new Affiliate {@context}", request);
            await using var ctx = _databaseContextFactory.Create();

            var affiliateEntity = GetAffiliateEntity(request);
            try
            {
                var existingEntity = await ctx.Affiliates
                    .FirstOrDefaultAsync(x => x.TenantId == request.TenantId &&
                                              (x.GeneralInfoEmail == request.GeneralInfo.Email ||
                                               x.GeneralInfoUsername == request.GeneralInfo.Username));
                if (existingEntity != null)
                {
                    throw new AlreadyExistsException("Affiliate already exists.");
                }
                ctx.Affiliates.Add(affiliateEntity);
                await ctx.SaveChangesAsync();

                await CreateOrUpdateUser(request.TenantId, affiliateEntity);
                
                var nosql = MapToNoSql(affiliateEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);
                
                // TODO: change logic
                await PushEvent(affiliateEntity, request.IsSubAffiliate ? AffiliateUpdatedEventType.CreatedSub : AffiliateUpdatedEventType.CreatedManual);
                
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return new Response<Grpc.Models.Affiliates.Affiliate>()
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(affiliateEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating partner {@context}", request);
                return e.FailedResponse<Grpc.Models.Affiliates.Affiliate>();
            }
        }

        public async Task<Response<Grpc.Models.Affiliates.Affiliate>> UpdateAsync(AffiliateUpdateRequest request)
        {
            _logger.LogInformation("Updating a Affiliate {@context}", request);
            await using var ctx = _databaseContextFactory.Create();

            var affiliateEntity = new AffiliateEntity()
            {
                AffiliateId = request.AffiliateId,
                TenantId = request.TenantId,
                BankAccountNumber = request.Bank.AccountNumber,
                BankAddress = request.Bank.BankAddress,
                BankName = request.Bank.BankName,
                BankBeneficiaryAddress = request.Bank.BeneficiaryAddress,
                BankBeneficiaryName = request.Bank.BeneficiaryName,
                BankIban = request.Bank.Iban,
                BankSwift = request.Bank.Swift,
                CompanyAddress = request.Company.Address,
                CompanyName = request.Company.Name,
                CompanyRegNumber = request.Company.RegNumber,
                CompanyVatId = request.Company.VatId,
                CreatedAt = DateTime.UtcNow,
                GeneralInfoCurrency = request.GeneralInfo.Currency.MapEnum<Currency>(),
                GeneralInfoRole = request.GeneralInfo.Role.MapEnum<AffiliateRole>(),
                GeneralInfoSkype = request.GeneralInfo.Skype,
                GeneralInfoState = request.GeneralInfo.State.MapEnum<AffiliateState>(),
                GeneralInfoUsername = request.GeneralInfo.Username,
                GeneralInfoZipCode = request.GeneralInfo.ZipCode,
                GeneralInfoEmail = request.GeneralInfo.Email,
                GeneralInfoPassword = request.GeneralInfo.Password,
                GeneralInfoPhone = request.GeneralInfo.Phone,
                GeneralInfoApiKey = request.GeneralInfo.ApiKey,
                Sequence = request.Sequence + 1,
                AccessIsGivenById = request.MasterAffiliateId ?? 0
            };
            try
            {
                var affectedRows = ctx.Affiliates
                    .Where(x => x.AffiliateId == affiliateEntity.AffiliateId &&
                                x.Sequence < affiliateEntity.Sequence)
                    .ToList();

                if (affectedRows.Any())
                {
                    foreach (var affectedRow in affectedRows)
                    {
                        affectedRow.TenantId = affiliateEntity.TenantId;
                        affectedRow.BankAccountNumber = affiliateEntity.BankAccountNumber;
                        affectedRow.BankAddress = affiliateEntity.BankAddress;
                        affectedRow.BankName = affiliateEntity.BankName;
                        affectedRow.BankBeneficiaryAddress = affiliateEntity.BankBeneficiaryAddress;
                        affectedRow.BankBeneficiaryName = affiliateEntity.BankBeneficiaryName;
                        affectedRow.BankIban = affiliateEntity.BankIban;
                        affectedRow.BankSwift = affiliateEntity.BankSwift;
                        affectedRow.CompanyAddress = affiliateEntity.CompanyAddress;
                        affectedRow.CompanyName = affiliateEntity.CompanyName;
                        affectedRow.CompanyRegNumber = affiliateEntity.CompanyRegNumber;
                        affectedRow.CompanyVatId = affiliateEntity.CompanyVatId;
                        affectedRow.CreatedAt = affiliateEntity.CreatedAt;
                        affectedRow.GeneralInfoCurrency = affiliateEntity.GeneralInfoCurrency;
                        affectedRow.GeneralInfoRole = affiliateEntity.GeneralInfoRole;
                        affectedRow.GeneralInfoSkype = affiliateEntity.GeneralInfoSkype;
                        affectedRow.GeneralInfoState = affiliateEntity.GeneralInfoState;
                        affectedRow.GeneralInfoUsername = affiliateEntity.GeneralInfoUsername;
                        affectedRow.GeneralInfoZipCode = affiliateEntity.GeneralInfoZipCode;
                        affectedRow.GeneralInfoEmail = affiliateEntity.GeneralInfoEmail;
                        affectedRow.GeneralInfoPassword = affiliateEntity.GeneralInfoPassword;
                        affectedRow.GeneralInfoPhone = affiliateEntity.GeneralInfoPhone;
                        affectedRow.GeneralInfoApiKey = affiliateEntity.GeneralInfoApiKey;
                        affectedRow.Sequence = affiliateEntity.Sequence;
                        affectedRow.AccessIsGivenById = affiliateEntity.AccessIsGivenById;
                    }
                }
                else
                {
                    await ctx.Affiliates.AddAsync(affiliateEntity);
                }
                await ctx.SaveChangesAsync();

                await CreateOrUpdateUser(request.TenantId, affiliateEntity);

                var nosql = MapToNoSql(affiliateEntity);
                await _myNoSqlServerDataWriter.InsertOrReplaceAsync(nosql);
                _logger.LogInformation("Sent partner update to MyNoSql {@context}", request);

                await _publisherPartnerUpdated.PublishAsync(MapToMessage(affiliateEntity, AffiliateUpdatedEventType.Updated));
                _logger.LogInformation("Sent partner update to service bus {@context}", request);

                return new Response<Grpc.Models.Affiliates.Affiliate>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(affiliateEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating partner {@context}", request);

                return e.FailedResponse<Grpc.Models.Affiliates.Affiliate>();
            }
        }

        public async Task<Response<Grpc.Models.Affiliates.Affiliate>> GetAsync(AffiliateGetRequest request)
        {
            await using var ctx = _databaseContextFactory.Create();
            try
            {
                var affiliateEntity = await ctx.Affiliates
                    .FirstOrDefaultAsync(x => x.AffiliateId == request.AffiliateId);
                if (affiliateEntity is null)
                {
                    throw new NotFoundException(nameof(request.AffiliateId), request.AffiliateId);
                }

                return new Response<Grpc.Models.Affiliates.Affiliate>
                {
                    Status = ResponseStatus.Ok,
                    Data = MapToGrpcInner(affiliateEntity)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting partner {@context}", request);

                return e.FailedResponse<Grpc.Models.Affiliates.Affiliate>();
            }
        }

        public async Task<Response<IReadOnlyCollection<AffiliateSubParam>>> GetSubParamsAsync(GetSubParamsRequest request)
        {
            await using var ctx = _databaseContextFactory.Create();
            try
            {
                var paramList = ctx.AffiliateSubParamCollection
                    .Where(x => x.AffiliateId == request.AffiliateId)
                    .ToList();

                return new Response<IReadOnlyCollection<AffiliateSubParam>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = paramList.Select(e => new AffiliateSubParam()
                    {
                        ParamName = e.ParamName,
                        ParamValue = e.ParamValue
                    }).ToList()
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting params for affiliateId =  {affiliateId}", request.AffiliateId);
                return e.FailedResponse<IReadOnlyCollection<AffiliateSubParam>>();
            }
        }

        public async Task<Response<IReadOnlyCollection<Grpc.Models.Affiliates.Affiliate>>>  SearchAsync(AffiliateSearchRequest request)
        {
            await using var ctx = _databaseContextFactory.Create();

            try
            {
                IQueryable<AffiliateEntity> query = ctx.Affiliates;

                if (request.MasterAffiliateId.HasValue)
                {
                    query = query.Where(e => e.AccessIsGivenById == request.MasterAffiliateId);
                }

                if (!string.IsNullOrEmpty(request.TenantId))
                {
                    query = query.Where(x => x.TenantId == request.TenantId);
                }

                if (!string.IsNullOrEmpty(request.Username))
                {
                    query = query.Where(x => x.GeneralInfoUsername.Contains(request.Username));
                }

                if (request.AffiliateId.HasValue)
                {
                    query = query.Where(x => x.AffiliateId == request.AffiliateId.Value);
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    query = query.Where(x => x.GeneralInfoEmail.Contains(request.Email));
                }

                if (request.CreatedAt != default)
                {
                    DateTimeOffset date = request.CreatedAt;
                    query = query.Where(x => x.CreatedAt == date);
                }

                if (request.Role.HasValue)
                {
                    query = query.Where(x => x.GeneralInfoRole == request.Role.MapEnum<AffiliateRole>());
                }

                var limit = request.Take <= 0 ? 1000 : request.Take;
                if (request.Asc)
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.AffiliateId > request.Cursor);
                    }

                    query = query.OrderBy(x => x.AffiliateId);
                }
                else
                {
                    if (request.Cursor != null)
                    {
                        query = query.Where(x => x.AffiliateId < request.Cursor);
                    }

                    query = query.OrderByDescending(x => x.AffiliateId);
                }

                query = query.Take(limit);

                await query.LoadAsync();

                var response = query
                    .AsEnumerable()
                    .Select(MapToGrpcInner)
                    .ToArray();

                return new Response<IReadOnlyCollection<Grpc.Models.Affiliates.Affiliate>>()
                {
                    Status = ResponseStatus.Ok,
                    Data = response
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error search request {@context}", request);

                return e.FailedResponse<IReadOnlyCollection<Grpc.Models.Affiliates.Affiliate>>();
            }
        }

    }
}
