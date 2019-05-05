using DatingApp.API.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {
            
        }

        public DbSet<Value> values {get; set;}
        public DbSet<User> users {get;set;}
        public DbSet <Photo> photos {get;set;}

        public DbSet <Like> likes {get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder){

            modelBuilder.Entity<Like>()
                .HasKey(k => new{k.HerId,k.HisId});

            modelBuilder.Entity<Like>()
                .HasOne(k => k.His)
                .WithMany(k => k.LikeHers)
                .HasForeignKey(k => k.HisId)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Like>()
                .HasOne(k => k.Her)
                .WithMany(k => k.LikeHims)
                .HasForeignKey(k => k.HerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}