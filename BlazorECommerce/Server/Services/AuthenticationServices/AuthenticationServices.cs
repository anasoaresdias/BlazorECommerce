using Microsoft.Extensions.Configuration;
using System.Linq;

namespace BlazorECommerce.Server.Services.AuthenticationServices
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly DataContext context;
        public readonly IConfiguration configuration;

        public AuthenticationServices(DataContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        public async Task<ServiceResponse<int>> Register(UserViewModel usermodel)
        {
            if (await UserExists(usermodel.Email, usermodel.UserName))
            {
                return new ServiceResponse<int> 
                { 
                    Success = false, 
                    Message = "User already exists!" 
                };
            }
            var user = new User();
            CreatePasswordHash(usermodel.Password, out byte[] passwordhash, out byte[] passwordsalt);
            user.PasswordHash = passwordhash;
            user.PasswordSalt = passwordsalt;
            user.Username = usermodel.UserName;
            user.FirstName = usermodel.FirstName;
            user.LastName = usermodel.LastName;
            user.Email = usermodel.Email;
            user.DateCreated = DateTime.Now;
            user.RolesId = usermodel.RolesId;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return new ServiceResponse<int> { 
                Data = user.Id,
                Message = "Registration Successful!"
            };
        }

        public async Task<bool> UserExists(string email, string username)
        {
            if(await context.Users.AnyAsync( x => 
                x.Username.ToLower().Equals(username.ToLower()) 
                || x.Email.ToLower().Equals(email.ToLower())) )
            {
                return true;
            }
            return false;
        }

        public async Task<ServiceResponse<string>> Login(UserLoginDTO usermodel)
        {
            var response = new ServiceResponse<string>();
            var user = await context.Users
                .FirstOrDefaultAsync(x => 
                    x.Username
                    .ToLower()
                    .Equals(usermodel.UserName.ToLower())
                    );
            if(user == null)
            {
                response.Success = false;
                response.Message = "User not found";
            }
            else if(!VerifyPasswordHash(usermodel.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong password";
            }
            else
            {
                user.Roles = user.Roles = await context.Roles.FirstOrDefaultAsync(x => x.Id == user.RolesId);
                response.Data = CreateToken(user);
                response.Success = true;
            }
            return response;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Roles.Name)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordhash, out byte[] passwordsalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordsalt = hmac.Key;
                passwordhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passowrhash, byte[] passordsalt)
        {
            using (var hmac = new HMACSHA512(passordsalt))
            {
                var computedhash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedhash.SequenceEqual(passowrhash);
            }
        }

        public async Task<ServiceResponse<bool>> ChangePassword(int userid, string newpassword)
        {
            var user = await context.Users.FindAsync(userid);
            if (user == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "User Not found!"
                };
            }

            CreatePasswordHash(newpassword, out byte[] passwordbash, out byte[] passwordsalt);
            user.PasswordHash = passwordbash;
            user.PasswordSalt = passwordsalt;

            await context.SaveChangesAsync();

            return new ServiceResponse<bool>
            {
                Data = true,
                Success = true,
                Message = "Password has been changed."
            };
        }
    }
}
