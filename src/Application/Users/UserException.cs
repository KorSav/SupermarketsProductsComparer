using PriceComparer.Application.Common;

namespace PriceComparer.Application.Users;

class UserException : AppException
{
    public UserException(Code code)
        : base(code.ToString())
    {
        if (!Enum.IsDefined(code))
            throw new InvalidOperationException(
                $"Unknown value for {nameof(UserException)} error code"
            );
    }

    public enum Code
    {
        NameIsNotUnique,
        NonregisteredLogin,
    }
}
