using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data.Entities
{
  
        public class DeliveryMethod : BaseEntity<int>
        {
            public string ShortName { get; set; }
            public string Description { get; set; }
            public string DeliveryTime { get; set; }
            public decimal Price { get; set; }

        // Default constructor that uses a default Func
        public DeliveryMethod() : base(obj => obj) // or just pass null if appropriate
        {
        }
    }
    
}
