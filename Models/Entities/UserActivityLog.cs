using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models.Entities;


public class UserActivityLog
{
    [Key]
    public long LogId { get; set; }

    [Required]
    //string because identinty
    public string UserId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Entity { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    [MaxLength(500)]
    public string? Details { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;


    public ApplicationUser User { get; set; }
}
