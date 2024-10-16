namespace Store.Data.Entities
{
    public class ProductType:BaseEntity<int>
    {
        public string Name { get; set; }

        // Default constructor that uses a default Func
        public ProductType() : base(obj => obj) // or just pass null if appropriate
        {
        }
    }
}