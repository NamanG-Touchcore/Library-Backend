namespace Library.Models
{

public class IBook{
    public int id {get;set;}
    public string? name {get;set;}
    public string? author {get;set;}
    public string? description {get;set;}
    public int issues {get;set;}
    public int? issueId {get;set;}
    public int? isActive {get;set;}
    public string? returnDate {get;set;}
    public string? expiryDate {get;set;}
    public string? issueDate {get;set;}
    public string? coverImage {get;set;}
    public int totalQuantity {get;set;}
    public int activeIssues {get;set;}
    public int isBookActive {get;set;}
}
}