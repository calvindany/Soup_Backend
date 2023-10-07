using Microsoft.AspNetCore.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
using Soup_Backend.DTOs.ListMenuClass;
using Soup_Backend.Models;

namespace Soup_Backend.Controllers
{
    [Route("api/[Controller]")]
    public class CategoryController : Controller
    {

        private readonly IConfiguration _configuration;

        public CategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetCategory()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    List<Category> categories = new List<Category>();
                    string query = "SELECT * FROM category";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using(MySqlDataReader  reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category()
                            {
                                Title = reader.GetString("category_name"),
                                Description = reader.GetString("description"),
                                Image = reader.GetString("image")
                            });
                        } 
                    }
                    conn.Close();
                    return Ok(categories);
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCategoryById/{category_id:int}")]
        public IActionResult GetCategoryById(int category_id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    DisplayListMenuClass displayListMenuClass = null;
                    List<DisplayCoursesFromCategory> relatedCourse = new List<DisplayCoursesFromCategory>();

                    string query = "SELECT * FROM category WHERE id=@categoryId";
                    string query2 = "SELECT * FROM course WHERE idcategory=@categoryId";


                    MySqlCommand cmdGetRelatedCourseInfo = new MySqlCommand(query2, conn);

                    cmdGetRelatedCourseInfo.Parameters.AddWithValue("categoryId", category_id);

                    using (MySqlDataReader reader = cmdGetRelatedCourseInfo.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            relatedCourse.Add(new DisplayCoursesFromCategory()
                            {
                                Title = reader.GetString("title"),
                                Price = reader.GetInt32("price"),
                                Image = reader.GetString("image"),
                            });
                        }
                    }

                    MySqlCommand cmdGetCategoryInfo = new MySqlCommand(query, conn);

                    cmdGetCategoryInfo.Parameters.AddWithValue("categoryId", category_id);

                    using (MySqlDataReader reader = cmdGetCategoryInfo.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            displayListMenuClass = new DisplayListMenuClass() {
                                Category_Name = reader.GetString("category_name"),
                                Description = reader.GetString("description"),
                                Image = reader.GetString("image"),
                                RelatedCourse = relatedCourse,
                            };
                        }

                    }
                    conn.Close();
                    return Ok(displayListMenuClass);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PostCategory")]
        public IActionResult PostCategory([FromBody] Category category)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string query = "INSERT INTO category (id, category_name, description, image) VALUES (DEFAULT, @category_name, @description, @image)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("category_name", category.Title);
                    cmd.Parameters.AddWithValue("description", category.Description);
                    cmd.Parameters.AddWithValue("image", category.Image);

                    int result = cmd.ExecuteNonQuery();
               
                    conn.Close();

                    if(result > 0)
                    {
                        return Ok("Success Add Data");
                    }

                    return Ok("Nothing Happen");
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("EditCategory/{category_id:int}")]
        public IActionResult EditCategory([FromBody] Category category, int category_id)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string query = "UPDATE category SET category_name=@new_category_name, description=@description, image=@image WHERE id=@id_category";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("id_category", category_id);
                    cmd.Parameters.AddWithValue("new_category_name", category.Title);
                    cmd.Parameters.AddWithValue("description", category.Description);
                    cmd.Parameters.AddWithValue("image", category.Image);

                    int result = cmd.ExecuteNonQuery();

                    conn.Close();

                    if (result > 0)
                    {
                        return Ok("Success Add Data");
                    }

                    return Ok("Nothing Happen");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCategory")]
        public IActionResult DeleteCategory(int category_id)
        {
            try
            {

                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    string query = "DELETE FROM category WHERE id=@id_category";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("id_category", category_id);

                    int result = cmd.ExecuteNonQuery();

                    conn.Close();

                    if (result > 0)
                    {
                        return Ok("Success Add Data");
                    }

                    return Ok("Nothing Happen");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
