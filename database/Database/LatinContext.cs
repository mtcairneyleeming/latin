using System;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace database.Database
{
    public class LatinContext : DbContext
    {
        public LatinContext()
        {
        }

        public LatinContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Definition> Definitions { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<LemmaData> LemmaData { get; set; }
        public virtual DbSet<Lemma> Lemmas { get; set; }
        public virtual DbSet<PartOfSpeech> PartOfSpeech { get; set; }
        public virtual DbSet<UserLearntWord> UserLearntWords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlite(
                $@"Data Source={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/LatinLearning/latin.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lemma>()
                .HasOne(l => l.UserLearntWord)
                .WithOne(ulw=> ulw.Lemma)
                .HasForeignKey<UserLearntWord>(ulw => ulw.LemmaId);
            
            modelBuilder.Entity<Lemma>()
                .HasOne(l => l.LemmaData)
                .WithOne(ld => ld.Lemma)
                .HasForeignKey<LemmaData>(ld => ld.LemmaId);
            
            modelBuilder.Entity<Lemma>()
                .HasMany(l => l.Forms)
                .WithOne(f => f.Lemma)
                .HasForeignKey(f=>f.LemmaId);
            
            modelBuilder.Entity<Lemma>()
                .HasMany(l => l.Definitions)
                .WithOne(d => d.Lemma)
                .HasForeignKey(d=>d.LemmaId);
            
            modelBuilder.Entity<LemmaData>()
                .HasOne(ld => ld.Gender)
                .WithMany(g => g.LemmaData)
                .HasForeignKey(ld => ld.GenderId);
            
            modelBuilder.Entity<LemmaData>()
                .HasOne(ld => ld.Category)
                .WithMany(c => c.LemmaData)
                .HasForeignKey(ld => ld.CategoryId);
            
            modelBuilder.Entity<LemmaData>()
                .HasOne(ld => ld.PartOfSpeech)
                .WithMany(p => p.LemmaData)
                .HasForeignKey(ld => ld.PartOfSpeechId);
            
            

        }
    }
}