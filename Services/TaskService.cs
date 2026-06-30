using CybersecurityBotPart3.Services;
using CybersecurityBotWPF.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

// Fix: Fully qualify 'Task' as 'CybersecurityBotWPF.Models.Task' to resolve ambiguity.
namespace CybersecurityBotWPF.Services
{
    public class TaskService
    {
        private readonly DatabaseService _dbService;
        private readonly ActivityLogService _logService;

        public TaskService(ActivityLogService logService)
        {
            _dbService = new DatabaseService();
            _logService = logService;
        }

        public int AddTask(CybersecurityBotWPF.Models.Task task)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = @"
                INSERT INTO Tasks (UserId, Title, Description, CreatedDate, ReminderDate, IsCompleted)
                VALUES (@UserId, @Title, @Description, @CreatedDate, @ReminderDate, @IsCompleted);
                SELECT last_insert_rowid();";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", task.UserId);
            cmd.Parameters.AddWithValue("@Title", task.Title);
            cmd.Parameters.AddWithValue("@Description", task.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@ReminderDate", task.ReminderDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "");
            cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);

            var taskId = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();

            // Log the action
            _logService.AddLog(task.UserId, "Task Added", $"Task: '{task.Title}' added");

            return taskId;
        }

        public List<CybersecurityBotWPF.Models.Task> GetTasks(int userId, bool includeCompleted = false)
        {
            var tasks = new List<CybersecurityBotWPF.Models.Task>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = includeCompleted
                ? "SELECT * FROM Tasks WHERE UserId = @UserId ORDER BY CreatedDate DESC"
                : "SELECT * FROM Tasks WHERE UserId = @UserId AND IsCompleted = 0 ORDER BY CreatedDate DESC";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new CybersecurityBotWPF.Models.Task
                {
                    TaskId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.GetString(3),
                    CreatedDate = DateTime.Parse(reader.GetString(4)),
                    ReminderDate = reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5)),
                    IsCompleted = reader.GetInt32(6) == 1
                });
            }

            connection.Close();
            return tasks;
        }

        public void UpdateTask(int taskId, bool isCompleted)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = "UPDATE Tasks SET IsCompleted = @IsCompleted WHERE TaskId = @TaskId";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@IsCompleted", isCompleted ? 1 : 0);
            cmd.Parameters.AddWithValue("@TaskId", taskId);
            cmd.ExecuteNonQuery();

            connection.Close();

            // Log the action
            var status = isCompleted ? "Completed" : "Reopened";
            _logService.AddLog(1, "Task Updated", $"Task {taskId} marked as {status}");
        }

        public void DeleteTask(int taskId, int userId)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = "DELETE FROM Tasks WHERE TaskId = @TaskId";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@TaskId", taskId);
            cmd.ExecuteNonQuery();

            connection.Close();

            _logService.AddLog(userId, "Task Deleted", $"Task {taskId} was deleted");
        }

        public void AddReminder(int taskId, DateTime reminderDate, int userId)
        {
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = "UPDATE Tasks SET ReminderDate = @ReminderDate WHERE TaskId = @TaskId";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@ReminderDate", reminderDate.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@TaskId", taskId);
            cmd.ExecuteNonQuery();

            connection.Close();

            _logService.AddLog(userId, "Reminder Set", $"Reminder set for task {taskId} on {reminderDate:yyyy-MM-dd}");
        }

            public List<CybersecurityBotWPF.Models.Task> GetTasksDueForReminder(int userId)
        {
            var tasks = new List<CybersecurityBotWPF.Models.Task>();
            var today = DateTime.Now.Date;

            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = @"
                SELECT * FROM Tasks 
                WHERE UserId = @UserId 
                AND IsCompleted = 0 
                AND ReminderDate IS NOT NULL 
                AND date(ReminderDate) <= date(@Today)";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Today", today.ToString("yyyy-MM-dd"));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new CybersecurityBotWPF.Models.Task
                {
                    TaskId = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Title = reader.GetString(2),
                    Description = reader.GetString(3),
                    CreatedDate = DateTime.Parse(reader.GetString(4)),
                    ReminderDate = reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5)),
                    IsCompleted = reader.GetInt32(6) == 1
                });
            }

            connection.Close();
            return tasks;
        }
    }
}