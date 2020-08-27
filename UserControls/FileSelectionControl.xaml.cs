using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Microsoft.Win32;

namespace C_V_App.UserControls
{
    /// <summary>
    /// Interaction logic for FileSelectionControl.xaml
    /// </summary>
    /// 
    [ContentProperty ("Label")]
    public partial class FileSelectionControl : UserControl
    {
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FileSelectionControl));

        public event EventHandler<EventArgs> FileNameChanged;

        public FileSelectionControl()
        {
            AllowCreateNewFile = false;
            FileDialogue = new OpenFileDialog();
            InitializeComponent();
            FileDialogTbx.TextChanged += new TextChangedEventHandler(OnFileNameChanged);
        }

        private OpenFileDialog FileDialogue { get; set; }

        public void FileDialogBtn_OnClick (object sender, RoutedEventArgs args)
        {
            FileDialogue.CheckFileExists = !AllowCreateNewFile;
            FileDialogue.CheckPathExists = !AllowCreateNewFile;
            if (FileDialogue.ShowDialog() == true)
            {
                FileName = FileDialogue.FileName;
            }
        }

        public bool AllowCreateNewFile { get; set; }

        public string Label
        {
            get { return this.FileDialogLbl.Content.ToString(); }
            set { this.FileDialogLbl.Content = value; }
        }

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public void OnFileNameChanged (object sender, TextChangedEventArgs args)
        {
            var local = FileNameChanged;
            if (local != null)
            {
                local(this, args);
            }
        }
    }
}
