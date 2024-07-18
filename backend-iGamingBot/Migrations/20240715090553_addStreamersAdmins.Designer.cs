﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using backend_iGamingBot.Infrastructure.Configs;

#nullable disable

namespace backend_iGamingBot.Migrations
{
    [DbContext(typeof(AppCtx))]
    [Migration("20240715090553_addStreamersAdmins")]
    partial class addStreamersAdmins
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DefaultUserRaffle", b =>
                {
                    b.Property<long>("ParticipantRafflesId")
                        .HasColumnType("bigint");

                    b.Property<long>("ParticipantsId")
                        .HasColumnType("bigint");

                    b.HasKey("ParticipantRafflesId", "ParticipantsId");

                    b.HasIndex("ParticipantsId");

                    b.ToTable("DefaultUserRaffle");
                });

            modelBuilder.Entity("DefaultUserRaffle1", b =>
                {
                    b.Property<long>("WinnerRafflesId")
                        .HasColumnType("bigint");

                    b.Property<long>("WinnersId")
                        .HasColumnType("bigint");

                    b.HasKey("WinnerRafflesId", "WinnersId");

                    b.HasIndex("WinnersId");

                    b.ToTable("DefaultUserRaffle1");
                });

            modelBuilder.Entity("DefaultUserStreamer", b =>
                {
                    b.Property<long>("AdminsId")
                        .HasColumnType("bigint");

                    b.Property<long>("NegotiableId")
                        .HasColumnType("bigint");

                    b.HasKey("AdminsId", "NegotiableId");

                    b.HasIndex("NegotiableId");

                    b.ToTable("DefaultUserStreamer");
                });

            modelBuilder.Entity("backend_iGamingBot.Config", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("ExpirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<object>("Payload")
                        .IsRequired()
                        .HasColumnType("json");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("backend_iGamingBot.DefaultUser", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("character varying(13)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("TgId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DefaultUser");

                    b.HasDiscriminator().HasValue("DefaultUser");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("backend_iGamingBot.Raffle", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("AmountOfWinners")
                        .HasColumnType("integer");

                    b.Property<long>("CreatorId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RaffleConditions")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("ShouldNotifyUsers")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowWinners")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Raffles");
                });

            modelBuilder.Entity("backend_iGamingBot.Subscriber", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("StreamerId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("SubscribeTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "StreamerId");

                    b.HasIndex("StreamerId");

                    b.ToTable("Subscribers");
                });

            modelBuilder.Entity("backend_iGamingBot.Streamer", b =>
                {
                    b.HasBaseType("backend_iGamingBot.DefaultUser");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Socials")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("TgId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("Streamer");
                });

            modelBuilder.Entity("backend_iGamingBot.User", b =>
                {
                    b.HasBaseType("backend_iGamingBot.DefaultUser");

                    b.Property<string>("PayMethods")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("TgId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("User");
                });

            modelBuilder.Entity("DefaultUserRaffle", b =>
                {
                    b.HasOne("backend_iGamingBot.Raffle", null)
                        .WithMany()
                        .HasForeignKey("ParticipantRafflesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_iGamingBot.DefaultUser", null)
                        .WithMany()
                        .HasForeignKey("ParticipantsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DefaultUserRaffle1", b =>
                {
                    b.HasOne("backend_iGamingBot.Raffle", null)
                        .WithMany()
                        .HasForeignKey("WinnerRafflesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_iGamingBot.DefaultUser", null)
                        .WithMany()
                        .HasForeignKey("WinnersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DefaultUserStreamer", b =>
                {
                    b.HasOne("backend_iGamingBot.DefaultUser", null)
                        .WithMany()
                        .HasForeignKey("AdminsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_iGamingBot.Streamer", null)
                        .WithMany()
                        .HasForeignKey("NegotiableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("backend_iGamingBot.Raffle", b =>
                {
                    b.HasOne("backend_iGamingBot.Streamer", "Creator")
                        .WithMany("CreatedRaffles")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("backend_iGamingBot.Subscriber", b =>
                {
                    b.HasOne("backend_iGamingBot.Streamer", "Streamer")
                        .WithMany("SubscribersRelation")
                        .HasForeignKey("StreamerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("backend_iGamingBot.DefaultUser", "User")
                        .WithMany("StreamersRelation")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Streamer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("backend_iGamingBot.DefaultUser", b =>
                {
                    b.Navigation("StreamersRelation");
                });

            modelBuilder.Entity("backend_iGamingBot.Streamer", b =>
                {
                    b.Navigation("CreatedRaffles");

                    b.Navigation("SubscribersRelation");
                });
#pragma warning restore 612, 618
        }
    }
}
