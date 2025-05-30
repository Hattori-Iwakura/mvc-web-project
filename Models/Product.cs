namespace mvc_web_project.Models;

public class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int QuantityInStock { get; set; }

    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public override string ToString()
    {
        return $"ProductId: {ProductId}, " +
            $"Name: {Name}, " +
            $"Description: {Description}, " +
            $"Price: {Price:C}, " +
            $"QuantityInStock: {QuantityInStock}, " +
            $"ImageUrl: {ImageUrl}, " +
            $"CategoryId: {CategoryId}, " +
            $"Category: {Category?.Name}, " +
            $"CreatedAt: {CreatedAt:u}, " +
            $"UpdatedAt: {UpdatedAt:u}";
    }
}