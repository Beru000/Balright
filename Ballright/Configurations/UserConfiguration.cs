using Ballright.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ballright.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserName)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.HasMany(u=>u.Orders)
                   .WithOne(o => o.User)
                   .HasForeignKey(o => o.UserID);
        }
    }
}
