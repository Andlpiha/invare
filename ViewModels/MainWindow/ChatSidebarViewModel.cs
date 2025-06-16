using System;
using System.Collections.Generic;
using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Avalonia.Metadata;
using Eremex.AvaloniaUI.Charts;
using ReactiveUI;
using System.Collections.ObjectModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inv.ViewModels.MainWindow
{
    public class Message
    {
        public string Sender { get; set; }
        public string MessageText { get; set; }

        // Если эта переменная равна 1, значит текущий пользователь получил сообщение, иначе
        // он отправил это сообщение
        public bool IsSent { get; }

        public Message(string sender, string messageText, bool sentOrReceived = true)
        {
            this.Sender = sender;
            this.MessageText = messageText;
            this.IsSent = sentOrReceived;
        }
    }

    public class ChatTemplatesSelector : IDataTemplate
    {
        // This Dictionary should store our shapes. We mark this as [Content], so we can directly add elements to it later.
        [Content]
        public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new Dictionary<string, IDataTemplate>();

        // Build the DataTemplate here
        public Control Build(object? param)
        {
            var key = getDictKey(param as ChatSidebarViewModel);
            if (key is null) // If the key is null, we throw an ArgumentNullException
            {
                throw new ArgumentNullException(nameof(param));
            }

            return AvailableTemplates[key].Build(param)!; // finally we look up the provided key and let the System build the DataTemplate for us
        }

        // Check if we can accept the provided data
        public bool Match(object? data)
        {
            var key = getDictKey(data as ChatSidebarViewModel);

            return key != null
                && AvailableTemplates.ContainsKey(key);
        }

        private string? getDictKey(ChatSidebarViewModel? data)
        {
            if (data is null)
                return null;
            if (data.selectedRepairID == -1)
                return "RepairNotSelected";
            else if (data.Messages.Count == 0)
                return "NoMessages";
            return "MessagesNotEmpty";
        }
    }

    public class ChatSidebarViewModel : ReactiveObject
	{
        public ObservableCollection<Message> Messages { get; set; } = new();
        public int selectedRepairID { get; set; } = 0;
    }
}