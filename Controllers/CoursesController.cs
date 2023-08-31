using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Soup_Backend.Models;

namespace Soup_Backend.Controllers
{   
    [Route("api/[controller]")]
    [ApiController] 
    public class CoursesController : Controller
    {
        private readonly IConfiguration _configuration;

        public CoursesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /*[Route("GetAllCourse")]*/
        /*ublic IActionResult GetAllCourse() {
            return null;
        }*/

        [HttpPost]
        [Route("PostCourse")]
        public IActionResult PostCourse([FromBody] Course course)
        {
            List<Course> courses = new List<Course>();
            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                string query = "INSERT INTO course (title, description, price, image, idcategory) VALUES (@title, @description, @price, @image, @idcategory)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("title", course.Title);
                cmd.Parameters.AddWithValue("description", course.Description);
                cmd.Parameters.AddWithValue("price", course.Price);
                cmd.Parameters.AddWithValue("image", course.Image);
                cmd.Parameters.AddWithValue("idcategory", course.IdCategory);

                cmd.ExecuteNonQuery();
                conn.Close();
            }

            return Ok("Success");
        }

        
    }
}
