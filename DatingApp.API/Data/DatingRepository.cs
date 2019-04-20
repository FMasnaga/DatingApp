using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext context;

        public DatingRepository (DataContext context){
            this.context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            this.context.Add(entity);

        }

        public void Delete<T>(T entity) where T : class
        {
            this.context.Remove(entity);
        }

        public async Task<Photo> GetMainPhoto( int userId)
        {
            //Console.WriteLine ("MAIN PHOTO Count : "+ context.photos.Where(u => u.UserId == userId)Count());
           return await context.photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await context.photos.FirstOrDefaultAsync( p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await context.users.Include(p=>p.photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await this.context.users.Include(p=>p.photos).ToListAsync();
            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await this.context.SaveChangesAsync() > 0;
        }
    }
}