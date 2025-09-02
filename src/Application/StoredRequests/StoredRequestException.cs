using PriceComparer.Application.Common;

namespace PriceComparer.Application.StoredRequests;

public class StoredRequestException : AppException
{
    public StoredRequestException(Code code)
        : base(code.ToString())
    {
        if (!Enum.IsDefined(code))
            throw new InvalidOperationException(
                $"Unknown value for {nameof(StoredRequestException)} error code"
            );
    }

    public enum Code
    {
        TryUpdateNonStored,
        TryDeleteNonStored,
    }
}
