using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Notebook
{
    public sealed class NoteModel : INotifyPropertyChanged
    {
        private string _text = string.Empty;
        private DateTimeOffset _modifiedAt;

        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTimeOffset ModifiedAt
        {
            get => _modifiedAt;
            set
            {
                if (_modifiedAt != value)
                {
                    _modifiedAt = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ModifiedText));
                }
            }
        }

        public string ModifiedText => ModifiedAt.ToString("M/d/yyyy h:mm:ss tt");

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public sealed partial class MainPage : Page
    {
        public ObservableCollection<NoteModel> Notes { get; } = new()
        {
            new NoteModel
            {
                Text = "By incorporating the Fluent Design System into all experiences, controls, and styles, WinUI provides consistent, intuitive, and...",
                ModifiedAt = new DateTimeOffset(2025, 4, 2, 15, 16, 0, TimeSpan.Zero)
            },
            new NoteModel
            {
                Text = "WinUI is a XAML markup-based user interface layer that contains modern controls and styles for building Windows apps",
                ModifiedAt = new DateTimeOffset(2025, 3, 31, 19, 8, 54, TimeSpan.Zero)
            },
            new NoteModel
            {
                Text = "Hello, WinUI!",
                ModifiedAt = new DateTimeOffset(2025, 3, 31, 19, 7, 16, TimeSpan.Zero)
            }
        };

        public MainPage()
        {
            InitializeComponent();
        }

        private async void NewNoteButton_Click(object sender, RoutedEventArgs e)
        {
            await EditNoteAsync(null);
        }

        private async void NotesGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is NoteModel note)
            {
                await EditNoteAsync(note);
            }
        }

        private async void NoteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && NotesGrid.SelectedItem is NoteModel note)
            {
                if (menuItem.Tag?.ToString() == "delete")
                {
                    Notes.Remove(note);
                }
                else
                {
                    await EditNoteAsync(note);
                }
            }
        }

        private async System.Threading.Tasks.Task EditNoteAsync(NoteModel? note)
        {
            var textBox = new TextBox
            {
                Text = note?.Text ?? string.Empty,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                PlaceholderText = "Enter your note",
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black),
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                PlaceholderForeground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkGray),
                MinHeight = 220,
                FontSize = 16
            };

            var dialog = new ContentDialog
            {
                Title = note is null ? "New note" : "Edit note",
                Content = textBox,
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };

            if (await dialog.ShowAsync() != ContentDialogResult.Primary)
            {
                return;
            }

            var text = textBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            if (note is null)
            {
                Notes.Insert(0, new NoteModel
                {
                    Text = text,
                    ModifiedAt = DateTimeOffset.Now
                });
            }
            else
            {
                note.Text = text;
                note.ModifiedAt = DateTimeOffset.Now;
            }
        }
    }
}
