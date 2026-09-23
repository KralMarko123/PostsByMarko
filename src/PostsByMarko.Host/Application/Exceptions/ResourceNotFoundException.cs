namespace PostsByMarko.Host.Application.Exceptions;

public sealed class ResourceNotFoundException(string message) : KeyNotFoundException(message);
