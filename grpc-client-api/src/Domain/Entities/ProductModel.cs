namespace Domain.Entities;

public class ProductModel
{
    public Guid Id { get; set; }

    public string Sku { get; set; }

    public string Name { get; set; }

    public DateTime ManufacturedDate { get; set; }

    public double ListPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
