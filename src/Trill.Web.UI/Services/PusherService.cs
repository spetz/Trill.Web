using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Trill.Pusher;
using Trill.Web.Core.Shared.DTO;

namespace Trill.Web.UI.Services
{
    internal class PusherService : IPusherService
    {
        private readonly GrpcChannel _channel;
        private readonly ILogger<PusherService> _logger;
        private readonly Notifier.NotifierClient _client;
        
        public event Action<StoryDto> StoryCreated;
        public event Action<ActionRejectedDto> ActionRejected;

        public PusherService(GrpcChannel channel, ILogger<PusherService> logger)
        {
            _channel = channel;
            _logger = logger;
            _client = new Notifier.NotifierClient(channel);
        }

        public Task InitAsync()
        {
            Task.Run(SubscribeStoriesAsync);
            Task.Run(SubscribeRejectedActionsAsync);

            return Task.CompletedTask;
        }
        
        public async Task CloseAsync()
        {
            _logger.LogInformation("Closing gRPC channel...");
            await _channel.ShutdownAsync();
        }

        private async Task SubscribeStoriesAsync()
        {
            var stream = _client.StreamStories(new SubscribeStories());
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var story = stream.ResponseStream.Current;
                StoryCreated?.Invoke(new StoryDto
                {
                    Id = story.Id,
                    Title = story.Title,
                    CreatedAt = DateTime.Parse(story.CreatedAt),
                    Author = new AuthorDto
                    {
                        Id = Guid.Parse(story.Author.Id),
                        Name = story.Author.Name
                    },
                    Tags = story.Tags
                });
            }
        }
        
        private async Task SubscribeRejectedActionsAsync()
        {
            var stream = _client.StreamRejectedActions(new SubscribeRejectedActions());
            while (await stream.ResponseStream.MoveNext(CancellationToken.None))
            {
                var actionRejected = stream.ResponseStream.Current;
                _logger.LogError(actionRejected.Code);
                _logger.LogError(actionRejected.Reason);
                ActionRejected?.Invoke(new ActionRejectedDto
                {
                    Reason = actionRejected.Reason,
                    Code = actionRejected.Code
                });
            }
        }
    }
}