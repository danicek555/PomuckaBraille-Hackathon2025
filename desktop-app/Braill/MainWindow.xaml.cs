using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Windows.Media.SpeechSynthesis;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.Media.Core;
using MediaPlayer = Windows.Media.Playback.MediaPlayer;

namespace Braill
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool detekovan = false;
        bool isDarkMode = false;
        string adresaMB = "";
        string obsahSouboru = "";


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            detekovan = false;
            foreach (var drive in DriveInfo.GetDrives())
            {
               if(drive.VolumeLabel == "MICROBIT")
                {
                    detekovan = true;
                    adresaMB = drive.RootDirectory.ToString();
                }
            }
            if (detekovan)
            {
                try
                {
                    obsahSouboru = File.ReadAllText(adresaMB + "DATA.txt");
                }
                catch (Exception vyjimka)
                {
                    MessageBox.Show("CONNECTION SUCCESSFUL\n\nError - data file not found");
                }
                tbxObsah.Text = obsahSouboru;
            }
            else
            {
                tbxObsah.Text = "MICROBIT NOT FOUND - PLEASE CONNECT VIA USB.";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button toggleButton = sender as Button;

            btnMode.Content = isDarkMode ? "Dark Mode" : "Light Mode";
            if (!isDarkMode)
            {
                // Aktivace dark mode
                this.Background = new SolidColorBrush(Color.FromRgb(30, 30, 30));

                foreach (var child in ((StackPanel)this.Content).Children)
                {
                    if (child is Label label)
                        label.Foreground = Brushes.White;

                    if (child is Button btn)
                    {
                        btn.Background = new SolidColorBrush(Color.FromRgb(80, 80, 80));
                        btn.Foreground = Brushes.White;
                    }

                    if (child is TextBox textBox)
                    {
                        textBox.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                        textBox.Foreground = Brushes.White;
                        textBox.BorderBrush = Brushes.Gray;
                    }
                }

                toggleButton.Content = "Light Mode";
                isDarkMode = true;
            }
            else
            {
                // Zpět do světlého režimu
                this.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));

                foreach (var child in ((StackPanel)this.Content).Children)
                {
                    if (child is Label label)
                        label.Foreground = Brushes.Black;

                    if (child is Button btn)
                    {
                        btn.Background = (string)btn.Content == "CONNECT" ? new SolidColorBrush(Color.FromRgb(0, 122, 204)) : new SolidColorBrush(Color.FromRgb(68, 68, 68));
                        btn.Foreground = Brushes.White;
                    }

                    if (child is TextBox textBox)
                    {
                        textBox.Background = Brushes.White;
                        textBox.Foreground = Brushes.Black;
                        textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204));
                    }
                }

                isDarkMode = false;
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var synthesizer = new SpeechSynthesizer();

            // Najdi hlas "Jakub"
            var czVoice = SpeechSynthesizer.AllVoices
                .FirstOrDefault(v => v.Language == "cs-CZ" && v.DisplayName.Contains("Jakub"));

            if (czVoice != null)
                synthesizer.Voice = czVoice;

            // Vytvoř stream řeči
            SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(obsahSouboru);

            // Přehrát výstup
            var player = new MediaPlayer();
            player.Source = MediaSource.CreateFromStream(stream, stream.ContentType);
            player.Play();
        }
    }
}