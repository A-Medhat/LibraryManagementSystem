namespace LibraryManagementSystem.Models.DTOs.Members;

public class MemberResponseDto
{
    public int MemberId { get; set; }
    public string MembershipNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateOnly JoinDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
