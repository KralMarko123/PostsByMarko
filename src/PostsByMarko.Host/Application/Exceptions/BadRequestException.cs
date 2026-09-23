namespace PostsByMarko.Host.Application.Exceptions;

public sealed class BadRequestException(string message) : ArgumentException(message);
