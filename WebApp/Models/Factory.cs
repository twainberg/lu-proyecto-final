using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WebApp.Models
{
    public class ApplicationDataContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
    {
        public ApplicationDataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();
            //optionsBuilder.UseSqlServer(Program.Configuration["ConnectionStrings:AuthConnection"]);
            optionsBuilder.UseSqlServer("data source=TW\\SQLEXPRESS;Initial Catalog=Auth;Trusted_Connection=True;MultipleActiveResultSets=true;");

            return new ApplicationDataContext(optionsBuilder.Options);
        }
    }
}