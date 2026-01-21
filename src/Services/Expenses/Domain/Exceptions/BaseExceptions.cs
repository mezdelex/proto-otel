namespace Domain.Exceptions;

public sealed class NotFoundException(string entity) : Exception($"{entity} not found.") { }
