using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Soup_Backend.DTOs.DetailClass;
using Soup_Backend.Models;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Xml;

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
                                Id = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                Description = reader.GetString("description"),
                                Price = reader.GetInt32("price"),
                                Image = reader.GetString("image"),
                                IdCategory = reader.GetInt32("idcategory")
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

        // This for Detail Course Page API
        [HttpGet]
        [Route("GetCourseById/{course_id:int}")]
        public IActionResult GetCourseById(int course_id) {
            DisplayDetailClass displayDetailClass = null;
            List<DisplayOtherCourse> displayOtherCourses = new List<DisplayOtherCourse>();

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                string queryGetSelectedClass = "SELECT * FROM course WHERE course.id=@idCourse";
                string queryGetOtherClass = "SELECT * FROM course WHERE NOT course.id = @idCourse";

                MySqlCommand cmdGetOtherCLass = new MySqlCommand(queryGetOtherClass, conn);
                cmdGetOtherCLass.Parameters.AddWithValue("idCourse", course_id);

                using(MySqlDataReader reader = cmdGetOtherCLass.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        displayOtherCourses.Add(new DisplayOtherCourse()
                        {
                            Id = reader.GetInt32("id"),
                            Title = reader.GetString("title"),
                            Price = reader.GetInt32("price"),
                            Image = reader.GetString("image"),
                        });
                    }
                }

                MySqlCommand cmdGetSelectedClass = new MySqlCommand(queryGetSelectedClass, conn);
                cmdGetSelectedClass.Parameters.AddWithValue("idCourse", course_id);

                using (MySqlDataReader reader = cmdGetSelectedClass.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        displayDetailClass = new DisplayDetailClass()
                        {
                            Title = reader.GetString("title"),
                            Price = reader.GetInt32("price"),
                            Image = reader.GetString("image"),
                            Description = reader.GetString("description"),
                            OtherCourse = displayOtherCourses,
                        };
                    }
                }

                conn.Close();
            }

            
            if (displayDetailClass == null)
            {
                return Ok("Nothing happen");
            
            }
            return Ok(displayDetailClass);

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

        [HttpPost]
        [Route("EditCourse/{course_id:int}")]

        public IActionResult EditCourse([FromBody] Course course, int course_id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string query = "UPDATE course SET title=@new_title, description=@new_description, price=@new_price, image=@new_image, idcategory=@new_idcategory";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    int result = cmd.ExecuteNonQuery();

                    cmd.Parameters.AddWithValue("new_title", course.Title);
                    cmd.Parameters.AddWithValue("new_description", course.Description);
                    cmd.Parameters.AddWithValue("new_price", course.Price);
                    cmd.Parameters.AddWithValue("new_image", course.Image);
                    cmd.Parameters.AddWithValue("idcategory", course.IdCategory);

                    if( result > 0)
                    {
                        return Ok("Data behasil diubah");
                    }
                }

                return Ok("Nothing Happen");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCourse")]
        public IActionResult DeleteCourse(int course_id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"))) {
                    conn.Open();
                    string query = "DELETE FROM course WHERE id=@course_id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("course_id", course_id);

                    int result = cmd.ExecuteNonQuery();

                    conn.Close();
                    if (result > 0)
                    {
                        return Ok("Course Deleted");
                    }

                    return Ok("Course Not Found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
    }
}
