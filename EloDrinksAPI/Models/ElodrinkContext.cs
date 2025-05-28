using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace EloDrinksAPI.Models;

public partial class ElodrinkContext : DbContext
{
    public ElodrinkContext()
    {
    }

    public ElodrinkContext(DbContextOptions<ElodrinkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<Orcamento> Orcamentos { get; set; }

    public virtual DbSet<OrcamentoHasItem> OrcamentoHasItems { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.IdItem).HasName("PRIMARY");

            entity.ToTable("item");

            entity.Property(e => e.IdItem)
                .HasMaxLength(45)
                .HasColumnName("idItem");
            entity.Property(e => e.Descricao)
                .HasColumnType("text")
                .HasColumnName("descricao");
            entity.Property(e => e.Nome)
                .HasMaxLength(45)
                .HasColumnName("nome");
            entity.Property(e => e.Preco).HasColumnName("preco");
            entity.Property(e => e.Tipo)
                .HasMaxLength(45)
                .HasColumnName("tipo");
        });

        modelBuilder.Entity<Orcamento>(entity =>
        {
            entity.HasKey(e => new { e.IdOrcamento, e.UsuarioIdUsuario })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("orcamento");

            entity.HasIndex(e => e.UsuarioIdUsuario, "fk_Orcamento_Usuario1");

            entity.Property(e => e.IdOrcamento)
                .HasMaxLength(45)
                .HasColumnName("idOrcamento");
            entity.Property(e => e.UsuarioIdUsuario)
                .HasMaxLength(45)
                .HasColumnName("Usuario_idUsuario");
            entity.Property(e => e.Cep).HasColumnName("cep");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.DrinksSelecionados)
                .HasColumnType("text")
                .HasColumnName("drinksSelecionados");
            entity.Property(e => e.Endereco)
                .HasMaxLength(100)
                .HasColumnName("endereco");
            entity.Property(e => e.HoraFim)
                .HasColumnType("time")
                .HasColumnName("horaFim");
            entity.Property(e => e.HoraInicio)
                .HasColumnType("time")
                .HasColumnName("horaInicio");
            entity.Property(e => e.Preco).HasColumnName("preco");
            entity.Property(e => e.QtdPessoas).HasColumnName("qtdPessoas");
            entity.Property(e => e.Status)
                .HasMaxLength(45)
                .HasColumnName("status");
            entity.Property(e => e.TipoEvento)
                .HasMaxLength(45)
                .HasColumnName("tipoEvento");

            entity.HasOne(d => d.UsuarioIdUsuarioNavigation).WithMany(p => p.Orcamentos)
                .HasForeignKey(d => d.UsuarioIdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Orcamento_Usuario1");
        });

        modelBuilder.Entity<OrcamentoHasItem>(entity =>
        {
            entity.HasKey(e => new { e.OrcamentoIdOrcamento, e.OrcamentoUsuarioIdUsuario, e.ItemIdItem })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("orcamento_has_item");

            entity.HasIndex(e => e.ItemIdItem, "fk_Orcamento_has_Item_Item1_idx");

            entity.HasIndex(e => new { e.OrcamentoIdOrcamento, e.OrcamentoUsuarioIdUsuario }, "fk_Orcamento_has_Item_Orcamento1_idx");

            entity.Property(e => e.OrcamentoIdOrcamento)
                .HasMaxLength(45)
                .HasColumnName("Orcamento_idOrcamento");
            entity.Property(e => e.OrcamentoUsuarioIdUsuario)
                .HasMaxLength(45)
                .HasColumnName("Orcamento_Usuario_idUsuario");
            entity.Property(e => e.ItemIdItem)
                .HasMaxLength(45)
                .HasColumnName("Item_idItem");

            entity.HasOne(d => d.ItemIdItemNavigation).WithMany(p => p.OrcamentoHasItems)
                .HasForeignKey(d => d.ItemIdItem)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Orcamento_has_Item_Item1");

            entity.HasOne(d => d.Orcamento).WithMany(p => p.OrcamentoHasItems)
                .HasForeignKey(d => new { d.OrcamentoIdOrcamento, d.OrcamentoUsuarioIdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Orcamento_has_Item_Orcamento1");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => new { e.IdPedido, e.OrcamentoIdOrcamento, e.OrcamentoUsuarioIdUsuario })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("pedido");

            entity.HasIndex(e => new { e.OrcamentoIdOrcamento, e.OrcamentoUsuarioIdUsuario }, "fk_Pedido_Orcamento1_idx");

            entity.Property(e => e.IdPedido)
                .HasMaxLength(45)
                .HasColumnName("idPedido");
            entity.Property(e => e.OrcamentoIdOrcamento)
                .HasMaxLength(45)
                .HasColumnName("Orcamento_idOrcamento");
            entity.Property(e => e.OrcamentoUsuarioIdUsuario)
                .HasMaxLength(45)
                .HasColumnName("Orcamento_Usuario_idUsuario");
            entity.Property(e => e.DataCriacao).HasColumnName("dataCriacao");
            entity.Property(e => e.Status)
                .HasMaxLength(45)
                .HasColumnName("status");
            entity.Property(e => e.Total).HasColumnName("total");

            entity.HasOne(d => d.Orcamento).WithMany(p => p.Pedidos)
                .HasForeignKey(d => new { d.OrcamentoIdOrcamento, d.OrcamentoUsuarioIdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Pedido_Orcamento1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.Property(e => e.IdUsuario)
                .HasMaxLength(45)
                .HasColumnName("idUsuario");
            entity.Property(e => e.DataCadastro).HasColumnName("dataCadastro");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(45)
                .HasColumnName("nome");
            entity.Property(e => e.Senha)
                .HasMaxLength(262)
                .HasColumnName("senha");
            entity.Property(e => e.Sobrenome)
                .HasMaxLength(45)
                .HasColumnName("sobrenome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(45)
                .HasColumnName("telefone");
            entity.Property(e => e.Tipo)
                .HasMaxLength(45)
                .HasColumnName("tipo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
