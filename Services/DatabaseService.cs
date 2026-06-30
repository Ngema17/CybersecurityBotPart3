using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.IO;

namespace CybersecurityBotWPF.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CybersecurityBot.db");
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Create Users table
            var createUsersTable = @"
                CREATE TABLE IF NOT EXISTS Users (
                    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    FavoriteTopic TEXT,
                    CreatedDate TEXT NOT NULL
                )";
            using var cmd = new SqliteCommand(createUsersTable, connection);
            cmd.ExecuteNonQuery();

            // Create Tasks table
            var createTasksTable = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    TaskId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    CreatedDate TEXT NOT NULL,
                    ReminderDate TEXT,
                    IsCompleted INTEGER DEFAULT 0,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId)
                )";
            using var cmd2 = new SqliteCommand(createTasksTable, connection);
            cmd2.ExecuteNonQuery();

            // Create QuizResults table
            var createQuizTable = @"
                CREATE TABLE IF NOT EXISTS QuizResults (
                    ResultId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    TotalQuestions INTEGER NOT NULL,
                    CorrectAnswers INTEGER NOT NULL,
                    Score REAL NOT NULL,
                    DateTaken TEXT NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId)
                )";
            using var cmd3 = new SqliteCommand(createQuizTable, connection);
            cmd3.ExecuteNonQuery();

            // Create ActivityLog table
            var createLogTable = @"
                CREATE TABLE IF NOT EXISTS ActivityLog (
                    LogId INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Action TEXT NOT NULL,
                    Details TEXT,
                    Timestamp TEXT NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(UserId)
                )";
            using var cmd4 = new SqliteCommand(createLogTable, connection);
            cmd4.ExecuteNonQuery();

            connection.Close();
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}