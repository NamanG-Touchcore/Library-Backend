using System;
using System.Data.SqlClient;
using Library.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Library.Repositories
{

    public class AuthRepo : IAuthRepo
    {
        public string Constr { get; set; }
        public IConfiguration configuration;
        public SqlConnection? con;
        private static string currentSalt = "ThisIsASalt";
        public AuthRepo(IConfiguration _configuration)
        {
            configuration = _configuration;
            // Constr=configuration.GetConnectionString("DbConnection");
            Constr = configuration.GetConnectionString("DbConnection");
            Console.WriteLine(Constr);
        }
        public IUser login(string username, string password)
        {
            string tempSalt = "", tempHash = "";
            using (con = new SqlConnection(Constr))
            {
                IUser user = new IUser();
                con.Open();
                var cmd = new SqlCommand($"Select * from userTable where username='{username}' ", con);
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    user.username = Convert.ToString(rdr["username"]);
                    user.password = Convert.ToString(rdr["password"]);
                    user.id = Convert.ToInt32(rdr["userId"]);
                    // bool tempPass = user.password != password;
                    // string userPassword = Convert.ToString(rdr["password"]);
                    user.role = Convert.ToInt32(rdr["role"]);
                    tempSalt = Convert.ToString(rdr["userSalt"]);
                    // var passwordSalt = Convert.FromBase64String(tempSalt);
                    // for (int i = 0; i < tempSalt.Length; i++)
                    // {
                    // var tempStr = Convert.ToByte(tempSalt[i]);
                    // user.passwordSalt.Append(Convert.ToByte(tempSalt[i]));
                    // }
                    tempHash = Convert.ToString(rdr["userHash"]);
                    // var passwordHash = Convert.FromBase64String(tempHash);
                    // for (int i = 0; i < tempSalt.Length; i++)
                    // {
                    // user.passwodHash.Append(Convert.ToByte(tempHash[i]));
                    // }
                    // if (!verifyHash(user.password, user.passwordHash, user.passwordSalt))
                    // {
                    //     // var val = verifyHash(user.password, user.passwordHash, user.passwordSalt);
                    //     return tempHash + " " + tempSalt;
                    // }

                }
                string userPassword = user.password;
                if (user.username == null)
                {
                    throw new AppException("User not found!");
                }
                // else if (user.password != password)
                else if (!verifyHash(password, Convert.FromBase64String(tempHash), Convert.FromBase64String(tempSalt)))
                {
                    throw new AppException("Wrong Password");
                }
                else
                {
                    var token = CreateToken(user);
                    user.token = token;
                    return user;
                }
                throw new AppException("Invalid User" + user.password);
            }
            return null;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes("This is a temp salt")))
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public IUser register(string username, string password, int role)
        {
            if (username == "")
                throw new AppException("Username Invalid!");
            if (password == "" || password.Length < 8)
                throw new AppException("Password Invalid");
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            string passwordHashString = Convert.ToBase64String(passwordHash);
            string passwordStaltString = Convert.ToBase64String(passwordSalt);
            byte[] temp = Convert.FromBase64String(passwordStaltString);
            // currentSalt = passwordStaltString;
            int userId = -1;
            string query = $"INSERT INTO userTable (username, role, password)  VALUES  ('{username}','{role}','{password}')";
            using (con = new SqlConnection(Constr))
            {
                con.Open();
                var cmd = new SqlCommand("signup", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@role", role);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@hash", passwordHashString);
                cmd.Parameters.AddWithValue("@salt", passwordStaltString);
                var rdr = cmd.ExecuteReader();
                rdr.Close();
                cmd = new SqlCommand($"SELECT userId from userTable where username='{username}'", con);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    userId = Convert.ToInt32(rdr["userId"]);
                }
            }
            IUser user = new IUser() { username = username, password = password, role = role, id = userId };
            var token = CreateToken(user);
            user.token = token;
            return user;
        }
        private bool verifyHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes("This is a temp salt")))
            {
                // var computedHashString = Encoding.UTF8.GetString(hmac.ComputeHash(System.Text.Convert.FromBase64String(password)));
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string CreateToken(IUser user)
        {
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, user.role==1?"Admin":"User")
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: cred
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }

    public interface IAuthRepo
    {
        public IUser login(string username, string password);
        public IUser register(string username, string password, int role);
    }
}