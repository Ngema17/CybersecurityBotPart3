using System;
using System.Collections.Generic;
using CybersecurityBotWPF.Models;
using Microsoft.Data.Sqlite;

namespace CybersecurityBotWPF.Services
{
    public class ActivityLogService
    {
        private readonly DatabaseService _dbService;

        public ActivityLogService()
        {
            _dbService = new DatabaseService();
        }

        public void AddLog(int userId, string action, string details)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = @"
                INSERT INTO ActivityLog (UserId, Action, Details, Timestamp)
                VALUES (@UserId, @Action, @Details, @Timestamp)";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@Details", details ?? string.Empty);
            cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public List<ActivityLog> GetRecentLogs(int userId, int count = 10)
        {
            var logs = new List<ActivityLog>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = @"
                SELECT * FROM ActivityLog 
                WHERE UserId = @UserId 
                ORDER BY Timestamp DESC 
                LIMIT @Count";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Count", count);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                logs.Add(new ActivityLog
                {
                    LogId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Action = reader.GetString(2),
                    Details = reader.GetString(3),
                    Timestamp = DateTime.Parse(reader.GetString(4))
                });
            }

            connection.Close();
            return logs;
        }

        public void AddChatLog(int userId, string userMessage, string botResponse)
        {
            AddLog(userId, "Chat Interaction", $"User: {userMessage} | Bot: {botResponse}");
        }

        public void AddTaskLog(int userId, string action, string taskTitle)
        {
            AddLog(userId, "Task Action", $"{action}: {taskTitle}");
        }

        public void AddQuizLog(int userId, int score, int total)
        {
            AddLog(userId, "Quiz Attempt", $"Score: {score}/{total} ({((double)score / total) * 100:F1}%)");
        }

        public void AddSentimentLog(int userId, string sentiment, string message)
        {
            AddLog(userId, "Sentiment Detected", $"{sentiment}: {message}");
        }
    }
}