using System.ComponentModel.DataAnnotations;
namespace Library.Models;
public class IUserEF
{
    [Key]
    public int userId { get; set; }
    public string? username { get; set; }
    public string? password { get; set; }
    public int issues { get; set; }
    public int role { get; set; }
    public string? token { get; set; }
}