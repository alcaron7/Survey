using GestDev.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestDev.Infrastructure.Data.Configurations
{
    namespace GestDev.Infrastructure.Data.Configurations
    {
        public class UserConfiguration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.HasKey(u => u.Id);

                builder.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                builder.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.HasIndex(u => u.Email)
                    .IsUnique();

                builder.Property(u => u.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");
            }
        }
    }
}
