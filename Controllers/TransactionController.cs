using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Soup_Backend.Logic;
using Soup_Backend.Models;
using System.Data;

namespace Soup_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IConfiguration _configuration;

        public TransactionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("CheckoutCourse")]
        public IActionResult PostCheckout([FromBody] Checkout checkout)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = "INSERT INTO checkout (fk_id_user, fk_id_course, schedule) VALUES (@userId, @courseId, @schedule)";
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("userId", checkout.UserId);
                    cmd.Parameters.AddWithValue("courseId", checkout.CourseId);
                    cmd.Parameters.AddWithValue("schedule", checkout.Schedule);

                    int result = cmd.ExecuteNonQuery();

                    conn.Close();

                    if (result > 0)
                    {
                        return Ok("Success");
                    }

                    return Ok("Nothing Happen");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCheckout/{user_id:int}")]
        public IActionResult GetCheckout(int user_id)
        {
            try
            {
                List<DisplayCheckoutData> checkoutData = new List<DisplayCheckoutData>();
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    string query = @"SELECT Course.title as CourseTitle, Course.price as CoursePrice, 
                                    Course.image as CourseImage, Checkout.schedule as Schedule, 
                                    Category.category_name as CategoryName FROM Course 
                                    INNER JOIN Checkout ON Course.id = Checkout.fk_id_course
                                    INNER JOIN Category ON Category.id = Course.idcategory
                                    WHERE Checkout.fk_id_user = @user_id";

                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("user_id", user_id);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);

                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    foreach (DataRow dr in dt.Rows)
                    {
                        checkoutData.Add(new DisplayCheckoutData()
                        {
                            Category = dr["CategoryName"].ToString(),
                            Title = dr["CourseTitle"].ToString(),
                            Schedule = dr["Schedule"].ToString(),
                            Price = dr["CoursePrice"].ToString(),
                        });
                    }
                }
                return Ok(checkoutData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("OrderCourse")]
        public IActionResult OrderCourse([FromBody] PostCheckoutToInvoice invoice)
        {
            try
            {
                TransactionLogic transactionLogic = new TransactionLogic();
                using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string generateInvoiceId = transactionLogic.GenerateInvoiceId();
                            string queryAddToInvoice = "INSERT INTO invoice (noinvoice, date) VALUES (@invoiceId, @date)";

                            MySqlCommand cmd = new MySqlCommand(queryAddToInvoice, conn, transaction);
                            cmd.Parameters.AddWithValue("invoiceId", generateInvoiceId);
                            cmd.Parameters.AddWithValue("date", DateTime.Now);

                            cmd.ExecuteNonQuery();

                            for (int i = 0; i < invoice.CourseId.LongCount(); i++)
                            {
                                string queryEditCheckout = "UPDATE checkout SET no_invoice=@invoiceId WHERE fk_id_user=@userId AND fk_id_course=@courseId";

                                MySqlCommand cmd2 = new MySqlCommand(queryEditCheckout, conn, transaction);
                                cmd2.Parameters.AddWithValue("invoiceId", generateInvoiceId);
                                cmd2.Parameters.AddWithValue("userId", invoice.UserId);
                                cmd2.Parameters.AddWithValue("courseId", invoice.CourseId[i]);
                                cmd2.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            conn.Close();
                            return Ok("Data Berhasil Di Input");
                        }
                        catch (Exception ex) { BadRequest(ex.Message); }
                    }
                    conn.Close();
                }
                return Ok("Nothing Happen");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
