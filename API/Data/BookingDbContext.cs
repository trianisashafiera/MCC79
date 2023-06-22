using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class BookingDbContext : DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {
        }
        //Table
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRole> AccountRoles { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }  
        public DbSet<Room> Rooms { get; set; }
        public DbSet<University> Universities { get; set;}

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Constraints uniq
            modelBuilder.Entity<Employee>().HasIndex(e => new
            {
                e.Nik,
                e.Email,
                e.PhoneNumber
            }).IsUnique();

            //University - Education (one to many)
            modelBuilder.Entity<University>()
                .HasMany(university => university.Educations)
                .WithOne(education => education.University)
                .HasForeignKey(education => education.UniversityGuid);

            //employee - education (one to one)
            modelBuilder.Entity<Employee>()
                .HasOne(employee => employee.Education)
                .WithOne(education => education.Employee)
                .HasForeignKey<Education>(education => education.Guid);

            //employee - booking (one to many)
            modelBuilder.Entity<Employee>()
                .HasMany(employee => employee.Bookings)
                .WithOne(booking => booking.Employee)
                .HasForeignKey(booking => booking.EmployeeGuid);

            //booking - room (many to one)
            modelBuilder.Entity<Booking>()
                .HasOne(booking => booking.Room)
                .WithMany(room => room.Bookings)
                .HasForeignKey(booking => booking.RoomGuid);

            //employee - account (one to one)
            modelBuilder.Entity<Employee>()
                .HasOne(employee => employee.Account)
                .WithOne(account => account.Employee)
                .HasForeignKey<Account>(account => account.Guid);

            //account - accountRole (many to one)
            modelBuilder.Entity<Account>()
                .HasMany(account => account.AccountRoles)
                .WithOne(accountRole => accountRole.Account)
                .HasForeignKey(accountRole => accountRole.AccountGuid);

            //accountRole - role (many to one)
            modelBuilder.Entity<AccountRole>()
                .HasOne(accountrole => accountrole.Role)
                .WithMany(role => role.AccountRoles)
                .HasForeignKey(accountrole => accountrole.RoleGuid);
        }

    }
}
