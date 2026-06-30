using CybersecurityBotPart3.Services;
using CybersecurityBotWPF.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityBotWPF.Services
{
    public class QuizService
    {
        private readonly DatabaseService _dbService;
        private readonly ActivityLogService _logService;
        private readonly List<QuizQuestion> _questions;

        public QuizService(ActivityLogService logService)
        {
            _dbService = new DatabaseService();
            _logService = logService;
            _questions = InitializeQuestions();
        }

        private List<QuizQuestion> InitializeQuestions()
        {
            return new List<QuizQuestion>
            {
                // Multiple Choice Questions
                new QuizQuestion
                {
                    QuestionId = 1,
                    QuestionText = "What does phishing refer to?",
                    Options = new List<string> {
                        "A type of computer virus",
                        "A scam to steal personal information",
                        "A method of securing passwords",
                        "A type of firewall"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Phishing is a cyber attack where scammers impersonate trusted entities to steal sensitive information.",
                    Category = "Phishing"
                },
                new QuizQuestion
                {
                    QuestionId = 2,
                    QuestionText = "What is a strong password practice?",
                    Options = new List<string> {
                        "Using your date of birth",
                        "Using the same password for all accounts",
                        "Using a mix of uppercase, lowercase, numbers, and symbols",
                        "Using your pet's name"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "A strong password should be at least 12 characters long with a mix of character types.",
                    Category = "Password Safety"
                },
                new QuizQuestion
                {
                    QuestionId = 3,
                    QuestionText = "What should you do if you receive a suspicious email?",
                    Options = new List<string> {
                        "Reply with your personal information",
                        "Click the links to check",
                        "Report it as phishing and delete it",
                        "Forward it to your friends"
                    },
                    CorrectAnswerIndex = 2,
                    Explanation = "Always report suspicious emails to your IT department or email provider.",
                    Category = "Phishing"
                },
                new QuizQuestion
                {
                    QuestionId = 4,
                    QuestionText = "What does HTTPS stand for?",
                    Options = new List<string> {
                        "Hyper Text Transfer Protocol Secure",
                        "High Tech Transfer Protocol System",
                        "Hyper Transfer Text Protocol Secure",
                        "Secure Hyper Text Protocol"
                    },
                    CorrectAnswerIndex = 0,
                    Explanation = "HTTPS ensures secure communication between your browser and the website.",
                    Category = "Safe Browsing"
                },
                new QuizQuestion
                {
                    QuestionId = 5,
                    QuestionText = "Which of the following is NOT a good cybersecurity practice?",
                    Options = new List<string> {
                        "Using two-factor authentication",
                        "Sharing passwords with friends",
                        "Regularly updating software",
                        "Using a password manager"
                    },
                    CorrectAnswerIndex = 1,
                    Explanation = "Passwords should never be shared with others, even friends or family.",
                    Category = "Password Safety"
                },

                // True/False Questions
                new QuizQuestion
                {
                    QuestionId = 6,
                    QuestionText = "Two-Factor Authentication (2FA) adds an extra layer of security.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "2FA requires a second verification method, making it much harder for attackers to access accounts.",
                    Category = "Security"
                },
                new QuizQuestion
                {
                    QuestionId = 7,
                    QuestionText = "Public Wi-Fi networks are always safe to use for banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Public Wi-Fi can be insecure. Use a VPN for sensitive transactions.",
                    Category = "Safe Browsing"
                },
                new QuizQuestion
                {
                    QuestionId = 8,
                    QuestionText = "Social engineering is a technique used by hackers to manipulate people.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Social engineering exploits human psychology rather than technical vulnerabilities.",
                    Category = "Social Engineering"
                },
                new QuizQuestion
                {
                    QuestionId = 9,
                    QuestionText = "Using the same password for multiple accounts is safe.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 1,
                    Explanation = "If one account is compromised, all accounts using the same password are at risk.",
                    Category = "Password Safety"
                },
                new QuizQuestion
                {
                    QuestionId = 10,
                    QuestionText = "Ransomware is a type of malware that locks your files and demands payment.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Ransomware encrypts files and demands a ransom for decryption.",
                    Category = "Malware"
                },
                new QuizQuestion
                {
                    QuestionId = 11,
                    QuestionText = "Should you click on links in emails from unknown senders?",
                    Options = new List<string> { "Yes", "No" },
                    CorrectAnswerIndex = 1,
                    Explanation = "Never click on links from unknown senders as they may lead to phishing sites.",
                    Category = "Phishing"
                },
                new QuizQuestion
                {
                    QuestionId = 12,
                    QuestionText = "Antivirus software should be kept up to date to protect against new threats.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswerIndex = 0,
                    Explanation = "Regular updates ensure antivirus software can detect the latest threats.",
                    Category = "Security"
                }
            };
        }

        public List<QuizQuestion> GetQuestions()
        {
            return _questions;
        }

        public async System.Threading.Tasks.Task<QuizResult> SubmitQuiz(int userId, List<int> userAnswers)
        {
            var result = new QuizResult
            {
                TotalQuestions = _questions.Count,
                CorrectAnswers = 0,
                IncorrectQuestions = new List<int>()
            };

            for (int i = 0; i < _questions.Count && i < userAnswers.Count; i++)
            {
                if (userAnswers[i] == _questions[i].CorrectAnswerIndex)
                {
                    result.CorrectAnswers++;
                }
                else
                {
                    result.IncorrectQuestions.Add(i);
                }
            }

            // Save to database
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = @"
                INSERT INTO QuizResults (UserId, TotalQuestions, CorrectAnswers, Score, DateTaken)
                VALUES (@UserId, @TotalQuestions, @CorrectAnswers, @Score, @DateTaken)";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@TotalQuestions", result.TotalQuestions);
            cmd.Parameters.AddWithValue("@CorrectAnswers", result.CorrectAnswers);
            cmd.Parameters.AddWithValue("@Score", result.ScorePercentage);
            cmd.Parameters.AddWithValue("@DateTaken", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.ExecuteNonQuery();

            connection.Close();

            // Log the action
            _logService.AddLog(userId, "Quiz Completed", $"Score: {result.CorrectAnswers}/{result.TotalQuestions} ({result.ScorePercentage:F1}%)");

            return result;
        }

        public List<QuizResult> GetQuizHistory(int userId)
        {
            var results = new List<QuizResult>();
            using var connection = _dbService.GetConnection();
            connection.Open();

            var query = "SELECT * FROM QuizResults WHERE UserId = @UserId ORDER BY DateTaken DESC LIMIT 10";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new QuizResult
                {
                    TotalQuestions = reader.GetInt32(2),
                    CorrectAnswers = reader.GetInt32(3),
                });
            }

            connection.Close();
            return results;
        }
    }
}