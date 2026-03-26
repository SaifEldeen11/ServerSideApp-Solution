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
    public class PerformanceNoteConfiguration : IEntityTypeConfiguration<PerformanceNote>
    {
        public void Configure(EntityTypeBuilder<PerformanceNote> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Note).IsRequired().HasMaxLength(2000);

            builder.HasOne(e => e.Swimmer)
                .WithMany(s => s.PerformanceNotes)
                .HasForeignKey(e => e.SwimmerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Coach)
                .WithMany(c => c.PerformanceNotes)
                .HasForeignKey(e => e.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("PerformanceNotes");
        }
    }
}
