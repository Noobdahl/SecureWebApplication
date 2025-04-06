public class Product
{
    public int ProductID { get; set; } // Primary key
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}