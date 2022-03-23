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
        public async Task<ServiceResponse<User>> Register(UserViewModel usermodel)
        {
            var response = new ServiceResponse<User>();
            var user = await context.Users.FirstOrDefaultAsync(p=>p.Username == usermodel.UserName);
            if (user == null)
            {
                user = new User();
                response.Data = user;
                CreatePasswordHash(usermodel.Password, out byte[] passwordhash, out byte[] passwordsalt);
                user.Username = usermodel.UserName;
                user.PasswordHash = passwordhash;
                user.PasswordSalt = passwordsalt;
                user.RolesId = usermodel.RolesId;
                user.Roles = await context.Roles.FirstOrDefaultAsync(x=>x.Id == user.RolesId);
                response.Message = "Regist successfull";
                context.Users.Add(user);
                await context.SaveChangesAsync();
                
            }
            else
            {
                response.Data = null;
                response.Success = false;
                response.Message = "Sorry, the user already exists.";
            }

            return response;
        }

        public async Task<string> Login(UserViewModel usermodel)
        {
            var user = await context.Users.FirstOrDefaultAsync(p => p.Username == usermodel.UserName);
            if (user.Username != usermodel.UserName)
            {
                return null;
            }
            if (!VerifyPasswordHash(usermodel.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            user.Roles = await context.Roles.FirstOrDefaultAsync(x => x.Id == user.RolesId);
            string token = CreateToken(user);
            return token;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Roles.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
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
    }
}
