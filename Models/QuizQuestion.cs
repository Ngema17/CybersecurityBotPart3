using System.Collections.Generic;

namespace CybersecurityBotWPF.Models
{
    public class QuizQuestion
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsTrueFalse => Options.Count == 2 &&
                                   Options[0] == "True" &&
                                   Options[1] == "False";
    }

    public class QuizResult
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public double ScorePercentage => TotalQuestions > 0 ? (CorrectAnswers / (double)TotalQuestions) * 100 : 0;
        public string Feedback
        {
            get
            {
                if (ScorePercentage >= 90) return "🏆 Excellent! You're a Cybersecurity Pro!";
                if (ScorePercentage >= 70) return "👏 Good job! Keep learning to become a pro!";
                if (ScorePercentage >= 50) return "📖 Not bad! Review the topics you missed.";
                return "💪 Keep learning! Cybersecurity is important for everyone.";
            }
        }
        public List<int> IncorrectQuestions { get; set; } = new List<int>();
    }
}