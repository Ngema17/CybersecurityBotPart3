using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CybersecurityBotWPF.Models;

namespace CybersecurityBotWPF.Services
{
    public class NLPService
    {
        private readonly Dictionary<string, List<string>> _intentPatterns;
        private readonly Dictionary<string, string> _intentActions;

        public NLPService()
        {
            _intentPatterns = InitializePatterns();
            _intentActions = InitializeActions();
        }

        private Dictionary<string, List<string>> InitializePatterns()
        {
            return new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["add_task"] = new List<string>
                {
                    @"add\s+task",
                    @"create\s+task",
                    @"new\s+task",
                    @"add\s+.*task",
                    @"task.*add",
                    @"create.*task",
                    @"add.*reminder",
                    @"set.*reminder",
                    @"remind.*to",
                    @"remind.*about"
                },
                ["show_tasks"] = new List<string>
                {
                    @"show.*task",
                    @"view.*task",
                    @"list.*task",
                    @"display.*task",
                    @"what.*task",
                    @"my.*task",
                    @"pending.*task",
                    @"tasks"
                },
                ["complete_task"] = new List<string>
                {
                    @"complete.*task",
                    @"done.*task",
                    @"finish.*task",
                    @"mark.*complete",
                    @"task.*done",
                    @"complete.*#",
                    @"done.*#"
                },
                ["delete_task"] = new List<string>
                {
                    @"delete.*task",
                    @"remove.*task",
                    @"cancel.*task",
                    @"task.*delete",
                    @"delete.*#"
                },
                ["start_quiz"] = new List<string>
                {
                    @"start.*quiz",
                    @"take.*quiz",
                    @"do.*quiz",
                    @"quiz.*me",
                    @"test.*cyber",
                    @"play.*quiz",
                    @"start.*test"
                },
                ["show_log"] = new List<string>
                {
                    @"show.*log",
                    @"view.*log",
                    @"activity.*log",
                    @"what.*done",
                    @"recent.*action",
                    @"log",
                    @"history"
                },
                ["greeting"] = new List<string>
                {
                    @"hello",
                    @"hi",
                    @"hey",
                    @"good morning",
                    @"good afternoon",
                    @"good evening"
                },
                ["help"] = new List<string>
                {
                    @"help",
                    @"what can you do",
                    @"commands",
                    @"features"
                }
            };
        }

        private Dictionary<string, string> InitializeActions()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["add_task"] = "I'll help you add a cybersecurity task. Please tell me the task title:",
                ["show_tasks"] = "Here are your tasks:",
                ["complete_task"] = "I'll mark that task as completed.",
                ["delete_task"] = "I'll remove that task from your list.",
                ["start_quiz"] = "Starting the cybersecurity quiz!",
                ["show_log"] = "Here's your recent activity:",
                ["greeting"] = "Hello! How can I help you with cybersecurity today?",
                ["help"] = "I can help you with:\n- Adding tasks (e.g., 'Add task to enable 2FA')\n- Viewing tasks\n- Completing tasks\n- Starting a quiz\n- Viewing activity log\n- Getting cybersecurity tips"
            };
        }

        public string DetectIntent(string input, out string intent, out string extractedData)
        {
            intent = "unknown";
            extractedData = "";

            foreach (var pattern in _intentPatterns)
            {
                foreach (var regex in pattern.Value)
                {
                    var match = Regex.Match(input, regex, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        intent = pattern.Key;
                        // Extract additional data from the input
                        extractedData = ExtractData(input, intent);
                        return _intentActions.TryGetValue(intent, out var response) ? response : "";
                    }
                }
            }

            // Check for cybersecurity topics (fallback to ResponseService)
            foreach (var keyword in new[] { "password", "phishing", "scam", "privacy", "browsing" })
            {
                if (input.ToLower().Contains(keyword))
                {
                    intent = "cybersecurity_topic";
                    extractedData = keyword;
                    return _intentActions.TryGetValue("help", out var response) ? response : "";
                }
            }

            return "I'm not sure about that. Would you like me to help you with tasks, a quiz, or cybersecurity tips?";
        }

        private string ExtractData(string input, string intent)
        {
            switch (intent)
            {
                case "add_task":
                    // Extract task description after "add task", "create task", etc.
                    var patterns = new[] { "add task", "create task", "new task", "remind me to", "remind me about" };
                    foreach (var pattern in patterns)
                    {
                        var index = input.ToLower().IndexOf(pattern.ToLower());
                        if (index >= 0)
                        {
                            var result = input.Substring(index + pattern.Length).Trim();
                            if (!string.IsNullOrEmpty(result))
                                return result;
                        }
                    }
                    return input; // Fallback
                case "complete_task":
                case "delete_task":
                    // Try to extract task ID or description
                    var match = Regex.Match(input, @"#(\d+)|(?:complete|delete|done)\s+(.+)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        if (match.Groups[1].Success)
                            return match.Groups[1].Value;
                        if (match.Groups[2].Success)
                            return match.Groups[2].Value;
                    }
                    return "task";
                default:
                    return "";
            }
        }

        public string GetIntentResponse(string intent, string extractedData, UserProfile user)
        {
            switch (intent)
            {
                case "add_task":
                    return $"I'll help you add the task: '{extractedData}'. Would you like to set a reminder? (yes/no)";
                case "show_tasks":
                    return "Let me show you your pending tasks...";
                case "complete_task":
                    return $"I'll mark task '{extractedData}' as completed. Good job!";
                case "delete_task":
                    return $"I'll remove task '{extractedData}' from your list.";
                case "start_quiz":
                    return "Great! Let's test your cybersecurity knowledge with a quiz!";
                case "show_log":
                    return "Here's a summary of your recent activities:";
                case "greeting":
                    return $"Hello {user.Name}! How can I help you stay safe online today?";
                case "help":
                    return _intentActions["help"];
                case "cybersecurity_topic":
                    return $"Let me share some tips about {extractedData}...";
                default:
                    return "I'm not quite sure what you want. Try saying 'help' to see what I can do!";
            }
        }
    }
}