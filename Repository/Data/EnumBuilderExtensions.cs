using Microsoft.EntityFrameworkCore;

static class EnumBuilderExtensions
{
    public static readonly string enumTblSuffix = "Lookup";

    public static ModelBuilder AddEnumLookup<TEnum>(
        this ModelBuilder modelBuilder,
        params (Type entity, string fk)[] dependents
    )
        where TEnum : struct, Enum
    {
        string[] names = Enum.GetNames<TEnum>();
        if (names.Length == 0)
            throw new ArgumentException("Enumeration could not be empty for lookup table creation");
        string tblName = typeof(TEnum).Name + enumTblSuffix;
        modelBuilder.SharedTypeEntity<Dictionary<string, object>>(
            tblName,
            eb =>
            {
                int maxNameLength = names.MaxBy(n => n.Length)!.Length;
                eb.Property<TEnum>("Id").HasConversion(typeof(TEnum).GetEnumUnderlyingType());
                eb.Property<string>("Name")
                    .IsUnicode(false)
                    .HasMaxLength(maxNameLength)
                    .IsRequired();
                eb.HasData(
                    Enum.GetValues<TEnum>()
                        .Select(ev => new Dictionary<string, object>()
                        {
                            { "Id", ev },
                            { "Name", ev.ToString() },
                        })
                );
                foreach (var (type, fk) in dependents)
                    eb.HasMany(type, null).WithOne().HasForeignKey(fk);
            }
        );
        return modelBuilder;
    }
}
