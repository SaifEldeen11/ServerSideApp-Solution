using Application.ServiceInterfaces;
using Core.Enums;
using Core.Identity;
using Core.Models;
using Infrastructure.Data.Contexts;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class DataSeeder : IDataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IdentityDbContext _identityContext;
        private readonly ILogger<DataSeeder> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DataSeeder(
            ApplicationDbContext context,
            IdentityDbContext identityContext,
            ILogger<DataSeeder> logger,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _identityContext = identityContext;
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
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
                await SeedUsersAsync();
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

            var roles = new[]
            {
                UserRoles.Admin,
                UserRoles.SeniorCoach,
                UserRoles.HeadCoach,
                UserRoles.RegularCoach,
                UserRoles.Swimmer
            };

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
                    Email = "mahmoud.khalaf@swimacademy.com",
                    PhoneNumber = "+2012345678",
                    HireDate = DateTime.UtcNow.AddYears(-5),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-5)
                },
                new Coach
                {
                    FirstName = "Mohanad",
                    LastName = "Nassar",
                    Email = "mohanad.nassar@swimacademy.com",
                    PhoneNumber = "+1-555-0102",
                    HireDate = DateTime.UtcNow.AddYears(-3),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-3)
                },
                new Coach
                {
                    FirstName = "Abd-Elfatah",
                    LastName = "Ahmed",
                    Email = "abd.elfatah@swimacademy.com",
                    PhoneNumber = "+1-555-0103",
                    HireDate = DateTime.UtcNow.AddYears(-1),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-1)
                },
                new Coach
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "admin@swimacademy.com",
                    PhoneNumber = "+1-555-9999",
                    HireDate = DateTime.UtcNow.AddYears(-10),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddYears(-10)
                }
            };

            await _context.Set<Coach>().AddRangeAsync(coaches);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} coaches", coaches.Count);
        }

        public async Task SeedUsersAsync()
        {
            _logger.LogInformation("Seeding Identity users for coaches...");

            var coaches = await _context.Set<Coach>().OrderBy(c => c.Id).ToListAsync();

            var coachUsers = new[]
            {
                new { Coach = coaches[0], Role = UserRoles.RegularCoach, Password = "Coach@123" },
                new { Coach = coaches[1], Role = UserRoles.SeniorCoach, Password = "Coach@123" },
                new { Coach = coaches[2], Role = UserRoles.HeadCoach, Password = "Coach@123" },
                new { Coach = coaches[3], Role = UserRoles.Admin, Password = "Admin@123" }
            };

            foreach (var item in coachUsers)
            {
                if (await _userManager.FindByEmailAsync(item.Coach.Email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = item.Coach.Email,
                        Email = item.Coach.Email,
                        FirstName = item.Coach.FirstName,
                        LastName = item.Coach.LastName,
                        CoachId = item.Coach.Id,
                        EmailConfirmed = true,
                        IsActive = true
                    };

                    var result = await _userManager.CreateAsync(user, item.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, item.Role);
                        _logger.LogInformation($"Created user for coach {item.Coach.FullName} with role {item.Role}");
                    }
                }
            }

            _logger.LogInformation("Identity users seeding completed");
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
                    Description = "Intermediate competitive swimming team for intermediate swimmers",
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
                new Swimmer
                {
                    FirstName = "Saif",
                    LastName = "Ahmed",
                    Email = "saif.ahmed@example.com",
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
                int recordCount = random.Next(3, 6);

                for (int i = 0; i < recordCount; i++)
                {
                    var distance = (EventDistance)Enum.GetValues(typeof(EventDistance))
                        .GetValue(random.Next(Enum.GetValues(typeof(EventDistance)).Length))!;

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