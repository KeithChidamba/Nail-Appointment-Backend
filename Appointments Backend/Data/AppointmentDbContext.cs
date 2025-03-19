using Microsoft.EntityFrameworkCore;

namespace Appointments_Backend.Data;

public class AppointmentDbContext: DbContext
{
    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options) { }

    public DbSet<Appointment> Appointments { get; set; }
}

public class Appointment
{
    public int AppointmentID { get; set; }
    public string ClientFirstName { get; set; }
    public string ClientLastName { get; set; }
    public string ClientEmail { get; set; }
    public string ClientPhone { get; set; }
    public string AppointmentDate { get; set; }
    public string AppointmentTime { get; set; }
    public string TimeWhenBooked { get; set; }
    public float AppointmentPrice { get; set; }
    public string AppointmentName { get; set; }
    public int AppointmentDurationInMinutes { get; set; }
}