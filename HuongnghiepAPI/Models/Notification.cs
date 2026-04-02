public class Notification
{
    public int NotificationId { get; set; }
    public int StudentId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false;
}
