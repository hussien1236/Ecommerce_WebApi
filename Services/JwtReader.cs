using System.Security.Claims;
using WebApplication9.Models;

namespace WebApplication9.Services
{
    public class JwtReader
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;
            if (identity == null)
                return 0;
            var claim = identity.Claims.FirstOrDefault(c => c.Type.ToLower() == "id");
            if (claim == null)
                return 0;
            try
            {
                int id = int.Parse(claim.Value);
                return id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }

        }
        public static string GetUserRole(ClaimsPrincipal user) {
            var identity = user.Identity as ClaimsIdentity;
            if (identity == null)
                return "";
            var claim = identity.Claims.FirstOrDefault(c => c.Type.ToLower().Contains("role"));
            if (claim == null)
                return "";
            return claim.Value;
        }
        public static Dictionary<string,string> GetUserClaims(ClaimsPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;
            var results = new Dictionary<string, string>();
            if (identity != null)
            {
                var claims = identity.Claims;
                foreach (var claim in claims)
                {
                    results.Add(claim.Type, claim.Value);
                }
            }
            return results;
        }
        }
}
