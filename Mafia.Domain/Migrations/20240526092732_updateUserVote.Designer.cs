﻿// <auto-generated />
using System;
using Mafia.Domain.Data.Adapters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Mafia.Domain.Migrations
{
    [DbContext(typeof(MafiaDbContext))]
    [Migration("20240526092732_updateUserVote")]
    partial class updateUserVote
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Mafia.Domain.Entities.AuditRecord", b =>
                {
                    b.Property<int>("AuditId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AuditId"));

                    b.Property<string>("AuditAction")
                        .HasColumnType("text");

                    b.Property<DateTime>("AuditDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("AuditMachineName")
                        .HasColumnType("text");

                    b.Property<int>("AuditRecordType")
                        .HasColumnType("integer");

                    b.Property<string>("AuditUserId")
                        .HasColumnType("text");

                    b.Property<string>("AuditUserName")
                        .HasColumnType("text");

                    b.Property<string>("JsonChangedData")
                        .HasColumnType("text");

                    b.Property<int>("RecordId")
                        .HasColumnType("integer");

                    b.HasKey("AuditId");

                    b.ToTable("AuditRecords");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Alfa2")
                        .HasColumnType("text");

                    b.Property<string>("Alfa3")
                        .HasColumnType("text");

                    b.Property<int>("Code")
                        .HasColumnType("integer");

                    b.Property<string>("CreatedById")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("ModifiedById")
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NameEn")
                        .HasColumnType("text");

                    b.Property<string>("NameRu")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("CurrentStageNumber")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MafiaCount")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerCount")
                        .HasColumnType("integer");

                    b.Property<int>("PlayerCurrentCount")
                        .HasColumnType("integer");

                    b.Property<string>("RoomNumber")
                        .HasColumnType("text");

                    b.Property<string>("RoomPassword")
                        .HasColumnType("text");

                    b.Property<int?>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.RoomPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("PlayerAge")
                        .HasColumnType("integer");

                    b.Property<int?>("PlayerGender")
                        .HasColumnType("integer");

                    b.Property<string>("PlayerId")
                        .HasColumnType("text");

                    b.Property<string>("PlayerName")
                        .HasColumnType("text");

                    b.Property<string>("PlayerPhoto")
                        .HasColumnType("text");

                    b.Property<bool>("RoomEnabled")
                        .HasColumnType("boolean");

                    b.Property<int>("RoomId")
                        .HasColumnType("integer");

                    b.Property<int?>("RoomRole")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomPlayers");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.RoomStage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Commisar_whore")
                        .HasColumnType("boolean");

                    b.Property<bool>("Day")
                        .HasColumnType("boolean");

                    b.Property<bool>("Doctor")
                        .HasColumnType("boolean");

                    b.Property<bool>("Mafia")
                        .HasColumnType("boolean");

                    b.Property<bool>("Nigth")
                        .HasColumnType("boolean");

                    b.Property<bool>("Putana")
                        .HasColumnType("boolean");

                    b.Property<int>("RoomId")
                        .HasColumnType("integer");

                    b.Property<int>("Stage")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomStages");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.RoomStagePlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Commisar_whore")
                        .HasColumnType("boolean");

                    b.Property<bool>("Day")
                        .HasColumnType("boolean");

                    b.Property<int>("DayCount")
                        .HasColumnType("integer");

                    b.Property<bool>("Doctor")
                        .HasColumnType("boolean");

                    b.Property<bool>("Mafia")
                        .HasColumnType("boolean");

                    b.Property<bool>("Nigth")
                        .HasColumnType("boolean");

                    b.Property<int>("PlayerId")
                        .HasColumnType("integer");

                    b.Property<bool>("Putana")
                        .HasColumnType("boolean");

                    b.Property<int>("RoomId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PlayerId");

                    b.HasIndex("RoomId");

                    b.ToTable("RoomStagePlayers");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Action")
                        .HasColumnType("text");

                    b.Property<string>("Controller")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FIOUserName")
                        .HasColumnType("text");

                    b.Property<string>("Ip")
                        .HasColumnType("text");

                    b.Property<string>("NewValue")
                        .HasColumnType("text");

                    b.Property<string>("OldValue")
                        .HasColumnType("text");

                    b.Property<string>("PatientFIO")
                        .HasColumnType("text");

                    b.Property<int?>("PatientId")
                        .HasColumnType("integer");

                    b.Property<string>("UserName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Organisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("CreatedById")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<double?>("Lattitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("ModifiedById")
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NameEn")
                        .HasColumnType("text");

                    b.Property<string>("NameKg")
                        .HasColumnType("text");

                    b.Property<string>("NameRu")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<bool>("ShowOnMap")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedById")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("ModifiedById")
                        .HasColumnType("text");

                    b.Property<DateTime>("ModifiedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NameKg")
                        .HasColumnType("text");

                    b.Property<string>("NameRu")
                        .HasColumnType("text");

                    b.Property<int?>("OrganisationId")
                        .HasColumnType("integer");

                    b.Property<string>("Patronimyc")
                        .HasColumnType("text");

                    b.Property<string>("Pin")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("OrganisationId");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Privilegios.ActionCategoryForUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NameRu")
                        .HasColumnType("text");

                    b.Property<int>("OrderByField")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ActionCategoryForUsers");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Privilegios.ActionForUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ActionCategoryForUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("NameRu")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ActionCategoryForUserId");

                    b.ToTable("ActionForUsers");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Privilegios.ControllerAndRoles", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ActionForUserId")
                        .HasColumnType("integer");

                    b.Property<string>("IdentityRoleId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ActionForUserId");

                    b.HasIndex("IdentityRoleId");

                    b.ToTable("ControllerAndRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Mafia.Domain.Entities.ApplicationUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<string>("FIO")
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<int?>("OrganisationId")
                        .HasColumnType("integer");

                    b.Property<string>("Patronymic")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<string>("Pin")
                        .HasColumnType("text");

                    b.Property<string>("Surname")
                        .HasColumnType("text");

                    b.HasIndex("OrganisationId");

                    b.HasDiscriminator().HasValue("ApplicationUser");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Country", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Mafia.Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.Room", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.RoomPlayer", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.ApplicationUser", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId");

                    b.HasOne("Mafia.Domain.Entities.Game.Room", "Room")
                        .WithMany("Players")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.RoomStage", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.Game.Room", "Room")
                        .WithMany("Stages")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.RoomStagePlayer", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.Game.RoomPlayer", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Mafia.Domain.Entities.Game.RoomStage", "Room")
                        .WithMany()
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Organisation", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Mafia.Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Position", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Mafia.Domain.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Mafia.Domain.Entities.Organisation", "Organisation")
                        .WithMany()
                        .HasForeignKey("OrganisationId");

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Privilegios.ActionForUser", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.Privilegios.ActionCategoryForUser", "ActionCategoryForUser")
                        .WithMany("ActionForUsers")
                        .HasForeignKey("ActionCategoryForUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ActionCategoryForUser");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Privilegios.ControllerAndRoles", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.Privilegios.ActionForUser", "ActionForUser")
                        .WithMany()
                        .HasForeignKey("ActionForUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", "IdentityRole")
                        .WithMany()
                        .HasForeignKey("IdentityRoleId");

                    b.Navigation("ActionForUser");

                    b.Navigation("IdentityRole");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Mafia.Domain.Entities.ApplicationUser", b =>
                {
                    b.HasOne("Mafia.Domain.Entities.Organisation", "Organisation")
                        .WithMany()
                        .HasForeignKey("OrganisationId");

                    b.Navigation("Organisation");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Game.Room", b =>
                {
                    b.Navigation("Players");

                    b.Navigation("Stages");
                });

            modelBuilder.Entity("Mafia.Domain.Entities.Privilegios.ActionCategoryForUser", b =>
                {
                    b.Navigation("ActionForUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
