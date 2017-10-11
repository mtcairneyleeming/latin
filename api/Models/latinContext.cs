using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api.Models
{
    public partial class latinContext : DbContext
    {
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Definition> Definition { get; set; }
        public virtual DbSet<Forms> Forms { get; set; }
        public virtual DbSet<Genders> Genders { get; set; }
        public virtual DbSet<HibLemmas> HibLemmas { get; set; }
        public virtual DbSet<HibParses> HibParses { get; set; }
        public virtual DbSet<LemmaData> LemmaData { get; set; }
        public virtual DbSet<Lemmas> Lemmas { get; set; }
        public virtual DbSet<Lists> Lists { get; set; }
        public virtual DbSet<ListsLemmas> ListsLemmas { get; set; }
        public virtual DbSet<PartOfSpeech> PartOfSpeech { get; set; }
        public virtual DbSet<UserLearntWords> UserLearntWords { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        public latinContext(DbContextOptions<latinContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {   
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=.\;Database=latin;Trusted_Connection=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category", "link");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(25);

                entity.Property(e => e.Number).HasColumnName("number");
            });

            modelBuilder.Entity<Definition>(entity =>
            {
                entity.ToTable("definition", "learn");

                entity.Property(e => e.DefinitionId).HasColumnName("definition_id");

                entity.Property(e => e.Alevel)
                    .IsRequired()
                    .HasColumnName("alevel")
                    .HasMaxLength(500);

                entity.Property(e => e.LemmaId).HasColumnName("lemma_id");

                entity.HasOne(d => d.Lemma)
                    .WithMany(p => p.Definition)
                    .HasForeignKey(d => d.LemmaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_definition_lemmas");
            });

            modelBuilder.Entity<Forms>(entity =>
            {
                entity.ToTable("forms", "perseus");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Form)
                    .HasColumnName("form")
                    .HasMaxLength(75);

                entity.Property(e => e.LemmaId).HasColumnName("lemma_id");

                entity.Property(e => e.MiscFeatures)
                    .HasColumnName("misc_features")
                    .HasMaxLength(50);

                entity.Property(e => e.MorphCode)
                    .HasColumnName("morph_code")
                    .HasMaxLength(13);

                entity.HasOne(d => d.Lemma)
                    .WithMany(p => p.Forms)
                    .HasForeignKey(d => d.LemmaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_forms_lemmas");
            });

            modelBuilder.Entity<Genders>(entity =>
            {
                entity.HasKey(e => e.GenderId);

                entity.ToTable("genders", "link");

                entity.Property(e => e.GenderId)
                    .HasColumnName("gender_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(250);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<HibLemmas>(entity =>
            {
                entity.HasKey(e => e.LemmaId);

                entity.ToTable("hib_lemmas", "perseus");

                entity.HasIndex(e => e.BareHeadword)
                    .HasName("bare_idx");

                entity.HasIndex(e => e.LemmaId)
                    .HasName("FK319881499970ED3");

                entity.Property(e => e.LemmaId)
                    .HasColumnName("lemma_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.BareHeadword)
                    .HasColumnName("bare_headword")
                    .HasMaxLength(100);

                entity.Property(e => e.LemmaSequenceNumber).HasColumnName("lemma_sequence_number");

                entity.Property(e => e.LemmaShortDef)
                    .HasColumnName("lemma_short_def")
                    .HasMaxLength(255);

                entity.Property(e => e.LemmaText)
                    .HasColumnName("lemma_text")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<HibParses>(entity =>
            {
                entity.ToTable("hib_parses", "perseus");

                entity.HasIndex(e => e.BareForm)
                    .HasName("bare_form_idx");

                entity.HasIndex(e => e.Form)
                    .HasName("form_idx");

                entity.HasIndex(e => e.LemmaId)
                    .HasName("FK3835E29E9970ED3");

                entity.HasIndex(e => new { e.LemmaId, e.Form })
                    .HasName("form_lemma");

                entity.HasIndex(e => new { e.LemmaId, e.MorphCode, e.ExpandedForm, e.Form })
                    .HasName("lemma_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BareForm)
                    .HasColumnName("bare_form")
                    .HasMaxLength(75);

                entity.Property(e => e.Dialects)
                    .HasColumnName("dialects")
                    .HasMaxLength(50);

                entity.Property(e => e.ExpandedForm)
                    .HasColumnName("expanded_form")
                    .HasMaxLength(75);

                entity.Property(e => e.Form)
                    .IsRequired()
                    .HasColumnName("form")
                    .HasMaxLength(75);

                entity.Property(e => e.LemmaId).HasColumnName("lemma_id");

                entity.Property(e => e.MiscFeatures)
                    .HasColumnName("misc_features")
                    .HasMaxLength(50);

                entity.Property(e => e.MorphCode)
                    .HasColumnName("morph_code")
                    .HasMaxLength(13);
            });

            modelBuilder.Entity<LemmaData>(entity =>
            {
                entity.HasKey(e => e.LemmaId);

                entity.ToTable("lemma_data", "learn");

                entity.Property(e => e.LemmaId)
                    .HasColumnName("lemma_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.GenderId).HasColumnName("gender_id");

                entity.Property(e => e.PartOfSpeech).HasColumnName("part_of_speech");

                entity.Property(e => e.UseSingular).HasColumnName("use_singular");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.LemmaData)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_nouns_declensions1");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.LemmaData)
                    .HasForeignKey(d => d.GenderId)
                    .HasConstraintName("FK_nouns_declensions");

                entity.HasOne(d => d.Lemma)
                    .WithOne(p => p.LemmaData)
                    .HasForeignKey<LemmaData>(d => d.LemmaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_nouns_lemmas");
            });

            modelBuilder.Entity<Lemmas>(entity =>
            {
                entity.HasKey(e => e.LemmaId);

                entity.ToTable("lemmas", "perseus");

                entity.Property(e => e.LemmaId).HasColumnName("lemma_id");

                entity.Property(e => e.LemmaShortDef)
                    .HasColumnName("lemma_short_def")
                    .HasMaxLength(255);

                entity.Property(e => e.LemmaText)
                    .HasColumnName("lemma_text")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Lists>(entity =>
            {
                entity.ToTable("lists", "learn");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(300);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ListsLemmas>(entity =>
            {
                entity.HasKey(e => new { e.ListId, e.LemmaId });

                entity.ToTable("lists_lemmas", "link");

                entity.Property(e => e.ListId).HasColumnName("list_id");

                entity.Property(e => e.LemmaId).HasColumnName("lemma_id");

                entity.HasOne(d => d.Lemma)
                    .WithMany(p => p.ListsLemmas)
                    .HasForeignKey(d => d.LemmaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_lists_lemmas_link_lemmas");

                entity.HasOne(d => d.List)
                    .WithMany(p => p.ListsLemmas)
                    .HasForeignKey(d => d.ListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_lists_lemmas_link_lists_lemmas_link");
            });

            modelBuilder.Entity<PartOfSpeech>(entity =>
            {
                entity.HasKey(e => e.PartId);

                entity.ToTable("part_of_speech", "learn");

                entity.Property(e => e.PartId).HasColumnName("part_id");

                entity.Property(e => e.PartDesc)
                    .IsRequired()
                    .HasColumnName("part_desc")
                    .HasMaxLength(250);

                entity.Property(e => e.PartName)
                    .IsRequired()
                    .HasColumnName("part_name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserLearntWords>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LemmaId });

                entity.ToTable("user_learnt_words", "learn");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.LemmaId).HasColumnName("lemma_id");

                entity.HasOne(d => d.Lemma)
                    .WithMany(p => p.UserLearntWords)
                    .HasForeignKey(d => d.LemmaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_learnt_words_lemmas");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLearntWords)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_learnt_words_user_learnt_words");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("users", "learn");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("binary(60)");
            });
        }
    }
}
