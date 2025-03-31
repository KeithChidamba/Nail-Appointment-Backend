using Microsoft.EntityFrameworkCore;

namespace Appointments_Backend.Data;

public class AppointmentDbContext: DbContext
{
    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options) { }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Business> BusinessOwners { get; set; }
}

