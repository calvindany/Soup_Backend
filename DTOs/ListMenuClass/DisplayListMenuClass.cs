using Soup_Backend.Models;

namespace Soup_Backend.DTOs.ListMenuClass
{
    public class DisplayListMenuClass
    {
        public string? Category_Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public List<DisplayCoursesFromCategory>? RelatedCourse  { get; set; }
    }
}
