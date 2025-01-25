using Microsoft.Azure.Relay;
using System.Net;

namespace pos_backend.Services.Impl
{
    public class RelayHostedService : BackgroundService
    {
        private readonly string _relayNamespace;
        private readonly string _connectionName;
        private readonly string _keyName;
        private readonly string _key;

        private HybridConnectionListener? _listener;

        public RelayHostedService(IConfiguration configuration)
        {
            var configurationSection = configuration.GetRequiredSection("AzureRelay");
            _relayNamespace = configurationSection.GetSection("RelayNamespace").Value!;
            _connectionName = configurationSection.GetSection("ConnectionName").Value!;
            _keyName = configurationSection.GetSection("KeyName").Value!;
            _key = configurationSection.GetSection("Key").Value!;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_keyName, _key);
            _listener = new HybridConnectionListener(new Uri($"sb://{_relayNamespace}/{_connectionName}"), tokenProvider);

            _listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
            _listener.Offline += (o, e) => { Console.WriteLine("Offline"); };
            _listener.Online += (o, e) => { Console.WriteLine("Online"); };

            _listener.RequestHandler = async (context) =>
            {
                try
                {
                    var endpointUrl = context.Request.Url.AbsolutePath;
                    var api = endpointUrl.Replace("/hybridos/", "");
                    var authHeader = context.Request.Headers["Authorization"];

                    var proxyRequest = new HttpRequestMessage
                    {
                        Method = new HttpMethod(context.Request.HttpMethod),
                        RequestUri = new Uri($"http://pos-backend:5299/{api}"),
                    };

                    foreach (string headerName in context.Request.Headers)
                    {
                        if (!string.Equals(headerName, "Host", StringComparison.OrdinalIgnoreCase))
                            proxyRequest.Headers.TryAddWithoutValidation(headerName, context.Request.Headers[headerName]);
                    }

                    if (context.Request.HttpMethod != "GET" && context.Request.HttpMethod != "HEAD")
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await context.Request.InputStream.CopyToAsync(memoryStream);
                            memoryStream.Position = 0;
                            proxyRequest.Content = new StreamContent(memoryStream);

                            foreach (string headerName in context.Request.Headers)
                            {
                                if (headerName.StartsWith("Content-", StringComparison.OrdinalIgnoreCase))
                                    proxyRequest.Content.Headers.TryAddWithoutValidation(headerName, context.Request.Headers[headerName]);
                            }
                        }
                    }

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.SendAsync(proxyRequest);

                        context.Response.StatusCode = response.StatusCode;

                        foreach (var header in response.Headers)
                        {
                            context.Response.Headers[header.Key] = string.Join(",", header.Value);
                        }

                        foreach (var header in response.Content.Headers)
                        {
                            context.Response.Headers[header.Key] = string.Join(",", header.Value);
                        }

                        var responseBody = await response.Content.ReadAsStreamAsync();
                        await responseBody.CopyToAsync(context.Response.OutputStream);
                    }
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = HttpStatusCode.InternalServerError;
                    using (var writer = new StreamWriter(context.Response.OutputStream))
                    {
                        await writer.WriteAsync($"Error: {ex.Message}");
                    }
                }
                finally
                {
                    await context.Response.CloseAsync();
                }
            };


            await _listener.OpenAsync();
            Console.WriteLine("Server listening");

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            if (_listener != null)
            {
                await _listener.CloseAsync();
                Console.WriteLine("Listener closed");
            }

            await base.StopAsync(stoppingToken);
        }
    }

}
