﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using YouTrack.Management.ModelRetrain.EF;

namespace YouTrack.Management.ReTrain.Migrations
{
    [DbContext(typeof(RetrainDbContext))]
    [Migration("20220426210801_UniqueKey")]
    partial class UniqueKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.14")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("YouTrack.Management.ModelRetrain.Entities.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ProjectKey")
                        .HasColumnType("text");

                    b.Property<bool>("RetrainEnabled")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("ProjectKey")
                        .IsUnique();

                    b.ToTable("Projects");
                });
#pragma warning restore 612, 618
        }
    }
}
