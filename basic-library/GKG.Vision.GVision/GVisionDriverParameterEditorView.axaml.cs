using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace GKG
{
    namespace Vision
    {
        public partial class GVisionDriverParameterEditorView : UserControl
        {
            private byte[] data = Array.Empty<byte>();
            private TextBox? dataTextBox;
            private Button? applyButton;

            public GVisionDriverParameterEditorView()
            {
                InitializeComponent();
                dataTextBox = this.FindControl<TextBox>("DataTextBox");
                applyButton = this.FindControl<Button>("ApplyButton");
                if (applyButton != null)
                {
                    applyButton.Click += ApplyButton_Click;
                }
                RefreshText();
            }

            public event EventHandler? AfterModify;

            public void SetData(byte[] value)
            {
                data = value?.ToArray() ?? Array.Empty<byte>();
                RefreshText();
            }

            public byte[] GetData()
            {
                return data.ToArray();
            }

            public void ApplyModifiedData(byte[] value)
            {
                SetData(value);
                AfterModify?.Invoke(this, EventArgs.Empty);
            }

            private void InitializeComponent()
            {
                AvaloniaXamlLoader.Load(this);
            }

            private void RefreshText()
            {
                if (dataTextBox != null)
                {
                    dataTextBox.Text = Convert.ToBase64String(data);
                }
            }

            private void ApplyButton_Click(object? sender, RoutedEventArgs e)
            {
                try
                {
                    string text = dataTextBox?.Text ?? string.Empty;
                    data = string.IsNullOrWhiteSpace(text) ? Array.Empty<byte>() : Convert.FromBase64String(text);
                    AfterModify?.Invoke(this, EventArgs.Empty);
                }
                catch
                {
                    // 保持当前数据不变，避免非法 Base64 直接打断宿主流程。
                }
            }
        }
    }
}
