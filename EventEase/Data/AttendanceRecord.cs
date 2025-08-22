public class AttendanceRecord
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int RegistrationId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string CheckInMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
}