using dotnet_app.Data;
using Microsoft.EntityFrameworkCore;

namespace dotnet_app.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}
        public DbSet<Value> Values{get;set;}
    }
}