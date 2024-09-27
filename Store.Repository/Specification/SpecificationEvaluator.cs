using Microsoft.EntityFrameworkCore;
using Store.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository.Specification
{
    public class SpecificationEvaluator<TEntity,TKey> where TEntity : BaseEntity<TKey>
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inpurQuery,ISpecification<TEntity> specs)
        {
            var query = inpurQuery;

            if(specs.Criteria is not null)
                query= query.Where(specs.Criteria);
            if(specs.OrderBy is not null)
                query= query.OrderBy(specs.OrderBy);
            if(specs.OrderByDescending is not null)
                query= query.OrderByDescending(specs.OrderByDescending);
            if (specs.IsPaginated)
                query = query.Skip(specs.Skip).Take(specs.Take);

            query = specs.Includes.Aggregate(query,(current,includeExpression) => current.Include(includeExpression));
            return query;
        }
    }
}
