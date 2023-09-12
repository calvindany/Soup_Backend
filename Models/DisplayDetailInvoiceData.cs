namespace Soup_Backend.Models
{
    public class DisplayDetailInvoiceData
    {
        public string? NoInvoice { get; set; }
        public string? InvoiceDate { get; set; }
        public int? TotalPrice { get; set; }
        public List<InvoiceCourseList>? InvoiceCourseLists { get; set; }
    }
}
