namespace AppMetrica.PostAPI;

/// <summary>
/// 
/// </summary>
/// <param name="EventName">Event name.</param>
/// <param name="EventTime">Time of the event. 
///     You can upload events only if the difference between the event date and the upload date is no more than seven days.</param>
/// <param name="AppMetricaDeviceId">Hash from the unique identifier of the device set by AppMetrica. The Post API allows you only to upload data for identifiers that were previously sent via the SDK. 
///     Do not pass the value with the <paramref name="ProfileId"/> parameter. The server accepts only one of these parameters.</param>
/// <param name="ProfileId">User profile ID. The Post API allows you only to upload data for identifiers that were previously sent via the SDK. 
///     Do not pass the value with the <paramref name="AppMetricaDeviceId"/> parameter. The server accepts only one of these parameters.</param>
public record AppMetricaEvent(string EventName, DateTimeOffset EventTime, string? AppMetricaDeviceId = null, string? ProfileId = null);
