namespace Domain.Constants;

public static class Errors
{
    public const string NotFoundError = "Not Found Error";

    public static string NotFoundErrorDetail(string entity) => $"{entity} not found";

    public const string ServerError = "Server Error";
    public const string ServerErrorDetail = "An unexpected error ocurred";
}
