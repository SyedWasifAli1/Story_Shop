using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_kharido.Models
{
    public class Project :DbContext
    {
        public Project() { }

        public Project(DbContextOptions<Project> option) : base(option)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Bill> Bills{ get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Feed> Feedis { get; set; }
        public DbSet<Subcriber> Subcribers { get; set; }

        public DbSet<TownsAdress> TownAddress { get; set; }

        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Writer> Writers { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }
}
