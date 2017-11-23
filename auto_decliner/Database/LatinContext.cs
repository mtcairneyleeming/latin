using Microsoft.EntityFrameworkCore;

namespace LatinAutoDecline.Database
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
        public virtual DbSet<Definition> Definition { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<Genders> Genders { get; set; }
        public virtual DbSet<LemmaData> LemmaData { get; set; }
        public virtual DbSet<Lemma> Lemmas { get; set; }
        public virtual DbSet<ListUser> ListUsers { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<PartOfSpeech> PartOfSpeech { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<SectionWord> SectionWords { get; set; }
        public virtual DbSet<UserLearntWord> UserLearntWords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=.\;Database=latin;Trusted_connection=True;");
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

                entity.Property(e => e.CategoryIdentifier)
                    .HasColumnName("category_identifier")
                    .HasColumnType("char(1)");

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

            modelBuilder.Entity<Form>(entity =>
            {
                entity.ToTable("forms", "perseus");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Text)
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

                entity.Property(e => e.GenderId).HasColumnName("gender_id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(250);

                entity.Property(e => e.GenderCode)
                    .IsRequired()
                    .HasColumnName("gender_code")
                    .HasColumnType("char(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);
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

                entity.Property(e => e.PartOfSpeechId).HasColumnName("part_of_speech_id");

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

                entity.HasOne(d => d.PartOfSpeech)
                    .WithMany(p => p.LemmaData)
                    .HasForeignKey(d => d.PartOfSpeechId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_lemma_data_part_of_speech");
            });

            modelBuilder.Entity<Lemma>(entity =>
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

            modelBuilder.Entity<ListUser>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ListId });

                entity.ToTable("list_users", "learn");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(100);

                entity.Property(e => e.ListId).HasColumnName("list_id");

                entity.Property(e => e.IsOwner).HasColumnName("is_owner");
                entity.Property(e => e.IsContributor).HasColumnName("is_contributor");
                entity.Property(e => e.IsLearning).HasColumnName("is_learning");
                entity.HasOne(d => d.List)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_users_lists");
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => e.ListId);

                entity.ToTable("lists", "learn");

                entity.Property(e => e.ListId)
                    .HasColumnName("list_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(250);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);
                
                entity.Property(e => e.IsPrivate).HasColumnName("is_private");
                entity.Property(e => e.IsSearchable).HasColumnName("is_searchable");
            });

            modelBuilder.Entity<PartOfSpeech>(entity =>
            {
                entity.HasKey(e => e.PartId);

                entity.ToTable("part_of_speech", "learn");

                entity.Property(e => e.PartId)
                    .HasColumnName("part_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.PartDesc)
                    .IsRequired()
                    .HasColumnName("part_desc")
                    .HasMaxLength(250);

                entity.Property(e => e.PartName)
                    .IsRequired()
                    .HasColumnName("part_name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.SectionId);

                entity.ToTable("sections", "learn");

                entity.Property(e => e.SectionId)
                    .HasColumnName("section_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ListId).HasColumnName("list_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.HasOne(d => d.List)
                    .WithMany(p => p.Sections)
                    .HasForeignKey(d => d.ListId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_sections_lists");
            });

            modelBuilder.Entity<SectionWord>(entity =>
            {
                entity.HasKey(e => new { e.SectionId, e.LemmaId });

                entity.ToTable("section_words", "link");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.LemmaId).HasColumnName("lemma_id");

                entity.HasOne(d => d.Lemma)
                    .WithMany(p => p.SectionWords)
                    .HasForeignKey(d => d.LemmaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_lists_lemmas_link_lemmas");

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.SectionWords)
                    .HasForeignKey(d => d.SectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_section_words");
            });

            modelBuilder.Entity<UserLearntWord>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LemmaId });

                entity.ToTable("user_learnt_words", "learn");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(100);

                entity.Property(e => e.LemmaId).HasColumnName("lemma_id");

                entity.Property(e => e.NextRevision)
                    .HasColumnName("next_revision")
                    .HasColumnType("date");

                entity.Property(e => e.RevisionStage)
                    .HasColumnName("revision_stage")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Lemma)
                    .WithMany(p => p.UserLearntWords)
                    .HasForeignKey(d => d.LemmaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_learnt_words_lemmas");
            });
        }
    }
}
