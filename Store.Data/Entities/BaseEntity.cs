using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Data.Entities
{
    public class BaseEntity<T>
    {
        private Func<object, object> value;

        public BaseEntity(Func<object, object> value)
        {
            this.value = value;
        }

        public T Id{ get; set; }
        public DateTime CreateAt { get; set; }= DateTime.Now;
    }
}
