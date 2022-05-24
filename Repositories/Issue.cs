using Library.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Library.Repositories
{

    public class IssueRepo : IIssueSQL
    {
        public string Constr { get; set; }
        public IConfiguration configuration;
        public SqlConnection? con;
        public BookContext context;
        public IssueRepo(IConfiguration _configuration)
        {
            configuration = _configuration;
            Constr = configuration.GetConnectionString("DbConnection");
            this.context = new BookContext(Constr);
            Console.WriteLine(Constr);
        }
        public IEnumerable<IIssue> GetIssues()
        {
            try
            {
                var issues = (from i in context.issueTableObj
                              join u in context.userTable on i.userId equals u.userId
                              join b in context.bookTable on i.bookId equals b.bookId
                              select new IIssue
                              {
                                  bookId = i.bookId,
                                  isActive = i.isActive,
                                  expiryDate = i.expiryDate,
                                  issueDate = i.issueDate,
                                  userId = u.userId,
                                  id = i.id,
                                  username = u.username,
                                  returnDate = i.returnDate,
                                  name = b.name,
                                  fine = i.fine
                              });
                return (IEnumerable<IIssue>)issues;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public IEnumerable<IIssue> GetIssuesByBookId(int bookId)
        {
            try
            {
                var issues = (from i in context.issueTableObj
                              join u in context.userTable on i.userId equals u.userId
                              join b in context.bookTable on i.bookId equals b.bookId
                              where i.bookId == bookId
                              select new IIssue
                              {
                                  bookId = i.bookId,
                                  isActive = i.isActive,
                                  expiryDate = i.expiryDate,
                                  issueDate = i.issueDate,
                                  userId = u.userId,
                                  id = i.id,
                                  username = u.username,
                                  returnDate = i.returnDate,
                                  name = b.name,
                                  fine = i.fine
                              });
                return (IEnumerable<IIssue>)issues;
            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public IEnumerable<IBook> GetIssuesByUserId(int id)
        {
            List<IBook> issueList = new();
            try
            {
                string query = $"SELECT  bookTable.*,issueTableObj.* FROM issueTableObj  INNER JOIN bookTable ON issueTableObj.userId={id} AND bookTable.isBookActive=1 AND issueTableObj.bookID=bookTable.bookId ";
                using (con = new SqlConnection(Constr))
                {
                    con.Open();
                    var cmd = new SqlCommand(query, con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        IBook bookObj = new IBook();
                        bookObj.id = Convert.ToInt32(rdr["bookId"]);
                        bookObj.author = Convert.ToString(rdr["author"]);
                        bookObj.description = Convert.ToString(rdr["description"]);
                        bookObj.name = Convert.ToString(rdr["name"]);
                        bookObj.issues = Convert.ToInt32(rdr["issues"]);
                        bookObj.issueId = Convert.ToInt32(rdr["id"]);
                        bookObj.isActive = Convert.ToInt32(rdr["isActive"]);
                        bookObj.returnDate = Convert.ToString(rdr["returnDate"]);
                        bookObj.issueDate = Convert.ToString(rdr["issueDate"]);
                        bookObj.expiryDate = Convert.ToString(rdr["expiryDate"]);
                        bookObj.coverImage = Convert.ToString(rdr["coverImage"]);
                        bookObj.isBookActive = Convert.ToInt32(rdr["isBookActive"]);

                        issueList.Add(bookObj);
                    }
                }
                return issueList;
            }
            catch (AppException e)
            {
                throw e;
            }
            finally
            {
                con?.Close();
            }
        }
        public IReturnStatement putIssues(int bookId, IIssueEF issue)
        {
            try
            {
                context.issueTableObj.Add(issue);
                context.SaveChanges();
                return new IReturnStatement { message = "Book Issued Successfully!" };
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        public IReturnStatement putIssue(int bookId, int issueId, int isActive, int fine)
        {
            var issue = context.issueTableObj.SingleOrDefault(i => i.id == issueId);
            if (isActive == 0)
            {
                var date = DateTime.Today.ToLongDateString();
                if (issue is not null)
                    issue.returnDate = date;
            }
            if (issue is not null)
            {
                issue.isActive = isActive;
                issue.fine = fine;
            }
            var book = context.bookTable.SingleOrDefault(b => b.bookId == bookId);
            if (book is not null)
                book.activeIssues += isActive == 0 ? -1 : 1;
            context.SaveChanges();
            return new IReturnStatement() { message = "Book " + (isActive == 1 ? "Issued" : "Returned") + "!" };

        }
    }

    public interface IIssueSQL
    {
        public IEnumerable<IIssue> GetIssuesByBookId(int id);
        public IEnumerable<IIssue> GetIssues();
        public IReturnStatement putIssues(int bookId, IIssueEF issue);
        public IReturnStatement putIssue(int issueId, int bookId, int func, int fine);
        public IEnumerable<IBook> GetIssuesByUserId(int id);
    }

}