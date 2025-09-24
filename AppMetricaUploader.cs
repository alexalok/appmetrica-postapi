using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AppMetrica.PostAPI;

public interface IAppMetricaUploader
{
    Task UploadEvent(AppMetricaEvent @event);
}

public class AppMetricaUploader : IAppMetricaUploader
{
    readonly HttpClient _httpClient;
    private readonly ILogger<AppMetricaUploader> _logger;
    readonly AppMetricaOptions _options;

    public AppMetricaUploader(IOptions<AppMetricaOptions> options, HttpClient? httpClient = null, ILogger<AppMetricaUploader>? logger = null)
    {
        _options = options.Value;
        _httpClient = httpClient ?? new HttpClient() { BaseAddress = AppMetricaOptions.BaseUrl };
        _logger = logger ?? NullLogger<AppMetricaUploader>.Instance;
    }

    public async Task UploadEvent(AppMetricaEvent @event)
    {
        var uriSb = new StringBuilder();
        uriSb.Append("?post_api_key=" + _options.PostApiKey);
        uriSb.Append("&application_id=" + _options.ApplicationId);
        uriSb.Append("&event_name=" + @event.EventName);

        if (@event.ProfileId != null)
            uriSb.Append("&profile_id=" + @event.ProfileId);

        if (@event.AppMetricaDeviceId != null)
            uriSb.Append("&appmetrica_device_id=" + @event.AppMetricaDeviceId);

        long timestamp = @event.EventTime.ToUnixTimeSeconds();
        uriSb.Append("&event_timestamp=" + timestamp);

        if (@event.EventJson != null)
        {
            var json = @event.EventJson.ToJsonString();
            var encodedJson = Uri.EscapeDataString(json);
            uriSb.Append("&event_json=" + encodedJson);
        }

        var reportUrl = uriSb.ToString();

        if (_logger.IsEnabled(LogLevel.Trace))
            _logger.LogTrace("Reporting AppMetrica event: {ReportUrl}", reportUrl);

        var req = new HttpRequestMessage(HttpMethod.Post, reportUrl);

        var resp = await _httpClient.SendAsync(req);
        if (resp.StatusCode != HttpStatusCode.OK)
            throw new AppMetricaUploadException(resp.StatusCode);

        string respContent = await resp.Content.ReadAsStringAsync();
        if (respContent != "Your data has been uploaded.")
            throw new AppMetricaUploadException($"Unexpected answer from AppMetrica: '{respContent}'.");
    }
}
