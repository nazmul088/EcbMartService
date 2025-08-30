using Microsoft.AspNetCore.Mvc;
using EcbMartService.Models;
using EcbMartService.Utils;
using EcbMartService.Data;
using EcbAuthService.Models;
using EcbMartService.Services;

namespace EcbMartService.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private static readonly Dictionary<string, string> OtpStore = new();
        private readonly JwtTokenGenerator _jwt;
        private readonly UserDbContext _userDbContext;
        private readonly ISmsSender _smsSender;

        public AuthController(JwtTokenGenerator jwt, UserDbContext userDbContext, ISmsSender smsSender)
        {
            _jwt = jwt;
            _userDbContext = userDbContext;
            _smsSender = smsSender;
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] OtpRequest request)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            OtpStore[request.PhoneNumber] = otp;

            Console.WriteLine($"OTP for {request.PhoneNumber} is {otp}");
            // await _smsSender.SendSmsAsync(request.PhoneNumber, $"Your code is {otp}");

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
                var token = _jwt.GenerateToken(request.PhoneNumber);
                if (user == null)
                {
                    user = new User { PhoneNumber = request.PhoneNumber, AuthToken = token };
                    _userDbContext.Users.Add(user);
                    _userDbContext.SaveChanges();
                }
                
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid OTP" });
        }

        [HttpPost("logOut")]
        [RequireAuth]
        public IActionResult LogOutUser([FromBody] LogOut request)
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
            var user = _userDbContext.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }
            user.AuthToken = null;
            _userDbContext.SaveChanges();
            return Ok(new { message = "Logged out successfully" });
        }
    }
}
