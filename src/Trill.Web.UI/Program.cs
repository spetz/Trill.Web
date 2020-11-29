using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Trill.Web.Core;
using Trill.Web.UI.Services;

namespace Trill.Web.UI
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddCore();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri("http://localhost:5000")});
            builder.Services.AddAntDesign();
            builder.Services.AddSingleton<IPusherService, PusherService>();
            builder.Services.AddScoped<IApiResponseHandler, ApiResponseHandler>();
            builder.Services.AddSingleton(services =>
            {
                var backendUrl = "http://localhost:5010";
                var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler
                {
                    // ServerCertificateCustomValidationCallback =
                    //     HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });

                return GrpcChannel.ForAddress(backendUrl, new GrpcChannelOptions {HttpHandler = httpHandler});
            });

            var host = builder.Build();
            var authenticationService = host.Services.GetRequiredService<IAuthenticationService>();
            await authenticationService.InitializeAsync();
            var pusherService = host.Services.GetRequiredService<IPusherService>();
            await Task.Run(() => pusherService.InitAsync());
            await host.RunAsync();
            await pusherService.CloseAsync();
        }
    }
}
