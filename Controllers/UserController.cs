using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Library.Models;
using Library.Repositories;

// GET books
// PUT books
// GET books/:bookId
// PUT books/:bookId
// GET books/:bookId/issues
// PUT books/:bookId/issues

namespace Library.Controllers;
[ApiController]
[Route("user")]

public class UserController : ControllerBase
{
    public readonly IBookRepoSQL repo2;
    public readonly IIssueSQL issueRepo;
    public readonly IAuthRepo authRepo;
    public UserController(IBookRepoSQL _repo, IIssueSQL _issueRepo, IAuthRepo _authRepo)
    {
        repo2 = _repo;
        issueRepo = _issueRepo;
        authRepo = _authRepo;
    }
    // public static IssueRepo issueRepo =new();
    [HttpPost("login")]
    public IActionResult Login(IUser user)
    {
        var str = authRepo.login(user.username, user.password);
        if (str == null)
            return NotFound("User not Found!");
        return Ok(str);
    }
    [HttpPost("register")]
    public IActionResult Register(IUser user)
    {
        return Ok(authRepo.register(user.username, user.password, user.role));
    }
    [Authorize]
    [HttpGet("{userId}/issues")]
    public IActionResult GetIssues(int userId)
    {
        return Ok(issueRepo.GetIssuesByUserId(userId));
    }
}