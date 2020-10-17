using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace entity_framework
{
    class Program
    {
        static void Main(string[] args)
        {
            using var context = new StudentDBContext();

            context.CreateAllEntities();
            var data = (from f in context.Faculties
                       join g in context.Groups on f.Id equals g.Faculty.Id
                       join s in context.Students on g.Id equals s.Group.Id
                       select new
                       {
                           facName = f.Name,
                           grName = g.Name,
                           studName = s.Name,
                           studAge = s.Age
                       })
                       .ToList().GroupBy(gr => new { gr.facName, gr.grName });

            foreach(var d in data)
            {
                Console.WriteLine($"Faculty: {d.Key.facName} Group: {d.Key.grName}");
                foreach(var st in d)
                {
                    Console.WriteLine($"{" ",5}Name: {st.studName,8} | Age: {st.studAge,2}");
                }
                Console.WriteLine();
            }
        }
    }

    public class StudentDBContext : DbContext
    {
        public StudentDBContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=./testDB.sqlite");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasKey(s => s.Id);
            modelBuilder.Entity<Student>().Property(s => s.Name).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Student>().Property(s => s.Age).HasMaxLength(50);

            modelBuilder.Entity<Group>().HasKey(g => g.Id);
            modelBuilder.Entity<Group>().Property(g => g.Name).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Group>().HasMany(g => g.Students).WithOne(s => s.Group);


            modelBuilder.Entity<Faculty>().HasKey(f => f.Id);
            modelBuilder.Entity<Faculty>().Property(f => f.Name).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Faculty>().HasMany(f => f.Groups).WithOne(g => g.Faculty);
        }

        public void CreateAllEntities()
        {
            using StudentDBContext context = new StudentDBContext();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                if (!context.Students.Any() && !context.Groups.Any() && !context.Faculties.Any())
                {
                    context.Students.AddRange(new Student[] {
                            new Student { Age = 17, Name = "Alex" },
                            new Student { Age = 18, Name = "Anton" },
                            new Student { Age = 18, Name = "Evgen" },
                            new Student { Age = 19, Name = "Ruslan" },
                            new Student { Age = 20, Name = "Valera" },
                            new Student { Age = 20, Name = "Vlad" }
                        });
                    context.SaveChanges();

                    context.Groups.AddRange(new Group[] {
                            new Group { Name = "PO1703", Students = { context.Students.Find(1), context.Students.Find(2) } },
                            new Group { Name = "TZ1803", Students = { context.Students.Find(3), context.Students.Find(4) } },
                            new Group { Name = "AE1603", Students = { context.Students.Find(5), context.Students.Find(6) } }
                        });
                    context.SaveChanges();

                    context.Faculties.AddRange(new Faculty[] {
                            new Faculty { Name = "TK", Groups = { context.Groups.Find(1), context.Groups.Find(2), context.Groups.Find(3) } }
                        });
                    context.SaveChanges();

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                transaction.Rollback();
            }
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
    }
}
