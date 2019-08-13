using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;

namespace SuspendWarning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _dt;
        private DateTime _timerStart;
        private TimeSpan _timerDuration = TimeSpan.FromSeconds(30);
        private string _suspendTextTemplate = "Suspending Computer in {0:mm\\:ss} seconds...";
        private bool _flipColor = true;

        public MainWindow()
        {
            InitializeComponent();

            StartCountdown();
        }

        private void StartCountdown()
        {
            _timerStart = DateTime.Now;
            _dt = new DispatcherTimer();
            _dt.Interval = TimeSpan.FromSeconds(0.2);
            _dt.Tick += new EventHandler(_dt_Tick);
            _dt.Start();
        }

        void _dt_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - _timerStart;
            TimeSpan remaining = _timerDuration - elapsed;

            string remainderString = string.Format(_suspendTextTemplate, remaining);
            if (remainderString != _suspendText.Text)
            {
                _suspendText.Text = remainderString;
            }

            if (remaining < TimeSpan.FromSeconds(10))
            {
                _rWarningColor.Fill = (_flipColor = !_flipColor) ? Brushes.Firebrick : Brushes.Transparent;
            }
            else
            {
                _rWarningColor.Fill = Brushes.Transparent;
            }

            if (elapsed > _timerDuration)
            {
                _dt.Stop();
                SuspendComputer();
            }
        }

        private void SuspendComputer()
        {
            SetSuspendState(false, false, false);
            Application.Current.Shutdown();
        }

        [DllImport("powrprof.dll")]
        static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _bSnooze.Visibility = System.Windows.Visibility.Hidden;
            _dt.Stop();
            Storyboard fadeOut = (Storyboard)this.Resources["fadeOut"];
            fadeOut.Children[0].SetValue(Storyboard.TargetProperty, this);
            fadeOut.Begin();
            Dispatcher.In(TimeSpan.FromSeconds(2), () => this.Hide());
            Dispatcher.In(TimeSpan.FromMinutes(10), () => { new MainWindow().Show(); this.Close(); });
        }
    }
}
