using System;
using System.Threading.Tasks;
using DatingApp.API.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;
        public AuthRepository(DataContext ctx)
        {
            context = ctx;
        }
        public async Task<User> login(string username, string password)
        {
            var user = await context.users.FirstOrDefaultAsync(x =>x.Username==username);

            if (user == null) return null;
            
            if (!verifyPassword(password, user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        private bool verifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                
                byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i=0; i < computedHash.Length; i++){
                    if(computedHash[i] != passwordHash[i]) return false;
                }

            }
            return true;
        }

        public async Task<User> register(User user, string password)
        {
            byte[] passwordHash , passwordSalt;
            createPasswordHash (password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            await context.users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> userExists(string username)
        {
            if (await context.users.AnyAsync(x=> x.Username == username)){
                return true;
            }

            return false;
        }
    }
}