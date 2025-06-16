using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System;
using Avalonia.Layout;

namespace Inv;

public partial class MessageView : UserControl
{
    // Property registrations
    public static readonly StyledProperty<string> SenderProperty =
        AvaloniaProperty.Register<MessageView, string>(nameof(Sender));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MessageView, string>(nameof(Text));

    public static readonly StyledProperty<bool> IsSentProperty =
        AvaloniaProperty.Register<MessageView, bool>(nameof(IsSent));

    public static readonly StyledProperty<bool> IsReadProperty =
        AvaloniaProperty.Register<MessageView, bool>(nameof(IsRead));

    public static readonly StyledProperty<DateTime> TimestampProperty =
        AvaloniaProperty.Register<MessageView, DateTime>(nameof(Timestamp));

    // Properties
    public string Sender
    {
        get => GetValue(SenderProperty);
        set => SetValue(SenderProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsSent
    {
        get => GetValue(IsSentProperty);
        set => SetValue(IsSentProperty, value);
    }

    public bool IsRead
    {
        get => GetValue(IsReadProperty);
        set => SetValue(IsReadProperty, value);
    }

    public DateTime Timestamp
    {
        get => GetValue(TimestampProperty);
        set => SetValue(TimestampProperty, value);
    }

    // Dynamic visual properties
    public HorizontalAlignment Alignment => IsSent ? HorizontalAlignment.Right : HorizontalAlignment.Left;
    public IBrush MessageBackground => IsSent ? Brushes.LightBlue : Brushes.LightGray;

    public MessageView()
    {
        InitializeComponent();
        Timestamp = DateTime.Now; // Default to current time
    }
}