using System.ComponentModel.DataAnnotations;
using program.Domain.Enums;

namespace program.Domain;

public class Measure: IEnumClass<MeasureId>
{
    public MeasureId Id { get; set; }
    [MaxLength(5)]
    public string Name { get; set; } = string.Empty;

    public List<Product> Products {get; set;} = [];
}