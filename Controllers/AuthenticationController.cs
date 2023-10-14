using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Soup_Backend.DTOs.Login;
using Soup_Backend.DTOs.Regist;

namespace Soup_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration) { 
            _configuration = configuration;
        }

        [HttpPost]
        [Route("/Login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            string query = "SELECT * FROM user WHERE email = @userEmail";
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnections")))
                {
                    conn.Open();
                    
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("userEmail", loginRequest.Email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        string email = null;
                        string password = null;

                        if(reader.Read())
                        {   
                            email = reader.GetString("email");
                            password = reader.GetString("password");
                        }

                        if(email != null && password != null)
                        {
                            if(loginRequest.Password == password)
                            {
                                return Ok(true);
                            }
                        }
                    }

                    conn.Close();
                    return Ok("Nothing happen");
                }
            } catch(Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Route("/Regist")]
        public IActionResult Regist([FromBody] RegistRequest registRequest) {
            string query = "INSERT INTO user VALUES (DEFAULT, @name, @email, @password";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Parameters.AddWithValue("nama", registRequest.Name);
                    cmd.Parameters.AddWithValue("email", registRequest.Email);
                    cmd.Parameters.AddWithValue("password", registRequest.Password);

                    var result = cmd.ExecuteNonQuery();

                    if(result != 0)
                    {
                        return Ok("Success Registration");
                    }

                    conn.Close();
                    return Ok("Not Inserted");
                }
            }
            catch(Exception e) {
                return BadRequest(e);
            }
        }
    }
}
