using System;
using System.Threading.Tasks;
using Trill.Web.Core.Services;
using Trill.Web.Core.Shared.DTO;
using Trill.Web.Core.Users.DTO;
using Trill.Web.Core.Users.Requests;

namespace Trill.Web.Core.Users.Services
{
    internal class UsersService : IUsersService
    {
        private readonly IHttpClient _client;

        public UsersService(IHttpClient client)
        {
            _client = client;
        }

        public Task<ApiResponse<UserDetailsDto>> GetAsync(Guid userId)
            => _client.GetAsync<UserDetailsDto>($"users-service/users/{userId}");

        public Task<ApiResponse> RegisterAsync(RegisterRequest request)
            => _client.PostAsync("users-service/sign-up", request);

        public Task<ApiResponse<AuthDto>> LoginAsync(Login request)
            => _client.PostAsync<AuthDto>("users-service/sign-in", request);

        public Task<ApiResponse<PagedDto<UserDto>>> BrowseUsersAsync(Guid? userId = null)
            => _client.GetAsync<PagedDto<UserDto>>($"users-service/users?userId={userId}");

        public Task<ApiResponse> FollowAsync(Guid followerId, Guid followeeId)
            => _client.PostAsync($"users-service/users/{followerId}/following/{followeeId}", new FollowUser
            {
                UserId = followerId,
                FolloweeId = followeeId
            });

        public Task<ApiResponse> UnfollowAsync(Guid followerId, Guid followeeId)
            => _client.DeleteAsync($"users-service/users/{followerId}/following/{followeeId}");
    }
}