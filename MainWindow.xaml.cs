using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CybersecurityBotPart3
{
    public partial class MainWindow : Window
    {
        private bool _waitingForName = true;
        private string _userName = "";

        public MainWindow()
        {
            InitializeComponent();

            // Wire up event handlers in code-behind
            this.Loaded += MainWindow_Loaded;
            this.TaskButton.Click += TaskButton_Click;
            this.QuizButton.Click += QuizButton_Click;
            this.LogButton.Click += LogButton_Click;
            this.SendButton.Click += SendButton_Click;
            this.ExitButton.Click += ExitButton_Click;
            this.UserInputBox.KeyDown += UserInputBox_KeyDown;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await AddBotMessage("Welcome to the Cybersecurity Awareness Bot!", "#64C8FF");
            await AddBotMessage("I am here to help you stay safe online.", "#96FF96");
            await AddBotMessage("Please enter your name:", "#FFD700");
            UserInputBox.Focus();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void UserInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                e.Handled = true;
                await ProcessUserInput();
            }
        }

        private async Task ProcessUserInput()
        {
            string input = UserInputBox.Text.Trim();
            UserInputBox.Clear();

            if (string.IsNullOrWhiteSpace(input))
            {
                await AddBotMessage("Please enter something before sending.", "#FF6666");
                return;
            }

            // Handle name input
            if (_waitingForName)
            {
                _userName = FormatName(input);
                _waitingForName = false;
                await AddUserMessage(input);
                await AddBotMessage($"Hello, {_userName}! Welcome!", "#FF66CC");
                await AddBotMessage("I can help you with:", "#FFFFFF");
                await AddBotMessage("• Cybersecurity tips (password, phishing, scams)", "#C8C864");
                await AddBotMessage("• Task management (add, view, complete tasks)", "#C8C864");
                await AddBotMessage("• Cybersecurity quiz", "#C8C864");
                await AddBotMessage("• View activity log", "#C8C864");
                await AddDivider();
                return;
            }

            // Handle exit
            if (input.ToLower() == "exit")
            {
                await AddUserMessage(input);
                await AddBotMessage($"Goodbye, {_userName}. Stay safe online!", "#66FF66");
                await Task.Delay(1500);
                Application.Current.Shutdown();
                return;
            }

            // Handle user input
            await AddUserMessage(input);

            // Update status
            StatusText.Text = "● Bot is typing...";
            StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA500"));

            // Process response (placeholder - will be replaced with actual logic)
            string response = GetResponse(input);
            await TypeMessage(response);

            // Reset status
            StatusText.Text = "● Ready";
            StatusText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64C864"));
        }

        private string GetResponse(string input)
        {
            string lower = input.ToLower();

            if (lower.Contains("password"))
                return "🔐 A strong password should be at least 12 characters long with uppercase, lowercase, numbers, and special characters. Enable Two-Factor Authentication for extra security!";

            if (lower.Contains("phishing"))
                return "🎣 Phishing attacks create urgency. Always verify suspicious emails by checking the sender's address and never click links in unsolicited messages.";

            if (lower.Contains("scam"))
                return "⚠️ If it sounds too good to be true, it probably is! Never share sensitive information over phone calls or messages from unknown sources.";

            if (lower.Contains("privacy"))
                return "🛡️ Review app permissions regularly and use privacy-focused browsers. Be mindful of what you share on social media.";

            if (lower.Contains("task") || lower.Contains("reminder"))
                return "📋 To manage tasks, use the 'Tasks' button above. You can add, view, and complete cybersecurity tasks!";

            if (lower.Contains("quiz") || lower.Contains("test"))
                return "🎯 Click the 'Quiz' button above to test your cybersecurity knowledge!";

            if (lower.Contains("log") || lower.Contains("history"))
                return "📊 Click the 'Log' button above to view your recent activity history.";

            if (lower.Contains("help") || lower.Contains("what can you do"))
            {
                return "🤖 I can help you with:\n" +
                       "• Cybersecurity tips (password, phishing, scams, privacy)\n" +
                       "• Task management (use the 'Tasks' button)\n" +
                       "• Cybersecurity quiz (use the 'Quiz' button)\n" +
                       "• View activity log (use the 'Log' button)\n" +
                       "• Type 'exit' to close the application";
            }

            if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey"))
                return $"Hello {_userName}! How can I help you with cybersecurity today?";

            if (lower.Contains("thank"))
                return "You're very welcome! Stay safe online! 🛡️";

            // Default response with helpful suggestions
            return "I'm not sure about that. Try asking about:\n" +
                   "• Password safety\n" +
                   "• Phishing attacks\n" +
                   "• Online scams\n" +
                   "• Privacy protection\n" +
                   "• Or use the buttons above for Tasks, Quiz, or Log!";
        }

        private async Task TypeMessage(string message)
        {
            SendButton.IsEnabled = false;
            UserInputBox.IsEnabled = false;

            var messagePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            var avatar = new TextBlock
            {
                Text = "🤖 ",
                FontSize = 16,
                Foreground = Brushes.White
            };

            var messageBlock = new TextBlock
            {
                Text = "",
                FontSize = 13,
                Foreground = new SolidColorBrush(Colors.LightGray),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 500
            };

            messagePanel.Children.Add(avatar);
            messagePanel.Children.Add(messageBlock);

            await Dispatcher.InvokeAsync(() => ChatPanel.Children.Add(messagePanel));

            // Typing effect
            for (int i = 0; i <= message.Length; i++)
            {
                messageBlock.Text = message.Substring(0, i);
                await Task.Delay(10);
                ScrollToBottom();
            }

            await Dispatcher.InvokeAsync(() => ChatPanel.Children.Add(new TextBlock { Height = 5 }));
            ScrollToBottom();

            SendButton.IsEnabled = true;
            UserInputBox.IsEnabled = true;
            UserInputBox.Focus();
        }

        private async Task AddBotMessage(string message, string colorHex)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                var converter = new BrushConverter();
                var messagePanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5)
                };

                var avatar = new TextBlock
                {
                    Text = "🤖 ",
                    FontSize = 16,
                    Foreground = Brushes.White
                };

                var messageBlock = new TextBlock
                {
                    Text = message,
                    FontSize = 13,
                    Foreground = (Brush)converter.ConvertFromString(colorHex),
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 500
                };

                messagePanel.Children.Add(avatar);
                messagePanel.Children.Add(messageBlock);
                ChatPanel.Children.Add(messagePanel);
                ChatPanel.Children.Add(new TextBlock { Height = 5 });
                ScrollToBottom();
            });
            await Task.Delay(50);
        }

        private async Task AddUserMessage(string message)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                var messagePanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 5, 0, 5),
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                var messageBlock = new TextBlock
                {
                    Text = message,
                    FontSize = 13,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64C8FF")),
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 400,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#28334A")),
                    Padding = new Thickness(10, 5, 10, 5)
                };

                var avatar = new TextBlock
                {
                    Text = " 👤",
                    FontSize = 16,
                    Foreground = Brushes.White
                };

                messagePanel.Children.Add(messageBlock);
                messagePanel.Children.Add(avatar);
                ChatPanel.Children.Add(messagePanel);
                ChatPanel.Children.Add(new TextBlock { Height = 5 });
                ScrollToBottom();
            });
            await Task.Delay(50);
        }

        private async Task AddDivider()
        {
            await Dispatcher.InvokeAsync(() =>
            {
                ChatPanel.Children.Add(new TextBlock
                {
                    Text = "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━",
                    Foreground = Brushes.Gray,
                    FontSize = 10,
                    Margin = new Thickness(0, 5, 0, 5)
                });
                ScrollToBottom();
            });
        }

        private void ScrollToBottom()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatScrollViewer.ScrollToBottom();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private string FormatName(string name)
        {
            name = name.Trim();
            if (string.IsNullOrEmpty(name)) return "User";
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        // ====== Feature Button Click Handlers ======
        private async void TaskButton_Click(object sender, RoutedEventArgs e)
        {
            await AddBotMessage("📋 **Task Management**", "#FFA500");
            await AddBotMessage("You can manage your cybersecurity tasks here.", "#FFFFFF");
            await AddBotMessage("• Type 'add task' to create a new task", "#C8C864");
            await AddBotMessage("• Type 'view tasks' to see your pending tasks", "#C8C864");
            await AddBotMessage("• Type 'complete task' to mark a task as done", "#C8C864");
            await AddBotMessage("• Type 'delete task' to remove a task", "#C8C864");
            await AddDivider();
        }

        private async void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            await AddBotMessage("🎯 **Cybersecurity Quiz**", "#28A745");
            await AddBotMessage("Test your cybersecurity knowledge!", "#FFFFFF");
            await AddBotMessage("Type 'start quiz' to begin the quiz.", "#C8C864");
            await AddBotMessage("The quiz will have 12 questions covering:", "#C8C864");
            await AddBotMessage("• Password safety", "#C8C864");
            await AddBotMessage("• Phishing awareness", "#C8C864");
            await AddBotMessage("• Safe browsing", "#C8C864");
            await AddBotMessage("• Social engineering", "#C8C864");
            await AddDivider();
        }

        private async void LogButton_Click(object sender, RoutedEventArgs e)
        {
            await AddBotMessage("📊 **Activity Log**", "#FFA500");
            await AddBotMessage("Here's a summary of your recent actions:", "#FFFFFF");
            await AddBotMessage("1. You started the Cybersecurity Awareness Bot", "#C8C864");
            await AddBotMessage("2. You entered your name", "#C8C864");
            await AddBotMessage("3. You explored the features", "#C8C864");
            await AddBotMessage("Type 'show log' to see your full activity history.", "#C8C864");
            await AddDivider();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}