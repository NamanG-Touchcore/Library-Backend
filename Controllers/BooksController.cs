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
    // public static IssueRepo issueRepo =new();
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(repo2.GetBookRecord().ToList());

    }
    // [HttpPut, Authorize(Roles = "Admin")]
    [HttpPut]
    public IActionResult Put(IBook book)
    {
        return Ok(repo2.addBook(book));
    }
    [HttpGet("query/{query}")]
    public IActionResult GetQueriedBooks(string query)
    {
        return Ok(repo2.GetBookQueries(query));
    }
    [HttpGet("issues")]
    public IActionResult GetIssues()
    {
        return Ok(issueRepo.GetIssues());
    }
    // This method is redundant because the implementation of selective data to be returned from the query is yet to be implemented, therefore there is no point of a separate route that returns the specific book information
    [HttpGet("{bookId}")]
    public IActionResult Get(int bookId)
    {
        return Ok(repo2.getBook(bookId));
    }
    // [HttpPut("{bookId}"), Authorize(Roles = "Admin")]
    [HttpPut("{bookId}")]
    public IActionResult Put(int bookId, IBookEF book)
    {
        return Ok(repo2.updateBook(bookId, book));
    }
    [HttpDelete("{bookId}")]
    // [HttpDelete("{bookId}"), Authorize(Roles = "Admin")]
    public IActionResult Delete(int bookId)
    {
        return Ok(repo2.DeleteBook(bookId));
    }
    [HttpGet("{bookId}/issues")]
    public IActionResult GetIssues(int bookId)
    {
        return Ok(issueRepo.GetIssuesByBookId(bookId));
    }
    // [HttpGet("{bookId}/issue/{issueId}")]
    // public IIssue GetIssue(int issueId)
    // {
    //     return issueRepo.GetIssue(issueId);
    // }
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
    // [HttpPut("{bookId}/issues/{issueId}")]
    // public void PutIssue(int bookId, int IssueId, IIssue issue)
    // {

    // }
}