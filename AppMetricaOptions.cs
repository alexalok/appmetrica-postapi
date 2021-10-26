namespace AppMetrica.PostAPI;

public class AppMetricaOptions
{
    public static readonly Uri BaseUrl = new Uri("https://api.appmetrica.yandex.ru/logs/v1/import/events");

    public int ApplicationId { get; set; }

    public string PostApiKey { get; set; } = null!;
}
