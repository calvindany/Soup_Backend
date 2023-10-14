namespace Soup_Backend.DTOs.DetailClass
{
    public class DisplayDetailClass
    {
        public string? Title { get; set; }
        public int? Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public List<DisplayOtherCourse>? OtherCourse { get; set; }

    }
}
