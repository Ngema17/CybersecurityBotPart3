using System;
using System.Collections.Generic;
using CybersecurityBotWPF.Models;

namespace CybersecurityBotWPF.Services
{
    public class ResponseService
    {
        private readonly Dictionary<string, List<string>> _keywordResponses;
        private readonly Random _random;

        public ResponseService()
        {
            _random = new Random();
            _keywordResponses = InitializeKeywordResponses();
        }

        private Dictionary<string, List<string>> InitializeKeywordResponses() => new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["password"] = new List<string>
                {
                    "🔐 A strong password should be at least 12 characters long with uppercase, lowercase, numbers, and special characters.",
                    "🔐 Never reuse passwords across different accounts. Use a password manager.",
                    "🔐 Enable Two-Factor Authentication (2FA) for extra security.",
                    "🔐 Avoid using personal information like your name or birthdate in passwords."
                },
            ["phishing"] = new List<string>
                {
                    "🎣 Phishing attacks create urgency - 'Your account will be closed!' Always verify suspicious emails.",
                    "🎣 Check the sender's email address carefully. Scammers use addresses that look similar to real ones.",
                    "🎣 Never click links in unsolicited emails. Hover to see the actual URL.",
                    "🎣 Look for spelling mistakes - legitimate companies rarely make these errors."
                },
            ["scam"] = new List<string>
                {
                    "⚠️ If it sounds too good to be true, it probably is!",
                    "⚠️ Never share sensitive information over phone calls or messages.",
                    "⚠️ Scammers impersonate tech support and banks. Always verify through official channels.",
                    "⚠️ Be cautious of unsolicited job offers or lottery winnings."
                },
            ["privacy"] = new List<string>
                {
                    "🛡️ Review app permissions regularly - many apps request access they don't need.",
                    "🛡️ Use privacy-focused browsers to reduce tracking.",
                    "🛡️ Be mindful of what you share on social media.",
                    "🛡️ Regularly check your social media privacy settings."
                },
            ["browsing"] = new List<string>
                {
                    "🌐 Always check for 'https://' and the padlock icon before entering sensitive information.",
                    "🌐 Use browser extensions like ad-blockers to protect against malicious ads.",
                    "🌐 Clear your browsing history and cookies regularly.",
                    "🌐 Avoid using public Wi-Fi for sensitive transactions."
                },
            ["how are you"] = new List<string>
                {
                    "I'm functioning optimally and ready to help you stay cyber-safe!",
                    "All systems secure! How can I assist you today?",
                    "I'm doing great! Ready to share security tips!"

        },
        };

        public string GetResponse(string input, UserProfile user, SentimentResult sentiment)
        {
            string lowerInput = input.ToLower();

            // Follow-up responses
            if (lowerInput.Contains("tell me more") || lowerInput.Contains("explain more") ||
                lowerInput.Contains("another tip") || lowerInput.Contains("more"))
            {
                if (!string.IsNullOrEmpty(user.CurrentTopic))
                {
                    return GetResponseForTopic(user.CurrentTopic);
                }
                return "What specific topic would you like to learn more about?";
            }

            // Memory feature - remember user interests
            if (lowerInput.Contains("interested in") || lowerInput.Contains("i like") ||
                lowerInput.Contains("tell me about"))
            {
                foreach (var topic in _keywordResponses.Keys)
                {
                    if (lowerInput.Contains(topic))
                    {
                        user.FavoriteTopic = topic;
                        user.CurrentTopic = topic;
                        return $"📝 Great! I'll remember that you're interested in {topic}.\n\n{GetResponseForTopic(topic)}";
                    }
                }
            }

            // Sentiment-based responses
            if (sentiment.SentimentType == SentimentType.Worried)
            {
                string topic = string.IsNullOrEmpty(sentiment.DetectedTopic) ? "cybersecurity" : sentiment.DetectedTopic;
                return $"😟 I understand your concern. It's normal to feel worried. Here's some help:\n\n{GetResponseForTopic(topic)}";
            }

            if (sentiment.SentimentType == SentimentType.Frustrated)
            {
                string topic = string.IsNullOrEmpty(sentiment.DetectedTopic) ? "security" : sentiment.DetectedTopic;
                return $"💪 I hear your frustration. Let me simplify this for you:\n\n{GetResponseForTopic(topic)}";
            }

            // Keyword matching
            foreach (var keyword in _keywordResponses.Keys)
            {
                if (lowerInput.Contains(keyword))
                {
                    user.CurrentTopic = keyword;
                    return GetRandomResponse(keyword);
                }
            }

            // Tips
            if (lowerInput.Contains("tip"))
            {
                if (lowerInput.Contains("phishing"))
                    return "💡 Tip: Legitimate companies never ask for sensitive information via email.";
                if (lowerInput.Contains("password"))
                    return "💡 Tip: Use passphrases like 'Correct-Horse-Battery-Staple' for strong, memorable passwords.";
                return "💡 Tip: Always keep your software updated to protect against known vulnerabilities!";
            }

            // Default with memory recall
            if (!string.IsNullOrEmpty(user.FavoriteTopic))
            {
                return $"I'm not sure I understand. Since you're interested in {user.FavoriteTopic}, would you like me to share more about that? Or ask me about passwords, phishing, scams, privacy, or safe browsing.";
            }

            // Default responses
            string[] defaults = {
                "I'm not sure I understand. Try asking about passwords, phishing, scams, privacy, or safe browsing.",
                "Hmm, I didn't catch that. Ask me for a 'phishing tip' or about 'password safety'!",
                "Let's focus on cybersecurity. What would you like to know about?"
            };
            return defaults[_random.Next(defaults.Length)];
        }

        private string GetResponseForTopic(string topic)
        {
            if (_keywordResponses.ContainsKey(topic))
            {
                return GetRandomResponse(topic);
            }
            return "What specific aspect would you like to know about?";
        }

        private string GetRandomResponse(string keyword)
        {
            if (_keywordResponses.ContainsKey(keyword))
            {
                var responses = _keywordResponses[keyword];
                return responses[_random.Next(responses.Count)];
            }
            return GetDefaultResponse();
        }

        private string GetDefaultResponse()
        {
            string[] defaults = {
                "I didn't quite catch that. Could you ask about passwords, phishing, scams, privacy, or safe browsing?",
                "Try asking me for a 'phishing tip' or about 'password safety'!"
            };
            return defaults[_random.Next(defaults.Length)];
        }
    }
}