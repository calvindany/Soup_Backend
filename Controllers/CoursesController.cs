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


        [HttpGet]
        [Route("")]
        public IActionResult GetAllCourse() {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    List<Course> courses = new List<Course>();

                    string query = "SELECT * FROM course";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                        
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            courses.Add(new Course()
                            {
                                Title = reader.GetString("title"),
                                Description = reader.GetString("description"),
                                Price = reader.GetInt32("price"),
                                Image = reader.GetString("image"),
                                IdCategory = reader.GetInt32("idcategori")
                            });
                        }
                    }

                    conn.Close();
                    return Ok(courses);
                } 
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
