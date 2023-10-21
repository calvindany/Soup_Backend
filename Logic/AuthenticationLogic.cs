using System.Text;

namespace Soup_Backend.Logic
{
    public class AuthenticationLogic
    {
        private IConfiguration _configuration;
        public AuthenticationLogic(IConfiguration configuration) {
            _configuration = configuration;
        }
        public string GenerateJWTBearer()
        {
            var key = Encoding.ASCII.GetBytes();
            return "";
        }
    }
}
