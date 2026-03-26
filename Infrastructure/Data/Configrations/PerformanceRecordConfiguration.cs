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
    public class PerformanceRecordConfiguration : IEntityTypeConfiguration<PerformanceRecord>
    {
        public void Configure(EntityTypeBuilder<PerformanceRecord> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Distance).HasConversion<int>();
            builder.Property(e => e.TimeInSeconds).HasPrecision(10, 2);

            builder.HasOne(e => e.Swimmer)
                .WithMany(s => s.PerformanceRecords)
                .HasForeignKey(e => e.SwimmerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.RecordedByCoach)
                .WithMany(c => c.PerformanceRecords)
                .HasForeignKey(e => e.RecordedByCoachId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.SwimmerId, e.Distance, e.RecordedDate });

            builder.ToTable("PerformanceRecords");
        }
    }
}
