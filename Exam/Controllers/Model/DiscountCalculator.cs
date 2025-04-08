namespace Exam.Controllers.Model
{
    public class Product
    {
        public int ProductID { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal AfterDiscountPrice { get; set; }
    }

    public class Promotion
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public decimal Discount { get; set; }
        public string? DiscountType { get; set; }
        public List<int>? ProductID { get; set; }
    }

    public class DiscountRequest
    {
        public List<Product> Products { get; set; } = new();
        public List<Promotion> Promotions { get; set; } = new();
    }
}
