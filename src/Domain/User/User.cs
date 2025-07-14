namespace PriceComparer.Domain;

public record UserId(int Value);

public record User(UserId Id, UserName Name);
