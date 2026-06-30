using System.Collections.Generic;

namespace CybersecurityBotWPF.Models
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FavoriteTopic { get; set; } = string.Empty;
        public List<string> DiscussedTopics { get; set; } = new List<string>();
        public string CurrentTopic { get; set; } = string.Empty;

        public void AddDiscussedTopic(string topic)
        {
            if (!DiscussedTopics.Contains(topic))
            {
                DiscussedTopics.Add(topic);
            }
        }

        public bool HasDiscussedTopic(string topic)
        {
            return DiscussedTopics.Contains(topic);
        }
    }
}