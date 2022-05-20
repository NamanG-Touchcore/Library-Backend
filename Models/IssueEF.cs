namespace Library.Models;

public class IIssueEF
{
    public int id { get; set; }
    public int bookId { get; set; }
    public int userId { get; set; }
    public int fine { get; set; }
    public string? issueDate { get; set; }
    public string? expiryDate { get; set; }
    public string? returnDate { get; set; }
    public int isActive { get; set; }
}