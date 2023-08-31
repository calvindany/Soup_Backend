namespace Soup_Backend.Models
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get;set; }
        public Courses Courses { get; set; }
    }
}
