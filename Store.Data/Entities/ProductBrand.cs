namespace Store.Data.Entities
{
    public class ProductBrand:BaseEntity<int>
    {
        public string Name { get; set; }

        // Default constructor that uses a default Func
        public ProductBrand() : base(obj => obj) // or just pass null if appropriate
        {
        }
    }
}