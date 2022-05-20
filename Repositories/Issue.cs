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


        // public IEnumerable<IIssue> GetIssues()
        // {
        //     List<IIssue> issueList = new();
        //     try
        //     {
        //         string query = $"SELECT issueTableObj.*, username, bookTable.name FROM issueTableObj INNER JOIN userTable ON userTable.userId=issueTableObj.userId INNER JOIN bookTable ON bookTable.bookId=issueTableObj.bookId;";
        //         using (con = new SqlConnection(Constr))
        //         {
        //             con.Open();
        //             var cmd = new SqlCommand(query, con);
        //             SqlDataReader rdr = cmd.ExecuteReader();
        //             while (rdr.Read())
        //             {
        //                 IIssue issueObj = new IIssue();
        //                 issueObj.isActive = Convert.ToInt32(rdr["isActive"]);
        //                 issueObj.expiryDate = Convert.ToString(rdr["expiryDate"]);
        //                 issueObj.issueDate = Convert.ToString(rdr["issueDate"]);
        //                 issueObj.userId = Convert.ToInt32(rdr["userId"]);
        //                 issueObj.id = Convert.ToInt32(rdr["id"]);
        //                 issueObj.bookId = Convert.ToInt32(rdr["bookId"]);
        //                 issueObj.returnDate = Convert.ToString(rdr["returnDate"]);
        //                 issueObj.username = Convert.ToString(rdr["username"]);
        //                 issueObj.name = Convert.ToString(rdr["name"]);
        //                 issueObj.fine = Convert.ToInt32(rdr["fine"]);
        //                 issueList.Add(issueObj);
        //             }
        //         }
        //         return issueList;
        //     }
        //     catch (AppException e)
        //     {
        //         throw e;
        //     }
        //     finally
        //     {
        //         con.Close();
        //     }
        // }

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
        // public IEnumerable<IIssue> GetIssuesByBookId(int bookId)
        // {
        //     List<IIssue> issueList = new();
        //     try
        //     {
        //         string query = $"SELECT issueTableObj.*,username FROM issueTableObj INNER JOIN userTable  ON issueTableObj.bookId={bookId} AND issueTableObj.userId=userTable.userId ";
        //         using (con = new SqlConnection(Constr))
        //         {
        //             con.Open();
        //             var cmd = new SqlCommand(query, con);
        //             SqlDataReader rdr = cmd.ExecuteReader();
        //             while (rdr.Read())
        //             {
        //                 IIssue issueObj = new IIssue();
        //                 issueObj.bookId = Convert.ToInt32(rdr["bookId"]);
        //                 issueObj.expiryDate = Convert.ToString(rdr["expiryDate"]);
        //                 issueObj.issueDate = Convert.ToString(rdr["issueDate"]);
        //                 issueObj.userId = Convert.ToInt32(rdr["userId"]);
        //                 issueObj.id = Convert.ToInt32(rdr["id"]);
        //                 issueObj.isActive = Convert.ToInt32(rdr["isActive"]);
        //                 issueObj.username = Convert.ToString(rdr["username"]);
        //                 issueObj.returnDate = Convert.ToString(rdr["returnDate"]);
        //                 issueObj.fine = Convert.ToInt32(rdr["fine"]);
        //                 issueList.Add(issueObj);
        //             }
        //         }
        //         return issueList;
        //     }
        //     catch (AppException e)
        //     {
        //         throw e;
        //     }
        //     finally
        //     {
        //         con.Close();
        //     }
        // }

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
                con.Close();
            }
        }

        // public IEnumerable<IBook> GetIssuesByUserId(int id)
        // {
        //     var list=(from i in context.issueTableObj join b in context.bookTable on i.userId == id && 1 equals b.isBookActive && i.bookId equals b.bookId );
        // }

        // public IReturnStatement putIssues(int bookId, IIssue issue)
        // {
        //     try
        //     {
        //         List<IIssue> issueList = new();
        //         string query = $"INSERT INTO issueTableObj ( bookId, userId, issueDate, expiryDate, isActive, returnDate, fine) VALUES ('{issue.bookId}', '{issue.userId}', '{issue.issueDate}','{issue.expiryDate}','{issue.isActive}', '{issue.returnDate}', '{issue.fine}');";
        //         string queryUpdate = $"UPDATE bookTable SET issues=issues+1, activeIssues=activeIssues+1 WHERE bookId='{bookId}'";
        //         using (con = new SqlConnection(Constr))
        //         {
        //             con.Open();
        //             var cmd = new SqlCommand(query, con);
        //             SqlDataReader rdr = cmd.ExecuteReader();
        //             while (rdr.Read())
        //             {
        //                 IIssue issueObj = new IIssue();
        //                 issueObj.bookId = Convert.ToInt32(rdr["bookId"]);
        //                 issueObj.expiryDate = Convert.ToString(rdr["expiryDate"]);
        //                 issueObj.issueDate = Convert.ToString(rdr["issueDate"]);
        //                 issueObj.userId = Convert.ToInt32(rdr["userId"]);
        //                 issueObj.id = Convert.ToInt32(rdr["id"]);
        //                 issueObj.isActive = Convert.ToInt32(rdr["isActive"]);
        //                 issueObj.returnDate = Convert.ToString(rdr["returnDate"]);
        //                 issueList.Add(issueObj);
        //             }
        //             rdr.Close();
        //             cmd = new SqlCommand(queryUpdate, con);
        //             rdr = cmd.ExecuteReader();
        //         }
        //         // return books.ToList();
        //         return new IReturnStatement() { message = "Book Issued" };
        //     }
        //     catch (AppException e)
        //     {
        //         throw e;
        //     }
        //     finally
        //     {
        //         con.Close();
        //     }
        // }

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

        // public IReturnStatement putIssue(int bookId, int issueId, int isActive, int fine)
        // {
        //     List<IIssue> issueList = new();

        //     string query;
        //     string queryUpdate;
        //     if (isActive == 0)
        //     {
        //         var date = DateTime.Today.ToLongDateString();
        //         query = $"UPDATE issueTableObj SET isActive='{isActive}', returnDate='{date}', fine={fine} WHERE id='{issueId}'";
        //         queryUpdate = $"UPDATE bookTable SET activeIssues=activeIssues-1 WHERE bookId='{bookId}'";
        //     }
        //     else
        //     {
        //         queryUpdate = $"UPDATE bookTable SET activeIssues=activeIssues+1 WHERE bookId='{bookId}'";
        //         query = $"UPDATE issueTableObj SET isActive='{isActive}', returnDate='null', fine={0} WHERE id='{issueId}'";
        //     }
        //     using (con = new SqlConnection(Constr))
        //     {
        //         con.Open();
        //         var cmd = new SqlCommand(query, con);
        //         SqlDataReader rdr = cmd.ExecuteReader();
        //         rdr.Close();
        //         cmd = new SqlCommand(queryUpdate, con);
        //         rdr = cmd.ExecuteReader();
        //     }
        //     // return books.ToList();
        //     return new IReturnStatement() { message = "Book " + (isActive == 1 ? "Issued" : "Returned") + "!" };
        // }

        public IReturnStatement putIssue(int bookId, int issueId, int isActive, int fine)
        {
            var issue = context.issueTableObj.SingleOrDefault(i => i.id == issueId);
            if (isActive == 0)
            {
                var date = DateTime.Today.ToLongDateString();
                issue.returnDate = date;
            }
            issue.isActive = isActive;
            issue.fine = fine;
            var book = context.bookTable.SingleOrDefault(b => b.bookId == bookId);
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