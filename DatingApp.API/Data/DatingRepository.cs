using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await this.context.likes.FirstOrDefaultAsync(u => u.HisId== userId && u.HerId == recipientId);
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

        public async Task<PageList<User>> GetUsers(UserParams userParams)
        {
            var users =  this.context.users.Include(p=>p.photos).OrderByDescending(u=> u.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            Console.WriteLine("USER ID :"+userParams.UserId+" "+ userParams.UserLike);
            if (userParams.UserLike){
                var userILike = await GetUserIds (userParams.UserId, userParams.LikeMe);
                users = users.Where (u => userILike.Contains(u.Id));
            } 

            if (userParams.LikeMe){
                var userLikeMe = await GetUserIds (userParams.UserId, userParams.LikeMe);
                users = users.Where (u => userLikeMe.Contains(u.Id));
            }


            if (userParams.MaxAge != 18 || userParams.MaxAge != 99){
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
                var minDob = DateTime.Today.AddYears (-userParams.MaxAge-1);
                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }
            
            if (!string.IsNullOrEmpty(userParams.OrderBy)){
                
                switch (userParams.OrderBy){
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PageList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<IEnumerable<int>> GetUserIds (int id, bool LikeMe){
            var user = await this.context.users.Include(x=>x.LikeHers).Include(x=>x.LikeHims).FirstOrDefaultAsync(u => u.Id == id);
            
            if (LikeMe){
                return user.LikeHims.Where(x=>x.HerId==id).Select(l => l.HisId);
            }
            return user.LikeHers.Where(x=>x.HisId==id).Select(l => l.HerId);
        }
        

        public async Task<bool> SaveAll()
        {
            return await this.context.SaveChangesAsync() > 0;
        }
    }
}