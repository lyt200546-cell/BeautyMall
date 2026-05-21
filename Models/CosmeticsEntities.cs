using System.Data.Entity;

namespace MZWlyt.Models
{
    public partial class CosmeticsEntities : DbContext
    {
        public CosmeticsEntities()
            : base("name=CosmeticsEntities")
        {
        }

        public virtual DbSet<tb_admin> tb_admins { get; set; }
        public virtual DbSet<tb_cart> tb_carts { get; set; }
        public virtual DbSet<tb_message> tb_messages { get; set; }
        public virtual DbSet<tb_orderDetails> tb_orderDetailses { get; set; }
        public virtual DbSet<tb_orders> tb_orderses { get; set; }
        public virtual DbSet<tb_product> tb_products { get; set; }
        public virtual DbSet<tb_user> tb_users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tb_admin>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<tb_admin>()
                .Property(e => e.tel)
                .IsUnicode(false);

            modelBuilder.Entity<tb_cart>()
                .Property(e => e.price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<tb_cart>()
                .Property(e => e.photo)
                .IsUnicode(false);

            modelBuilder.Entity<tb_orderDetails>()
                .Property(e => e.price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<tb_orderDetails>()
                .Property(e => e.photo)
                .IsUnicode(false);

            modelBuilder.Entity<tb_orders>()
                .Property(e => e.allPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<tb_orders>()
                .Property(e => e.tel)
                .IsUnicode(false);

            modelBuilder.Entity<tb_product>()
                .Property(e => e.photo)
                .IsUnicode(false);

            modelBuilder.Entity<tb_product>()
                .Property(e => e.price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<tb_product>()
                .HasMany(e => e.tb_cart)
                .WithRequired(e => e.tb_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tb_product>()
                .HasMany(e => e.tb_orderDetails)
                .WithRequired(e => e.tb_product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tb_user>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<tb_user>()
                .Property(e => e.tel)
                .IsUnicode(false);

            modelBuilder.Entity<tb_user>()
                .Property(e => e.email)
                .IsUnicode(false);
        }
    }
}
