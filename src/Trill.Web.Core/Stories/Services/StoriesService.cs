using System;
using System.Threading.Tasks;
using Trill.Web.Core.Services;
using Trill.Web.Core.Shared.DTO;
using Trill.Web.Core.Stories.Requests;

namespace Trill.Web.Core.Stories.Services
{
    internal class StoriesService : IStoriesService
    {
        private readonly IHttpClient _client;

        public StoriesService(IHttpClient client)
        {
            _client = client;
        }

        public Task<ApiResponse<PagedDto<StoryDto>>> BrowseAsync(string query, Guid? userId = null)
            => _client.GetAsync<PagedDto<StoryDto>>($"stories-service/stories?query={query}&userId={userId}");

        public Task<ApiResponse<StoryDetailsDto>> GetAsync(long storyId, Guid? userId = null)
            => _client.GetAsync<StoryDetailsDto>($"stories-service/stories/{storyId}?userId={userId}");

        public Task<ApiResponse> SendAsync(SendStoryRequest request)
            => _client.PostAsync("stories-service/stories", request);

        public Task<ApiResponse> SendUsingBrokerAsync(SendStoryRequest request)
            => _client.PostAsync("stories-service/stories/async", request);

        public Task<ApiResponse> RateAsync(long storyId, int rate)
            => _client.PostAsync($"stories-service/stories/{storyId}/rate", new RateStory
            {
                StoryId = storyId,
                Rate = rate
            });

        public Task<ApiResponse> RateUsingBrokerAsync(long storyId, int rate)
            => _client.PostAsync($"stories-service/stories/{storyId}/rate/async", new RateStory
            {
                StoryId = storyId,
                Rate = rate
            });
    }
}