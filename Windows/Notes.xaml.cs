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
            btnClear.Color1 = style.Color1HomeHeadButton;
            btnClear.Color2 = style.Color2HomeHeadButton;
            btnSave.Color1 = style.Color1HomeHeadButton;
            btnSave.Color2 = style.Color2HomeHeadButton;

            btnClear.Update();
            btnSave.Update();
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
    }
}
