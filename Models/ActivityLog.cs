using System;

namespace CybersecurityBotWPF.Models
{
    public class ActivityLog
    {
        public int LogId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }

        public string DisplayText => $"[{Timestamp:HH:mm}] {Action}: {Details}";
    }

    public enum LogAction
    {
        TaskAdded,
        TaskUpdated,
        TaskCompleted,
        TaskDeleted,
        ReminderSet,
        QuizStarted,
        QuizCompleted,
        NLPInteraction,
        SentimentDetected,
        ChatMessage
    }
}