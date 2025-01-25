using Microsoft.Azure.Relay;
using System.Net;

const string RelayNamespace = "hybridos-pos.servicebus.windows.net";
const string ConnectionName = "hybridos";
const string KeyName = "Listener";
const string Key = "ZtRyCXfeGPAerrzx4a8mWKo4GzAHHVP4r+ARmEthGK8=";

static async Task RunAsync()
{
    var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(KeyName, Key);
    var listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", RelayNamespace, ConnectionName)), tokenProvider);

    listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
    listener.Offline += (o, e) => { Console.WriteLine("Offline"); };
    listener.Online += (o, e) => { Console.WriteLine("Online"); };

    listener.RequestHandler = async (context) =>
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


    await listener.OpenAsync();
    Console.WriteLine("Server listening");


    await Console.In.ReadLineAsync();

    await listener.CloseAsync();
}

RunAsync().GetAwaiter().GetResult();