using Application.ServiceInterfaces;
using Core.Enums;
using Core.Models;
using Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ServiceImplementation
{
    public class DataSeeder : IDataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DataSeeder> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;  

        public DataSeeder(
            ApplicationDbContext context,
            ILogger<DataSeeder> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _roleManager = roleManager;  
        }

        public async Task SeedAllAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");


                await SeedRolesAsync();

                // Check if business data already exists
                if (await _context.Set<Coach>().AnyAsync())
                {
                    _logger.LogInformation("Database already has business data. Skipping business data seeding.");
                    return;
                }

                await SeedCoachesAsync();
                await SeedTeamsAsync();
                await SeedSwimmersAsync();
                await SeedPerformanceRecordsAsync();
                await SeedPerformanceNotesAsync();

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding database");
                throw;
            }
        }
        public async Task SeedRolesAsync()
        {
            _logger.LogInformation("Seeding roles...");

            var roles = new[] { "SuperAdmin", "Admin", "Coach", "Swimmer" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation($"Created role: {role}");
                }
            }

            _logger.LogInformation("Role seeding completed");
        }
        public async Task SeedCoachesAsync()
        {
            var coaches = new List<Coach>
            {
                new Coach
                {
                    FirstName = "Mahmoud",
                    LastName = "Khalaf",
                    Email = "MahmoudKhalafWork.com",
                    PhoneNumber = "+2012345678",
                    HireDate = DateTime.UtcNow.AddYears(-5),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-5)
                },
                new Coach
                {
                    FirstName = "Mohanad",
                    LastName = "Nassar",
                    Email = "MohanadNassarWork.com",
                    PhoneNumber = "+1-555-0102",
                    HireDate = DateTime.UtcNow.AddYears(-3),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-3)
                },
                new Coach
                {
                    FirstName = "Abd-Elfatah",
                    LastName = "Ahmed",
                    Email = "miAbd-ElfatahWork.com",
                    PhoneNumber = "+1-555-0103",
                    HireDate = DateTime.UtcNow.AddYears(-1),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-1)
                }
            };

            await _context.Set<Coach>().AddRangeAsync(coaches);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} coaches", coaches.Count);
        }

        public async Task SeedTeamsAsync()
        {
            var coaches = await _context.Set<Coach>().ToListAsync();
            if (!coaches.Any()) return;

            var teams = new List<Team>
            {
                new Team
                {
                    Name = "Dolphin",
                    Description = "intermediate competitive swimming team for intermediate swimmers",
                    CoachId = coaches[0].Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                },
                new Team
                {
                    Name = "Shark",
                    Description = "Advanced team competitive swimming team for Advanced swimmers",
                    CoachId = coaches[1].Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                },
                new Team
                {
                    Name = "Crocodile",
                    Description = "Beginner team for young swimmers learning the basics",
                    CoachId = coaches[2].Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-2)
                }
            };

            await _context.Set<Team>().AddRangeAsync(teams);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} teams", teams.Count);
        }

        public async Task SeedSwimmersAsync()
        {
            var teams = await _context.Set<Team>().ToListAsync();
            if (!teams.Any()) return;

            var swimmers = new List<Swimmer>
            {
                // Dolphins Elite Team
                new Swimmer
                {
                    FirstName = "Emma",
                    LastName = "Thompson",
                    Email = "emma.t@example.com",
                    DateOfBirth = new DateTime(2010, 3, 15),
                    PhoneNumber = "+1-555-1001",
                    JoinDate = DateTime.UtcNow.AddYears(-2),
                    CompetitionReadiness = CompetitionReadiness.Ready,
                    IsActive = true,
                    TeamId = teams[0].Id,
                    CreatedAt = DateTime.UtcNow.AddYears(-2)
                },
                new Swimmer
                {
                    FirstName = "James",
                    LastName = "Wilson",
                    Email = "james.w@example.com",
                    DateOfBirth = new DateTime(2009, 7, 22),
                    PhoneNumber = "+1-555-1002",
                    JoinDate = DateTime.UtcNow.AddYears(-3),
                    CompetitionReadiness = CompetitionReadiness.Ready,
                    IsActive = true,
                    TeamId = teams[0].Id,
                    CreatedAt = DateTime.UtcNow.AddYears(-3)
                },
                
                // Sharks Academy Team
                new Swimmer
                {
                    FirstName = "Saif",
                    LastName = "Ahmed",
                    Email = "SaifAhmed.m@example.com",
                    DateOfBirth = new DateTime(2009, 2, 14),
                    PhoneNumber = "+1-555-1003",
                    JoinDate = DateTime.UtcNow.AddYears(-3),
                    CompetitionReadiness = CompetitionReadiness.Ready,
                    IsActive = true,
                    TeamId = teams[1].Id,
                    CreatedAt = DateTime.UtcNow.AddYears(-3)
                },
                new Swimmer
                {
                    FirstName = "Oliver",
                    LastName = "Chen",
                    Email = "oliver.c@example.com",
                    DateOfBirth = new DateTime(2010, 9, 30),
                    PhoneNumber = "+1-555-1004",
                    JoinDate = DateTime.UtcNow.AddMonths(-8),
                    CompetitionReadiness = CompetitionReadiness.OnTrack,
                    IsActive = true,
                    TeamId = teams[1].Id,
                    CreatedAt = DateTime.UtcNow.AddMonths(-8)
                },
                
                // Seahorses Junior Team
                new Swimmer
                {
                    FirstName = "Isabella",
                    LastName = "Garcia",
                    Email = "isabella.g@example.com",
                    DateOfBirth = new DateTime(2013, 5, 12),
                    PhoneNumber = "+1-555-1005",
                    JoinDate = DateTime.UtcNow.AddMonths(-3),
                    CompetitionReadiness = CompetitionReadiness.NotReady,
                    IsActive = true,
                    TeamId = teams[2].Id,
                    CreatedAt = DateTime.UtcNow.AddMonths(-3)
                },
                new Swimmer
                {
                    FirstName = "Lucas",
                    LastName = "Brown",
                    Email = "lucas.b@example.com",
                    DateOfBirth = new DateTime(2012, 8, 19),
                    PhoneNumber = "+1-555-1006",
                    JoinDate = DateTime.UtcNow.AddMonths(-4),
                    CompetitionReadiness = CompetitionReadiness.NotReady,
                    IsActive = true,
                    TeamId = teams[2].Id,
                    CreatedAt = DateTime.UtcNow.AddMonths(-4)
                }
            };

            await _context.Set<Swimmer>().AddRangeAsync(swimmers);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} swimmers", swimmers.Count);
        }

        public async Task SeedPerformanceRecordsAsync()
        {
            var swimmers = await _context.Set<Swimmer>().ToListAsync();
            var coaches = await _context.Set<Coach>().ToListAsync();
            if (!swimmers.Any() || !coaches.Any()) return;

            var random = new Random();
            var records = new List<PerformanceRecord>();

            foreach (var swimmer in swimmers)
            {
                // Add 3-5 performance records per swimmer
                int recordCount = random.Next(3, 6);

                for (int i = 0; i < recordCount; i++)
                {
                    var distance = (EventDistance)Enum.GetValues(typeof(EventDistance))
                        .GetValue(random.Next(Enum.GetValues(typeof(EventDistance)).Length))!;

                    // Generate realistic times based on distance
                    decimal baseTime = distance switch
                    {
                        EventDistance.Fifty => random.Next(25, 35),
                        EventDistance.Hundred => random.Next(55, 75),
                        EventDistance.TwoHundred => random.Next(120, 160),
                        EventDistance.FourHundred => random.Next(260, 340),
                        _ => random.Next(30, 200)
                    };

                    records.Add(new PerformanceRecord
                    {
                        SwimmerId = swimmer.Id,
                        Distance = distance,
                        TimeInSeconds = baseTime + (decimal)(random.NextDouble() * 5),
                        RecordedDate = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
                        RecordedByCoachId = coaches[random.Next(coaches.Count)].Id,
                        Comments = i % 3 == 0 ? "Personal best!" : "Good effort",
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 90))
                    });
                }
            }
            await _context.Set<PerformanceRecord>().AddRangeAsync(records);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} performance records", records.Count);
        }

        public async Task SeedPerformanceNotesAsync()
        {
            var swimmers = await _context.Set<Swimmer>().ToListAsync();
            var coaches = await _context.Set<Coach>().ToListAsync();
            if (!swimmers.Any() || !coaches.Any()) return;

            var random = new Random();
            var notes = new List<PerformanceNote>();

            var noteTemplates = new[]
            {
                "Excellent technique in freestyle",
                "Needs work on flip turns",
                "Great improvement in breathing",
                "Shows promise for competitions",
                "Struggling with butterfly stroke",
                "Excellent endurance today",
                "Work on starts and dives",
                "Good attitude during practice",
                "Should consider joining advanced group",
                "Made significant progress this month"
            };

            foreach (var swimmer in swimmers)
            {
                // Add 2-4 notes per swimmer
                int noteCount = random.Next(2, 5);

                for (int i = 0; i < noteCount; i++)
                {
                    notes.Add(new PerformanceNote
                    {
                        SwimmerId = swimmer.Id,
                        CoachId = coaches[random.Next(coaches.Count)].Id,
                        Note = noteTemplates[random.Next(noteTemplates.Length)],
                        NoteDate = DateTime.UtcNow.AddDays(-random.Next(1, 60)),
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 60))
                    });
                }
            }

            await _context.Set<PerformanceNote>().AddRangeAsync(notes);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} performance notes", notes.Count);
        }
    }
}