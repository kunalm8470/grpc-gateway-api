namespace Domain.Models.Requests;

public class UpdateProductDto
{
    public string Sku { get; set; }

    public string Name { get; set; }

    public DateTime ManufacturedDate { get; set; }

    public double ListPrice { get; set; }
}
