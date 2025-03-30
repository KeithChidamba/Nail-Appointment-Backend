namespace Appointments_Backend.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BusinessOwner
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BusinessID { get; set; }

    [Required]
    [MaxLength(255)]
    public string? BusinessName { get; set; }

    [Required]
    [MaxLength(100)]
    public string? OwnerFirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string? OwnerLastName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string? OwnerEmail { get; set; }

    [Required]
    [Phone]
    [MaxLength(20)]
    public string?   OwnerPhone { get; set; }

    [Required]
    [MaxLength(255)] // Store hashed passwords
    public string? OwnerPassword { get; set; }
}

