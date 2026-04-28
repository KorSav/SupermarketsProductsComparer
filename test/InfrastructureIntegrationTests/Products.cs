using ApplicationCore.Entities.Product;

namespace InfrastructureIntegrationTests;

public static class Products
{
    public static Product FozzyApple =>
        new(
            Name: "Apple 🍎, 500г",
            Measure: new(500, MeasureUnit.Gram),
            Price: 15,
            LinkImage: new Uri("https://image-stock.com/guid"),
            LinkProduct: new Uri("https://fozzy-products.com/1"),
            Shop: Shop.Fozzy
        );
    public static Product FozzySpice =>
        new(
            Name: "Some spice, 10г",
            Measure: new(10, MeasureUnit.Gram),
            Price: 20,
            LinkImage: new Uri("https://fozzy-images.com/2"),
            LinkProduct: new Uri("https://fozzy-products.com/2"),
            Shop: Shop.Fozzy
        );
    public static Product FozzySpiceDup =>
        new(
            Name: "Some spice, 10г",
            Measure: new(10, MeasureUnit.Gram),
            Price: 20,
            LinkImage: new Uri("https://fozzy-images.com/2"),
            LinkProduct: new Uri("https://fozzy-products.com/2"),
            Shop: Shop.Fozzy
        );
    public static Product SilpoMilk =>
        new(
            Name: "Молоко, 1л",
            Measure: new(1, MeasureUnit.Litre),
            Price: 30,
            LinkImage: new Uri("https://silpo-images.com/1"),
            LinkProduct: new Uri("https://silpo-products.com/1"),
            Shop: Shop.Silpo
        );
    public static Product SilpoDrink =>
        new(
            Name: "Напій juice, 200мл",
            Measure: new(200, MeasureUnit.MiliLitre),
            Price: 32,
            LinkImage: new Uri("https://image-stock.com/guid2"),
            LinkProduct: new Uri("https://silpo-products.com/1"),
            Shop: Shop.Silpo
        );
    public static Product ForaCheese =>
        new(
            Name: "Сир гол., 300г",
            Measure: new(300, MeasureUnit.Gram),
            Price: 120,
            LinkImage: new Uri("https://fora-images.com/2"),
            LinkProduct: new Uri("https://fora-products.com/2"),
            Shop: Shop.Fora
        );
    public static Product ForaCheese2 =>
        new(
            Name: "Сир гол., 1кг",
            Measure: new(1, MeasureUnit.KiloGram),
            Price: 360,
            LinkImage: new Uri("https://image-stock.com/3"),
            LinkProduct: new Uri("https://fora-products.com/3"),
            Shop: Shop.Fora
        );
}
