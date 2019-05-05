﻿// <auto-generated />
using System;
using DatingApp.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DatingApp.API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("DatingApp.API.Model.Like", b =>
                {
                    b.Property<int>("HerId");

                    b.Property<int>("HisId");

                    b.HasKey("HerId", "HisId");

                    b.HasIndex("HisId");

                    b.ToTable("likes");
                });

            modelBuilder.Entity("DatingApp.API.Model.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("Description");

                    b.Property<bool>("IsMain");

                    b.Property<string>("PublicId");

                    b.Property<string>("Url");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("photos");
                });

            modelBuilder.Entity("DatingApp.API.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Country");

                    b.Property<DateTime>("Created");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Gender");

                    b.Property<string>("Interest");

                    b.Property<string>("Introduction");

                    b.Property<string>("KnownAs");

                    b.Property<DateTime>("LastActive");

                    b.Property<string>("LookingFor");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("Username");

                    b.Property<string>("city");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("DatingApp.API.Model.Value", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("values");
                });

            modelBuilder.Entity("DatingApp.API.Model.Like", b =>
                {
                    b.HasOne("DatingApp.API.Model.User", "Her")
                        .WithMany("LikeHims")
                        .HasForeignKey("HerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DatingApp.API.Model.User", "His")
                        .WithMany("LikeHers")
                        .HasForeignKey("HisId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DatingApp.API.Model.Photo", b =>
                {
                    b.HasOne("DatingApp.API.Model.User", "User")
                        .WithMany("photos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
