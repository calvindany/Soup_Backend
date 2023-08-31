namespace Soup_Backend.Models
{
    public class Courses
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Image { get;set; }
        public ICollection<Category> Categories { get; set; }
    }
}
