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
                            string queryAddToInvoice = "INSERT INTO invoice (no_invoice, date, totalprice) VALUES (@invoiceId, @date, @totalprice)";

                            MySqlCommand cmd = new MySqlCommand(queryAddToInvoice, conn, transaction);
                            cmd.Parameters.AddWithValue("invoiceId", generateInvoiceId);
                            cmd.Parameters.AddWithValue("date", DateTime.Now);
                            cmd.Parameters.AddWithValue("totalprice", invoice.TotalCoursePrice);

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

        [HttpGet]
        [Route("GetInvoice/{user_id:int}")]
        public IActionResult GetInvoice(int user_id)
        {
            List<DisplayInvoiceData> invoice = new List<DisplayInvoiceData>();

            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                string query = @"SELECT invoice.no_invoice as NoInvoice, invoice.date as InvoiceDates, COALESCE(totalcourse, 0) as TotalCourse, invoice.totalprice as TotalPrice FROM invoice 
                                INNER JOIN (
	                                SELECT no_invoice, COUNT(*) as totalcourse
                                    FROM checkout
                                    WHERE fk_id_user = @userId
                                    GROUP BY no_invoice
                                ) as checkout_counts
                                ON checkout_counts.no_invoice = invoice.no_invoice;";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("userId", user_id);

                cmd.ExecuteNonQuery();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {

                    invoice.Add(new DisplayInvoiceData()
                    {
                        NoInvoice = dr["NoInvoice"].ToString(),
                        InvoiceDates = dr["InvoiceDates"].ToString(),
                        TotalCourse = Convert.ToInt32(dr["TotalCourse"]),
                        TotalPrice = Convert.ToInt32(dr["TotalPrice"])
                    });
                }
            }
            return Ok(invoice);
        }

        [HttpGet]
        [Route("GetDetailInvoice/{invoice_id}")]
        public IActionResult GetDetailInvoice(string invoice_id)
        {
            DisplayDetailInvoiceData displayDetailInvoiceData = new DisplayDetailInvoiceData();
            List<InvoiceCourseList> courseList = new List<InvoiceCourseList>();
            using (MySqlConnection conn = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();

                string queryGetInvoiceInformation = @"SELECT invoice.no_invoice AS NoInvoice, 
                                                    invoice.date AS InvoiceDate, invoice.totalprice AS TotalPrice
                                                    FROM invoice
                                                    WHERE invoice.id = @invoiceId";
                MySqlCommand cmd = new MySqlCommand(queryGetInvoiceInformation, conn);

                cmd.Parameters.AddWithValue("invoiceId", invoice_id);
                cmd.ExecuteNonQuery();

                string queryGetRelatedCheckoutByNoInvoice = @"SELECT course.title AS CourseTitle, category.category_name AS CategoryName, 
                                                                checkout.schedule AS ScheduledCourse, course.price AS CoursePrice
                                                                FROM checkout
                                                                INNER JOIN course ON checkout.fk_id_course = course.id
                                                                INNER JOIN category ON course.idcategory = category.id
                                                                WHERE checkout.no_invoice = @noInvoice";
                MySqlCommand cmd2 = new MySqlCommand(queryGetRelatedCheckoutByNoInvoice, conn);
                cmd2.Parameters.AddWithValue("noInvoice", invoice_id);
                cmd2.ExecuteNonQuery();

                // Input all data from queryGetRelatedCheckoutByNoInvoice to InvoiceCourseList
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd2);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    courseList.Add(new InvoiceCourseList()
                    {
                        CourseName = dr["CourseTitle"].ToString(),
                        Category = dr["CategoryName"].ToString(),
                        ScheduledCourse = dr["ScheduledCourse"].ToString(),
                        CoursePrice = Convert.ToInt32(dr["CoursePrice"])
                    });
                }

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        displayDetailInvoiceData.NoInvoice = reader.GetString("NoInvoice");
                        displayDetailInvoiceData.InvoiceDate = reader.GetString("InvoiceDate");
                        displayDetailInvoiceData.TotalPrice = reader.GetInt32("TotalPrice");
                        displayDetailInvoiceData.InvoiceCourseLists = courseList;

                    }
                }

                conn.Close();
            }

            return Ok(courseList);
        }
    }


}
