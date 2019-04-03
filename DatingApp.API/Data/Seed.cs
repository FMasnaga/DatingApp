using System.Collections.Generic;
using DatingApp.API.Model;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext context;
        public Seed (DataContext context){
            this.context = context;
        }

        public void SeedUsers (){
            var userData = System.IO.File.ReadAllText("Data/SeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach(var user in users){

                byte[] passwordHash, passwordSalt;
                createPasswordHash("password", out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Username = user.Username.ToLower();
                this.context.users.Add(user);
            }
            this.context.SaveChanges();
        }

        private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()){
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}