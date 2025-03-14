﻿// <auto-generated />
using System;
using ConstructionProjectManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ConstructionProjectManagement.Migrations
{
    [DbContext(typeof(ConstructionDbContext))]
    [Migration("20250223171921_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ConstructionProjectManagement.Models.ConstructionProject", b =>
                {
                    b.Property<string>("ProjectId")
                        .HasMaxLength(6)
                        .HasColumnType("character varying(6)");

                    b.Property<string>("OtherCategory")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProjectCategory")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ProjectConstructionStartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("ProjectCreatorId")
                        .HasColumnType("uuid");

                    b.Property<string>("ProjectDetails")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("ProjectLocation")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("ProjectStage")
                        .HasColumnType("integer");

                    b.HasKey("ProjectId");

                    b.HasIndex("ProjectName")
                        .IsUnique();

                    b.ToTable("ConstructionProjects");
                });

            modelBuilder.Entity("ConstructionProjectManagement.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("12345678-1234-1234-1234-1234567890ab"),
                            Email = "test@admin.com",
                            Password = "$2a$11$fjG27TwzU.jmCj0hf2hf3OxvfY5wYgX3Y10AbZTXcnttBJBmE8Z6K"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
