namespace Soup_Backend.Logic
{
    public class TransactionLogic
    {
        public string GenerateInvoiceId()
        {
            Random random = new Random();
            int number = random.Next(0, 9999);


            string invoiceId = "SOUP";

            if (number < 10)
            {
                invoiceId += "000" + number.ToString();
            }
            else if (number < 100)
            {
                invoiceId += "00" + number.ToString();
            }
            else if (number < 1000)
            {
                invoiceId += "0" + number.ToString();
            }
            else
            {
                invoiceId += "" + number.ToString();

            }

            return invoiceId;
        }
    }

    public int CountCoursePerInvoice()
    {

    }
}
