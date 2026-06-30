using System;

namespace CybersecurityBotWPF.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public int UserId { get; set; }

        public string ReminderDisplay
        {
            get
            {
                if (ReminderDate.HasValue)
                    return ReminderDate.Value.ToString("yyyy-MM-dd");
                return "No reminder";
            }
        }

        public string StatusDisplay
        {
            get
            {
                return IsCompleted ? "✅ Completed" : "⏳ Pending";
            }
        }
    }
}