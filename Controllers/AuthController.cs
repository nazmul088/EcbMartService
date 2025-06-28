using Microsoft.AspNetCore.Mvc;
using EcbAuthService.Models;
using EcbAuthService.Utils;

namespace EcbAuthService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private static readonly Dictionary<string, string> OtpStore = new();
        private readonly JwtTokenGenerator _jwt;
        private readonly UserDbContext _userDbContext;

        public AuthController(JwtTokenGenerator jwt, UserDbContext userDbContext)
        {
            _jwt = jwt;
            _userDbContext = userDbContext;
        }

        [HttpPost("request-otp")]
        public IActionResult RequestOtp([FromBody] OtpRequest request)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            OtpStore[request.PhoneNumber] = otp;

            Console.WriteLine($"OTP for {request.PhoneNumber} is {otp}");

            return Ok(new { message = "OTP sent successfully" });
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] OtpVerification request)
        {
            if (OtpStore.TryGetValue(request.PhoneNumber, out var storedOtp) &&
                storedOtp == request.Otp)
            {
                OtpStore.Remove(request.PhoneNumber);
                // Save user to database if not exists
                var user = _userDbContext.Users.FirstOrDefault(u => u.PhoneNumber == request.PhoneNumber);
                if (user == null)
                {
                    user = new User { PhoneNumber = request.PhoneNumber };
                    _userDbContext.Users.Add(user);
                    _userDbContext.SaveChanges();
                }
                var token = _jwt.GenerateToken(request.PhoneNumber);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid OTP" });
        }
    }
}
