﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MuscleMetrics.Data;

#nullable disable

namespace MuscleMetrics.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250205144708_UsuarioAtualizacaoParte2")]
    partial class UsuarioAtualizacaoParte2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MuscleMetrics.Entidades.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Altura")
                        .HasColumnType("float");

                    b.Property<double>("BF")
                        .HasColumnType("float");

                    b.Property<double>("CaloriaDieta")
                        .HasColumnType("float");

                    b.Property<double>("Carboidrato")
                        .HasColumnType("float");

                    b.Property<double>("FatorAtividade")
                        .HasColumnType("float");

                    b.Property<double>("GastoCalórico")
                        .HasColumnType("float");

                    b.Property<double>("Gordura")
                        .HasColumnType("float");

                    b.Property<int>("Idade")
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Peso")
                        .HasColumnType("float");

                    b.Property<double>("Proteina")
                        .HasColumnType("float");

                    b.Property<string>("Sexo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TMB")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Usuario");
                });
#pragma warning restore 612, 618
        }
    }
}
