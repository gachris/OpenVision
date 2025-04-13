// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.DbContexts;

namespace OpenVision.IdentityServer.Admin.EntityFramework.PostgreSQL.Migrations.AuditLogging
{
    [DbContext(typeof(AdminAuditLogDbContext))]
    [Migration("20191120100220_DbInit")]
    partial class DbInit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Skoruba.AuditLogging.EntityFramework.Entities.AuditLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Action")
                        .HasColumnType("text");

                    b.Property<string>("Category")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Data")
                        .HasColumnType("text");

                    b.Property<string>("Event")
                        .HasColumnType("text");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<string>("SubjectAdditionalData")
                        .HasColumnType("text");

                    b.Property<string>("SubjectIdentifier")
                        .HasColumnType("text");

                    b.Property<string>("SubjectName")
                        .HasColumnType("text");

                    b.Property<string>("SubjectType")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AuditLog");
                });
#pragma warning restore 612, 618
        }
    }
}
