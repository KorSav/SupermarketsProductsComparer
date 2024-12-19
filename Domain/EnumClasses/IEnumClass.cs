namespace program.Domain.EnumClasses;

public interface IEnumClass<TEnum> where TEnum : Enum
{
    public TEnum Id { get; set; }
    public string Name { get; set; }
}