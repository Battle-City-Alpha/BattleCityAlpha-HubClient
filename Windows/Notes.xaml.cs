using hub_client.Configuration;
using hub_client.WindowsAdministrator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hub_client.Windows
{
    /// <summary>
    /// Logique d'interaction pour Notes.xaml
    /// </summary>
    public partial class Notes : Window
    {
        private NotesAdministrator _admin;
        private AppDesignConfig style = FormExecution.AppDesignConfig;
        string path;

        public Notes(NotesAdministrator admin)
        {
            InitializeComponent();
            _admin = admin;
            Loaded += Notes_Loaded;
            path = System.IO.Path.Combine(FormExecution.path, "notes.bca");

            this.MouseDown += Window_MouseDown;
        }

        private void Notes_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStyle();
            if (File.Exists(path))
                notesBox.SetText(File.ReadAllText(System.IO.Path.Combine(FormExecution.path, path)));
            notesBox.chat.IsReadOnly = false;
        }

        private void LoadStyle()
        {
            btnClear.Color1 = style.GetGameColor("Color1HomeHeadButton");
            btnClear.Color2 = style.GetGameColor("Color2HomeHeadButton");
            btnSave.Color1 = style.GetGameColor("Color1HomeHeadButton");
            btnSave.Color2 = style.GetGameColor("Color2HomeHeadButton");

            btnClear.Update();
            btnSave.Update();

            this.FontFamily = style.Font;              
        }

        private void btnSave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            File.WriteAllText(path, notesBox.GetText());
            _admin.Client.OpenPopBox("Contenu sauvegardé !", "Opération réussie");
        }

        private void btnClear_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            notesBox.Clear();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Loaded -= Notes_Loaded;
        }

        private void closeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        private void maximizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.bg_border.CornerRadius = new CornerRadius(50);
            }
            else if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.bg_border.CornerRadius = new CornerRadius(0);
            }
        }
        private void minimizeIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
