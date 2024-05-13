using System.Net;
using System.Text;
using Microsoft.Extensions.Options;

namespace AppMetrica.PostAPI;

public interface IAppMetricaUploader
{
    Task UploadEvent(AppMetricaEvent @event);
}

public class AppMetricaUploader : IAppMetricaUploader
{
    readonly HttpClient _httpClient;
    readonly AppMetricaOptions _options;

    public AppMetricaUploader(IOptions<AppMetricaOptions> options, HttpClient? httpClient = null)
    {
        _options = options.Value;
        _httpClient = httpClient ?? new HttpClient() { BaseAddress = AppMetricaOptions.BaseUrl };
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
            uriSb.Append("&appmetrica_device_id =" + @event.AppMetricaDeviceId);

        long timestamp = @event.EventTime.ToUnixTimeSeconds();
        uriSb.Append("&event_timestamp=" + timestamp);

        var req = new HttpRequestMessage(HttpMethod.Post, uriSb.ToString());

        var resp = await _httpClient.SendAsync(req);
        if (resp.StatusCode != HttpStatusCode.OK)
            throw new AppMetricaUploadException(resp.StatusCode);

        string respContent = await resp.Content.ReadAsStringAsync();
        if (respContent != "Your data has been uploaded.")
            throw new AppMetricaUploadException($"Unexpected answer from AppMetrica: '{respContent}'.");
    }
}
