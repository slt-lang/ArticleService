﻿// <auto-generated />
using System;
using ArticleService.Adapters.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArticleService.Adapters.Migrations
{
    [DbContext(typeof(ArticleServiceContext))]
    [Migration("20250325144504_M3")]
    partial class M3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ArticleService.Adapters.Database.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .IsUnicode(true)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.HasIndex("CreateDate");

                    b.HasIndex("Id");

                    b.HasIndex("Name");

                    b.ToTable("Articles", "ArticleService");
                });

            modelBuilder.Entity("ArticleService.Adapters.Database.Models.ArticleHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<string>("Content")
                        .IsRequired()
                        .IsUnicode(true)
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("CreateDate");

                    b.HasIndex("Id");

                    b.ToTable("ArticleHistory", "ArticleService");
                });

            modelBuilder.Entity("ArticleService.Adapters.Database.Models.ArticleVisits", b =>
                {
                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("Visits")
                        .HasColumnType("bigint");

                    b.HasKey("ArticleId", "Date");

                    b.HasIndex("ArticleId");

                    b.HasIndex("Date");

                    b.ToTable("ArticleVisits", "ArticleService");
                });

            modelBuilder.Entity("ArticleService.Adapters.Database.Models.ArticleHistory", b =>
                {
                    b.HasOne("ArticleService.Adapters.Database.Models.Article", null)
                        .WithMany("History")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ArticleService.Adapters.Database.Models.ArticleVisits", b =>
                {
                    b.HasOne("ArticleService.Adapters.Database.Models.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");
                });

            modelBuilder.Entity("ArticleService.Adapters.Database.Models.Article", b =>
                {
                    b.Navigation("History");
                });
#pragma warning restore 612, 618
        }
    }
}
