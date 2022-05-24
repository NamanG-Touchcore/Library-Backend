using Microsoft.AspNetCore.Mvc;
using Library.Models;
using Library.Repositories;
using Microsoft.AspNetCore.Authorization;

// GET books
// PUT books
// GET books/:bookId
// PUT books/:bookId
// GET books/:bookId/issues
// PUT books/:bookId/issues

namespace Library.Controllers;
// [Authorize]
[ApiController]
[Route("books")]

public class BookController : ControllerBase
{
    public readonly IBookRepoSQL repo2;
    public readonly IIssueSQL issueRepo;
    public BookController(IBookRepoSQL _repo, IIssueSQL _issueRepo)
    {
        repo2 = _repo;
        issueRepo = _issueRepo;
    }
    [HttpGet]
    public IActionResult GetPaginatedBooks(int pageNo = -1, int pageSize = -1)
    {
        if (pageNo > 0 && pageSize > 0)
            return Ok(repo2.GetBooksByPage(pageNo, pageSize));
        return Ok(repo2.GetBookRecord().ToList());
    }
    [HttpGet("count")]
    public IActionResult GetBooksCount()
    {
        return Ok(repo2.getBookSize());
    }

    [HttpPut, Authorize(Roles = "Admin")]
    public IActionResult Put(IBook book)
    {
        return Ok(repo2.addBook(book));
    }

    [HttpGet("issues")]
    public IActionResult GetIssues()
    {
        return Ok(issueRepo.GetIssues());
    }
    [HttpGet("{bookId}")]
    public IActionResult Get(int bookId)
    {
        return Ok(repo2.getBook(bookId));
    }
    [HttpPut("{bookId}"), Authorize(Roles = "Admin")]
    public IActionResult Put(int bookId, IBookEF book)
    {
        return Ok(repo2.updateBook(bookId, book));
    }
    [HttpDelete("{bookId}"), Authorize(Roles = "Admin")]
    public IActionResult Delete(int bookId)
    {
        return Ok(repo2.DeleteBook(bookId));
    }
    [HttpGet("{bookId}/issues")]
    public IActionResult GetIssues(int bookId)
    {
        return Ok(issueRepo.GetIssuesByBookId(bookId));
    }
    [HttpPut("{bookId}/issues")]
    public IActionResult PutIssues(int bookId, IIssueEF issue)
    {
        return Ok(issueRepo.putIssues(bookId, issue));
    }
    [HttpPut("{bookId}/issues/{issueId}")]
    public IActionResult UpdateIssue(int bookId, int issueId, int isActive, int fine)
    {
        return Ok(issueRepo.putIssue(bookId, issueId, isActive, fine));
    }
}