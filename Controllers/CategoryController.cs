using Microsoft.AspNetCore.Mvc;
using MySql.Data;
using MySql.Data.MySqlClient;
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

                    cmd.ExecuteNonQuery();

                    conn.Close();

                    return Ok("Operation Success");
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

                    cmd.ExecuteNonQuery();

                    conn.Close();

                    return Ok("Data Updated");
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

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    return Ok(query);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
