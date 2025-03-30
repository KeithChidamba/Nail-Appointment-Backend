public class Appointment
{
    public int AppointmentID { get; set; }
    public string? ClientFirstName { get; set; }
    public string? ClientLastName { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientPhone { get; set; }
    public string? AppointmentDate { get; set; }
    public string? AppointmentTime { get; set; }
    public string? DateTimeWhenBooked { get; set; }
    public double AppointmentPrice { get; set; }
    public string? AppointmentName { get; set; }
    public int AppointmentDurationInMinutes { get; set; }
    public int isConfirmed{get;set;}
}