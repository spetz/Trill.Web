using System;
using System.Threading.Tasks;
using Trill.Web.Core.Ads.DTO;
using Trill.Web.Core.Ads.Requests;
using Trill.Web.Core.Services;

namespace Trill.Web.Core.Ads.Services
{
    internal class AdsService : IAdsService
    {
        private readonly IHttpClient _client;

        public AdsService(IHttpClient client)
        {
            _client = client;
        }

        public Task<ApiResponse<PagedDto<AdDto>>> BrowseAsync(Guid? userId = null)
            => _client.GetAsync<PagedDto<AdDto>>($"ads-service/ads?userId={userId}");

        public Task<ApiResponse<AdDetailsDto>> GetAsync(Guid adId)
            => _client.GetAsync<AdDetailsDto>($"ads-service/ads/{adId}");

        public Task<ApiResponse> CreateAsync(CreateAdRequest request)
            => _client.PostAsync("ads-service/ads", request);

        public Task<ApiResponse> ApproveAsync(Guid adId)
            => _client.PutAsync($"ads-service/ads/{adId}/approve", new {adId});

        public Task<ApiResponse> RejectAsync(Guid adId)
            => _client.PutAsync($"ads-service/ads/{adId}/reject", new {adId});
    }
}