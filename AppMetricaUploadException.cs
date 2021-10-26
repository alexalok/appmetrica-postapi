using System.Net;

namespace AppMetrica.PostAPI;

public class AppMetricaUploadException : Exception
{
    public AppMetricaUploadException(HttpStatusCode statusCode) : base(GetMessage(statusCode))
    {

    }

    static string GetMessage(HttpStatusCode statusCode) =>
        statusCode switch
        {
            HttpStatusCode.Forbidden => "The request omitted an authorization header, or an invalid token was specified.",
            HttpStatusCode.BadRequest => "One or more required parameters were missing in the request.",
            _ => $"Status code indicates the upload was unsuccessful: {statusCode}."
        };
}
