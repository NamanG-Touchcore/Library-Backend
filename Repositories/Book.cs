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
        private static BookContext context;

        public BookRepoSQL(IConfiguration _configuration)
        {
            configuration = _configuration;
            // Constr=configuration.GetConnectionString("DbConnection");
            Constr = configuration.GetConnectionString("DbConnection");
            context = new BookContext(Constr);
            Console.WriteLine(Constr);
        }

        public IEnumerable<IBook> GetBookRecord()
        {
            var books = (from b in context?.bookTable
                         where b.isBookActive == 1
                         select new IBook
                         {
                             id = b.bookId,
                             name = b.name,
                             author = b.author,
                             description = b.description,
                             coverImage = b.coverImage,
                             isBookActive = b.isBookActive,
                             activeIssues = b.activeIssues,
                             totalQuantity = b.totalQuantity,
                             issues = b.issues
                         });
            return (IEnumerable<IBook>)books;
        }
        public int getBookSize()
        {

            try
            {
                var books = (from b in context.bookTable
                             where b.isBookActive == 1
                             select new IBook
                             {
                                 id = b.bookId,
                                 name = b.name,
                                 author = b.author,
                                 description = b.description,
                                 coverImage = b.coverImage,
                                 isBookActive = b.isBookActive,
                                 activeIssues = b.activeIssues,
                                 totalQuantity = b.totalQuantity,
                                 issues = b.issues
                             });
                return books.Count();
            }
            catch (AppException e)
            {
                throw e;
            }
        }
        public List<IBook> GetBooksByPage(int pageNumber, int pageSize)
        {
            var books = (from b in context.bookTable
                         where b.isBookActive == 1
                         select new IBook
                         {
                             id = b.bookId,
                             name = b.name,
                             author = b.author,
                             description = b.description,
                             coverImage = b.coverImage,
                             isBookActive = b.isBookActive,
                             activeIssues = b.activeIssues,
                             totalQuantity = b.totalQuantity,
                             issues = b.issues
                         });
            var paginatedList = books.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            paginatedList[0].totalQuantity = this.getBookSize();
            return paginatedList;
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
                var bookObj = new IBookEF { name = book.name, author = book.author, issues = book.issues, description = book.description, totalQuantity = 10, coverImage = book.coverImage, isBookActive = 1, activeIssues = 0, bookId = null };
                context.bookTable.Add(bookObj);
                context.SaveChangesAsync();
                return new IReturnStatement() { message = "Book Added!" };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IBookEF getBook(int id)
        {
            try
            {
                IBookEF book = context.bookTable.SingleOrDefault(book => book.bookId == id);
                return book;
            }
            catch (System.Exception)
            {

                throw;
            }
        }


        public IReturnStatement updateBook(int bookId, IBookEF book)
        {
            try
            {
                var bookObj = context.bookTable.SingleOrDefault(book => book.bookId == bookId);
                bookObj.author = book.author;
                bookObj.name = book.name;
                bookObj.description = book.description;
                if (book.coverImage != "unchanged")
                    bookObj.coverImage = book.coverImage;
                context.SaveChanges();
                return new IReturnStatement { message = "Book Updated Successfully!" };

            }
            catch (System.Exception)
            {

                throw;
            }
        }
        public IReturnStatement DeleteBook(int id)
        {
            try
            {
                var book = context.bookTable.SingleOrDefault(book => book.bookId == id);
                book.isBookActive = 0;
                context.SaveChanges();
                return new IReturnStatement { message = "Book Deleted Successfully!" };

            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
    public interface IBookRepoSQL
    {
        public IEnumerable<IBook> GetBookRecord();
        public IReturnStatement addBook(IBook book);
        public IBookEF getBook(int id);
        public IReturnStatement updateBook(int bookId, IBookEF book);
        public IReturnStatement DeleteBook(int bookId);
        public int getBookSize();
        public List<IBook> GetBooksByPage(int pageNo, int pageSize);
    }

}