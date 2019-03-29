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
    }
}