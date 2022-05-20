using System.ComponentModel.DataAnnotations;
namespace Library.Models
{

    public class IBookEF
    {
        [Key]
        public int? bookId { get; set; }
        public string? name { get; set; }
        public string? author { get; set; }
        public string? description { get; set; }
        public int issues { get; set; }
        public string? coverImage { get; set; }
        public int totalQuantity { get; set; }
        public int activeIssues { get; set; }
        public int isBookActive { get; set; }
    }
}