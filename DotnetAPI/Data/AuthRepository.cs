using System;
using System.Threading.Tasks;
using dotnet_app.Models;
using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data {
    public class AuthRepository : IAuthRepository {
        public DataContext _context { get; }
        public AuthRepository (DataContext context) {
            _context = context;
        }

        public async Task<User> Login (string username, string password) {
            var user = await _context.Users.FirstOrDefaultAsync (x => x.userName == username);
            if (user == null)
                return null;
            if (!VerifyPasswordHash (password, user.Passwordhash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash (string password, byte[] passwordhash, byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512 (passwordSalt)) {
                var computedHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordhash[i])
                        return false;
                }
                return true;
            }
        }

        public async Task<User> Register (User user, string password) {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash (password, out passwordHash, out passwordSalt);
            user.PasswordSalt = passwordSalt;
            user.Passwordhash = passwordHash;

            await _context.Users.AddAsync (user);
            await _context.SaveChangesAsync ();

            return user;
        }

        private void CreatePasswordHash (string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512 ()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash (System.Text.Encoding.UTF8.GetBytes (password));
            }
        }

        public async Task<bool> UserExist (string username) {
            if (await _context.Users.AnyAsync (x => x.userName == username))
                return true;
            return false;
        }
    }
}