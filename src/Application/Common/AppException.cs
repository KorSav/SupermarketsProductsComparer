namespace PriceComparer.Application.Common;

public class AppException(string? message = null, Exception? inner = null)
    : Exception(message, inner);
