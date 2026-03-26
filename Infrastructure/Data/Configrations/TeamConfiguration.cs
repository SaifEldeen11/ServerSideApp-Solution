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
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);

            // Set Relationships
            builder.HasOne(e => e.Coach)
                .WithMany(c => c.Teams)
                .HasForeignKey(e => e.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Teams");
        }
    }
}
