using Microsoft.AspNetCore.Identity;
using Store.Data.Entities.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repository
{
    public class StoreIdentityContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Salma Basem",
                    Email = "salma@gmail.com",
                    UserName = "salmaBasem",
                    Address=new Address
                    {
                        FirstName="Salma",
                        LastName="Basem",
                        City="El Shrouk",
                        State="Cairo",
                        street="3",
                        PostalCode="12345"
                    }
                };
                await userManager.CreateAsync(user,"Password123!");
            }
        }
    }
}
