using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Configrations
{
    public class SwimmerConfigrations : IEntityTypeConfiguration<Swimmer>
    {
        public void Configure(EntityTypeBuilder<Swimmer> builder)
        {
            builder.HasKey(S => S.Id);
            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(200);
            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.CompetitionReadiness).HasConversion<int>();

            // Set Relationships

            builder.HasOne(e => e.Team)
            .WithMany(t => t.Swimmers)
            .HasForeignKey(e => e.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

            builder.ToTable("Swimmers");
        }
    }
}
