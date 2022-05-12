using Library.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Library.Repositories
{

    public class BookRepoSQL : IBookRepoSQL
    {
        public string Constr { get; set; }
        public IConfiguration configuration;
        public SqlConnection? con;

        public BookRepoSQL(IConfiguration _configuration)
        {
            configuration = _configuration;
            // Constr=configuration.GetConnectionString("DbConnection");
            Constr = configuration.GetConnectionString("DbConnection");
            Console.WriteLine(Constr);
        }

        public List<IBook> GetBookRecord()
        {
            try
            {
                List<IBook> books = new();
                // string str="null";
                using (con = new SqlConnection(Constr))
                {
                    con.Open();
                    var cmd = new SqlCommand("GetBookRecords", con);
                    // cmd.CommandType=System.Data.CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //  return rdr["author"];
                        IBook bookObj = new IBook();
                        bookObj.id = Convert.ToInt32(rdr["bookId"]);
                        bookObj.name = Convert.ToString(rdr["name"]);
                        bookObj.author = Convert.ToString(rdr["author"]);
                        if ((rdr["issues"] is DBNull))
                            bookObj.issues = 0;
                        else
                            bookObj.issues = Convert.ToInt32(rdr["issues"]);

                        bookObj.description = Convert.ToString(rdr["description"]);
                        bookObj.coverImage = Convert.ToString(rdr["coverImage"]);
                        bookObj.totalQuantity = Convert.ToInt32(rdr["totalQuantity"]);
                        bookObj.activeIssues = Convert.ToInt32(rdr["activeIssues"]);
                        bookObj.isBookActive = Convert.ToInt32(rdr["isBookActive"]);
                        books.Add(bookObj);
                    }
                }
                return books;
            }
            catch (AppException e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
            // return books.ToList();
        }
        public int getBookSize(){
            
            try
            {
                using (con = new SqlConnection(Constr))
                {
                    con.Open();
                    var cmd = new SqlCommand("SELECT count(*) from bookTable", con);
                    // cmd.CommandType=System.Data.CommandType.StoredProcedure;
                    var rdr = Convert.ToInt32(cmd.ExecuteScalar());
                    return rdr;
                }
                // return books.ToList();
            return 0;
        }
        catch(AppException e)
        {
            throw e;
        }
        }
        public bool validateBook(IBook book)
        {
            if (book.name.Length > 0 && book.description.Length > 0 && book.author.Length > 0)
            {
                return true;
            }
            return false;
        }
        public IReturnStatement addBook(IBook book)
        {
            try
            {
                if (!validateBook(book))
                    throw new AppException("Book Details not Valid!");
                IBook bookObj = new IBook();
                string query = "INSERT INTO bookTable ( name, author, issues, description, coverImage, isBookActive, totalQuantity, activeIssues) VALUES ('" + book.name + "', '" + book.author + "', '" + book.issues + "', '" + book.description + "','" + book.coverImage + "', '1', '0', '0');";
                using (con = new SqlConnection(Constr))
                {
                    con.Open();
                    var cmd = new SqlCommand("AddBook", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@bookName", book.name);
                    cmd.Parameters.AddWithValue("@bookAuthor", book.author);
                    cmd.Parameters.AddWithValue("@bookDescription", book.description);
                    cmd.Parameters.AddWithValue("@bookImage", !(book.coverImage is null) ? book.coverImage : "");
                    cmd.Parameters.AddWithValue("@bookIssues", book.issues);
                    // cmd.CommandType=System.Data.CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //  return rdr["author"];
                        bookObj.id = Convert.ToInt32(rdr["bookId"]);
                        bookObj.name = Convert.ToString(rdr["name"]);
                        bookObj.author = Convert.ToString(rdr["author"]);
                        if ((rdr["issues"] is DBNull))
                            bookObj.issues = 0;
                        else
                            bookObj.issues = Convert.ToInt32(rdr["issues"]);
                        bookObj.description = Convert.ToString(rdr["description"]);
                    }
                }
                // return books.ToList();
                return new IReturnStatement() { message = "Book Added!" };
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
        public IBook getBook(int id)
        {
            try
            {
                IBook bookObj = new IBook();
                string query = "SELECT * FROM bookTable where bookId = " + id + " ;";
                using (con = new SqlConnection(Constr))
                {
                    con.Open();
                    var cmd = new SqlCommand("getBook", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@bookId", id);
                    // cmd.CommandType=System.Data.CommandType.StoredProcedure;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //  return rdr["author"];
                        bookObj.id = Convert.ToInt32(rdr["bookId"]);
                        bookObj.name = Convert.ToString(rdr["name"]);
                        bookObj.author = Convert.ToString(rdr["author"]);
                        if ((rdr["issues"] is DBNull))
                            bookObj.issues = 0;
                        else
                            bookObj.issues = Convert.ToInt32(rdr["issues"]);
                        bookObj.description = Convert.ToString(rdr["description"]);
                        bookObj.totalQuantity = Convert.ToInt32(rdr["totalQuantity"]);
                        bookObj.isBookActive = Convert.ToInt32(rdr["isBookActive"]);
                        bookObj.activeIssues = Convert.ToInt32(rdr["activeIssues"]);
                    }
                }
                // return books.ToList();
                if (bookObj.name is null)
                    throw new AppException("Book Not Found!");
                return bookObj;
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
        public IReturnStatement updateBook(int bookId, IBook book)
        {
            try
            {
                if (!validateBook(book))
                    throw new AppException("Book Invalid");
                IBook bookObj = new IBook();
                string query;
                if (book.coverImage != "unchanged")
                    // query = $"UPDATE bookTable SET name='{book.name}', author='{book.author}', description='{book.description}',coverImage='{book.coverImage}'   WHERE bookId='{bookId}'";
                    query = "updateBookWithCoverImage";
                else
                    // query = $"UPDATE bookTable SET name='{book.name}', author='{book.author}', description='{book.description}'   WHERE bookId='{bookId}'";
                    query = "updateBookWithoutCoverImage";
                using (con = new SqlConnection(Constr))
                {
                    con.Open();
                    var cmd = new SqlCommand(query, con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@bookName", book.name);
                    cmd.Parameters.AddWithValue("@bookAuthor", book.author);
                    cmd.Parameters.AddWithValue("@bookDescription", book.description);
                    cmd.Parameters.AddWithValue("@bookImage", !(book.coverImage is null) ? book.coverImage : "");
                    cmd.Parameters.AddWithValue("@bookId", bookId);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //  return rdr["author"];
                        bookObj.id = Convert.ToInt32(rdr["bookId"]);
                        bookObj.name = Convert.ToString(rdr["name"]);
                        bookObj.author = Convert.ToString(rdr["author"]);
                        if ((rdr["issues"] is DBNull))
                            bookObj.issues = 0;
                        else
                            bookObj.issues = Convert.ToInt32(rdr["issues"]);
                        bookObj.description = Convert.ToString(rdr["description"]);
                    }
                }
                // return books.ToList();
                return new IReturnStatement() { message = "Book Updated!" };
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
        public IReturnStatement DeleteBook(int id)
        {
            try
            {
                IBook bookObj = new IBook();
                string query = $"UPDATE bookTable SET isBookActive=0 WHERE bookId={id}";
                using (con = new SqlConnection(Constr))
                {
                    con.Open();
                    var cmd = new SqlCommand("deleteBook", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@bookId", id);
                    SqlDataReader rdr = cmd.ExecuteReader();

                }
                // return books.ToList();
                return new IReturnStatement() { message = "Book Deleted Successfully" };
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
        public List<IBookQuery> GetBookQueries(string query)
        {
            try
            {
                List<IBookQuery> bookList = new List<IBookQuery>();
                using (con = new SqlConnection(Constr))
                {
                    con.OpenAsync();
                    var cmd = new SqlCommand("SELECT * from bookTable WHERE name LIKE '%" + query + "%'", con);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        IBookQuery bookObj = new IBookQuery();
                        bookObj.id = Convert.ToInt32(rdr["bookId"]);
                        bookObj.name = Convert.ToString(rdr["name"]);
                        bookList.Add(bookObj);
                    }
                    rdr.CloseAsync();
                }
                // return books.ToList();
                return bookList;
            }
            catch (AppException e)
            {
                throw e;
            }
            finally
            {
                con.CloseAsync();
            }
        }
    }
    public interface IBookRepoSQL
    {
        public List<IBook> GetBookRecord();
        public IReturnStatement addBook(IBook book);
        public IBook getBook(int id);
        public List<IBookQuery> GetBookQueries(string query);
        public IReturnStatement updateBook(int bookId, IBook book);
        public IReturnStatement DeleteBook(int bookId);
        public int getBookSize();
    }

}