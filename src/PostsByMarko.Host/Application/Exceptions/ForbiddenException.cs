namespace PostsByMarko.Host.Application.Exceptions;

public sealed class ForbiddenException(string message) : UnauthorizedAccessException(message);
