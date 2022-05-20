using Library.Models;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Library.Repositories
{

    public class BookContext : DbContext
    {
        public string Constr { get; set; }
        public IConfiguration configuration;
        public SqlConnection? con;

        public BookContext(string _ConStr)
        {
            this.Constr = _ConStr;
        }

        public DbSet<IBookEF> bookTable { get; set; }
        public DbSet<IIssueEF> issueTableObj { get; set; }
        public DbSet<IUserEF> userTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Constr);
        }

    }
}