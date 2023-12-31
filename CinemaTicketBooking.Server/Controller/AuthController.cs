﻿using CinemaTicketBooking.Server.Scaffolds.Models.DataLayer.Contracts;
using CinemaTicketBooking.Server.Scaffolds.Models.EntityLayer;
using CinemaTicketBooking.Server.Scaffolds.Models.ModelLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketBooking.Server.Controller
{
    [ApiController]
    [Route("api/[controller]")]  // This sets the base route for this controller to "api/auth"
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IUserRepository userRepository;
        public AuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            this.userRepository = userRepository;
            _configuration = configuration;
        }
        // Endpoint for registration: api/auth/register
        [HttpPost("registerCustomer")]
        public async Task<ActionResult<Users>> RegisterAsyncCustomer(RegistrationRequestModel model)
        {
            try
            {
                if (userRepository == null)
                {
                    Console.WriteLine("userRepository is null");
                    return StatusCode(500, "Internal Server Error");
                }
                // Check if the request is valid
                if (string.IsNullOrWhiteSpace(model.Username) ||
                    string.IsNullOrWhiteSpace(model.Password) ||
                    (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.PhoneNumber)))
                {
                    return BadRequest("Please fill in all required fields.");
                }


                // Check if the username already exists
                var existingUser = await userRepository.FindByUsername(model.Username);
                if (existingUser != null)
                {
                    return BadRequest("Username already exists.");
                }

                // Check if the email or phone number already exists
                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    var existingEmail = await userRepository.FindByEmail(model.Email);
                    if (existingEmail != null)
                    {
                        return BadRequest("Email already exists.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    var existingPhoneNumber = await userRepository.FindByPhoneNumber(model.PhoneNumber);
                    if (existingPhoneNumber != null)
                    {
                        return BadRequest("Phone Number already exists.");
                    }
                }
                var newUser = new Users
                {
                    Username = model.Username,
                    Password = model.Password, // Note: In a production environment, you should hash the password
                    PhoneNumber = model.PhoneNumber ?? "",  // Sử dụng ?? để gán null nếu không có giá trị
                    Email = model.Email ?? "",
                    Role = RoleEnum.Customer,
                    FullName = "",  // Gán trực tiếp null nếu không có giá trị
                    Address = "",
                    Sex = "",
                };

                // Optionally, hash the password before storing it
                // newUser.SetPassword(model.Password);

                userRepository.Add(newUser); // Save the user to the database
                // Optionally, you can return additional information or a success status
                return Ok(new { message = "Registration successful", username = newUser.Username, role = newUser.Role });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);

                // Return a generic error message to the client
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("registerEmployee")]
        public async Task<ActionResult<Users>> RegisterAsyncEmployee(RegistrationRequestModel model)
        {
            try
            {
                if (userRepository == null)
                {
                    Console.WriteLine("userRepository is null");
                    return StatusCode(500, "Internal Server Error");
                }
                // Check if the request is valid
                if (string.IsNullOrWhiteSpace(model.Username) ||
                    string.IsNullOrWhiteSpace(model.Password) ||
                    (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.PhoneNumber)))
                {
                    return BadRequest("Please fill in all required fields.");
                }

                // Check if the username already exists
                var existingUser = await userRepository.FindByUsername(model.Username);
                if (existingUser != null)
                {
                    return BadRequest("Username already exists.");
                }

                // Check if the email or phone number already exists
                if (!string.IsNullOrWhiteSpace(model.Email))
                {
                    var existingEmail = await userRepository.FindByEmail(model.Email);
                    if (existingEmail != null)
                    {
                        return BadRequest("Email already exists.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    var existingPhoneNumber = await userRepository.FindByPhoneNumber(model.PhoneNumber);
                    if (existingPhoneNumber != null)
                    {
                        return BadRequest("Phone Number already exists.");
                    }
                }
                var newUser = new Users
                {
                    Username = model.Username,
                    Password = model.Password, // Note: In a production environment, you should hash the password
                    PhoneNumber = model.PhoneNumber ?? "",  // Sử dụng ?? để gán null nếu không có giá trị
                    Email = model.Email ?? "",
                    Role = model.Role.Value,
                    FullName = "",  // Gán trực tiếp null nếu không có giá trị
                    Address = "",
                    Sex = "",

                };

                // Optionally, hash the password before storing it
                // newUser.SetPassword(model.Password);

                userRepository.Add(newUser); // Save the user to the database
                // Optionally, you can return additional information or a success status
                return Ok(new { message = "Registration successful", username = newUser.Username, role = newUser.Role });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);

                // Return a generic error message to the client
                return StatusCode(500, "Internal Server Error");
            }
        }
        // Endpoint for login: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<Users>> LoginAsync(LoginRequestModel model)
        {
            try
            {
                // Find user in the database
                var user = await userRepository.FindByUsernameOrPhoneNumber(model.Username);
 
                // Kiểm tra xem người dùng có được tìm thấy hay không
                if (user != null)
                {
                    Console.WriteLine($"User found in repository: {user.Username}");
                }
                else
                {
                    return BadRequest("User or Phone Number not found!");
                }

                // Check the password using BCrypt.Verify
                if (model.Password == user.Password)
                {
                    string token = GenerateJwtToken(user);
                    //Console.WriteLine(token);
                    //Console.WriteLine("Login successful");
                    return Ok(new
                    {
                        message = "Login Successful",
                        username = user.Username,
                        user_id = user.Id,
                        user_role = (int)user.Role,
                        login_as = user.Role,
                        token = token
                    });
                }
                else
                {
                    return BadRequest("Wrong Password");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);

                // Return a generic error message to the client
                return StatusCode(500, "Internal Server Error");
            }
        }
        // Hàm để lấy giá trị cho login_as
        private string GetLoginAsMessage(string role)
        {
            switch (role)
            {
                case "1":
                    return "Login as customer";
                case "2":
                    return "Login as staff";
                case "3":
                    return "Login as manager";
                case "4":
                    return "Login as admin";
                default:
                    return "Unknown role";
            }
        }
        private string GenerateJwtToken(Users user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        // ... (existing code)

        [HttpGet("forgot-password")]
        public async Task<ActionResult> ForgotPasswordAsync(
        [FromQuery] string username,
        [FromQuery] string newPassword,
        [FromQuery] string confirmPassword)
        {
            try
            {
                var user = await userRepository.FindByUsername(username);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                // Check if new password and confirm password match
                if (newPassword != confirmPassword)
                {
                    return BadRequest("New password and confirm password do not match");
                }

                user.Password = newPassword;
                await userRepository.UpdatePassword(username, newPassword);
                return Ok(new { message = "Password reset initiated successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);

                // Return a generic error message to the client
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpGet("reset-password")]
        public async Task<ActionResult> ResetPasswordAsync(
        [FromQuery] string username,
        [FromQuery] string oldPassword,
        [FromQuery] string newPassword,
        [FromQuery] string confirmPassword)
        {
            try
            {
                var user = await userRepository.FindByUsername(username);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                // Check if old password matches
                if (oldPassword != user.Password)
                {
                    return BadRequest("Incorrect old password");
                }

                // Check if new password and confirm password match
                if (newPassword != confirmPassword)
                {
                    return BadRequest("New password and confirm password do not match");
                }

                // Update the password with the new one
                user.Password = newPassword;
                await userRepository.UpdatePassword(username, newPassword);

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex.Message);

                // Return a generic error message to the client
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
