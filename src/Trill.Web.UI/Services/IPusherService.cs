using System;
using System.Threading.Tasks;
using Trill.Web.Core.Shared.DTO;

namespace Trill.Web.UI.Services
{
    public interface IPusherService
    {
        Task InitAsync();
        Task CloseAsync();
        event Action<StoryDto> StoryCreated;
        event Action<ActionRejectedDto> ActionRejected;
    }
}