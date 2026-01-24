namespace Application.Responses;

public record Error(string Code, string Description, ErrorTypes Type);
