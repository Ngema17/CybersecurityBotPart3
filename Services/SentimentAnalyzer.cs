using System;
using System.Collections.Generic;

namespace CybersecurityBotWPF.Services
{
    public enum SentimentType
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Happy
    }

    public class SentimentResult
    {
        public SentimentType SentimentType { get; set; }
        public string DetectedTopic { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    public class SentimentAnalyzer
    {
        private readonly Dictionary<string, SentimentType> _sentimentKeywords;
        private readonly Dictionary<string, string> _topicKeywords;

        public SentimentAnalyzer()
        {
            _sentimentKeywords = new Dictionary<string, SentimentType>(StringComparer.OrdinalIgnoreCase)
            {
                ["worried"] = SentimentType.Worried,
                ["concerned"] = SentimentType.Worried,
                ["scared"] = SentimentType.Worried,
                ["anxious"] = SentimentType.Worried,
                ["frustrated"] = SentimentType.Frustrated,
                ["annoyed"] = SentimentType.Frustrated,
                ["confused"] = SentimentType.Frustrated,
                ["curious"] = SentimentType.Curious,
                ["interested"] = SentimentType.Curious,
                ["happy"] = SentimentType.Happy,
                ["great"] = SentimentType.Happy,
                ["good"] = SentimentType.Happy
            };

            _topicKeywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["phish"] = "phishing",
                ["scam"] = "scams",
                ["password"] = "passwords",
                ["privacy"] = "privacy",
                ["browsing"] = "safe browsing"
            };
        }

        public SentimentResult AnalyzeSentiment(string input)
        {
            var result = new SentimentResult
            {
                SentimentType = SentimentType.Neutral,
                Confidence = 0.5
            };

            string lowerInput = input.ToLower();

            foreach (var keyword in _sentimentKeywords)
            {
                if (lowerInput.Contains(keyword.Key))
                {
                    result.SentimentType = keyword.Value;
                    result.Confidence = 0.85;
                    break;
                }
            }

            foreach (var topic in _topicKeywords)
            {
                if (lowerInput.Contains(topic.Key))
                {
                    result.DetectedTopic = topic.Value;
                    break;
                }
            }

            return result;
        }
    }
}