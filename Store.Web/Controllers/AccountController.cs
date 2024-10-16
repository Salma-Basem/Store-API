using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Data.Entities.IdentityEntities;
using Store.Service.Services.UserService;
using Store.Service.Services.UserService.Dtos;


namespace Store.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IUserService _userServices;
        private readonly UserManager<AppUser> _userManger;

        public AccountController(IUserService userServices, UserManager<AppUser> userManger)
        {
            _userServices = userServices;
            _userManger = userManger;
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto input)
        {
            var user = await _userServices.Login(input);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }
            return Ok(user);
        }

        // Register Endpoint
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto input)
        {
            var user = await _userServices.Register(input);
            if (user == null)
            {
                return BadRequest(new { message = "User already exists" });
            }
            return Ok(user);
        }

        // Get Current User Details
        [HttpGet("current-user")]
        public async Task<ActionResult<UserDto>> GetCurrentUserDetails()
        {
            var userIdClaim = User?.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            var user = await _userManger.FindByIdAsync(userIdClaim.Value);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new UserDto
            {
                Id = Guid.Parse(user.Id),
                DisplayName = user.DisplayName,
                Email = user.Email,
            });
        }
    }
}