using System.ComponentModel.DataAnnotations;

namespace EventEase.Data;

public class Registration
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public RegistrationStatus Status { get; set; }
    public string? SpecialRequirements { get; set; }
    public bool IsAttended { get; set; }
}

public enum RegistrationStatus
{
    Pending,
    Confirmed,
    Cancelled,
    Attended
}

public class RegistrationFormModel
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [RegularExpression(@"^[\+]?[1-9][\d]{0,15}$", ErrorMessage = "Please enter a valid phone number")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Special requirements cannot exceed 500 characters")]
    public string? SpecialRequirements { get; set; }

    [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and conditions")]
    public bool AgreeToTerms { get; set; }
}
