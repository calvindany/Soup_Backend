namespace Soup_Backend.Models
{
    public class PostCheckoutToInvoice
    {
        public int? UserId { get; set; }
        public string? NoInvoice { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Schedule { get; set; }
        public List<int> CourseId { get; set; }
    }
}
