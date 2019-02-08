using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;

namespace WeatherBotConfig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ServiceController _sc = new ServiceController {ServiceName = "WayfarersWeatherService"};
        private readonly Timer _timer = new Timer();

        public MainWindow()
        {
            InitializeComponent();

            _timer.Interval = 1000;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Elapsed += _timer_Elapsed;

            _timer.Start();
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateStatus();
        }

        private void ViewLogButton_Click(object sender, RoutedEventArgs e)
        {
            const string filePath = @"C:\WayfarersWeather\Logs\";

            if (Directory.Exists(filePath))
                Process.Start("explorer.exe", filePath);
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sc.Status != ServiceControllerStatus.Running) return;

            try
            {
                _sc.Stop();
                _sc.WaitForStatus(ServiceControllerStatus.Stopped);
                _sc.Start();
                _sc.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sc.Status != ServiceControllerStatus.Stopped) return;

            try
            {
                _sc.Start();
                _sc.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_sc.Status != ServiceControllerStatus.Running) return;

            try
            {
                _sc.Stop();
                _sc.WaitForStatus(ServiceControllerStatus.Stopped);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        private async void UpdateStatus()
        {
            await Task.Run(() =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (ServiceController.GetServices().Any(s => s.ServiceName == "WayfarersWeatherService"))
                    {
                        switch (_sc.Status)
                        {
                            case ServiceControllerStatus.ContinuePending:
                                ServiceStatusText.Text = "Continuing...";
                                ServiceStatusText.Foreground = Brushes.Yellow;
                                break;
                            case ServiceControllerStatus.Paused:
                                ServiceStatusText.Text = "Paused";
                                ServiceStatusText.Foreground = Brushes.Yellow;
                                break;
                            case ServiceControllerStatus.PausePending:
                                ServiceStatusText.Text = "Pausing...";
                                ServiceStatusText.Foreground = Brushes.Yellow;
                                break;
                            case ServiceControllerStatus.Running:
                                ServiceStatusText.Text = "Online";
                                ServiceStatusText.Foreground = Brushes.Lime;
                                break;
                            case ServiceControllerStatus.StartPending:
                                ServiceStatusText.Text = "Starting...";
                                ServiceStatusText.Foreground = Brushes.Yellow;
                                break;
                            case ServiceControllerStatus.Stopped:
                                ServiceStatusText.Text = "Offline";
                                ServiceStatusText.Foreground = Brushes.Red;
                                break;
                            case ServiceControllerStatus.StopPending:
                                ServiceStatusText.Text = "Stopping...";
                                ServiceStatusText.Foreground = Brushes.Yellow;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        StartButton.IsEnabled = true;
                        RestartButton.IsEnabled = true;
                        StopButton.IsEnabled = true;
                    }
                    else
                    {
                        ServiceStatusText.Text = "Not Installed!";
                        ServiceStatusText.Foreground = Brushes.Red;

                        StartButton.IsEnabled = false;
                        RestartButton.IsEnabled = false;
                        StopButton.IsEnabled = false;
                    }
                });
            });
        }
    }
}
