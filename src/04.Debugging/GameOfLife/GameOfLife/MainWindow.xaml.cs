using System;
using System.Windows;
using System.Windows.Threading;
using System.Linq;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private Grid mainGrid;
        DispatcherTimer timer;   //  Generation timer
        private int genCounter;
        private AdWindow[] adWindows;

        public MainWindow()
        {
            InitializeComponent();
            mainGrid = new Grid(MainCanvas);

            timer = new DispatcherTimer();
            timer.Tick += OnTimer;
            timer.Interval = TimeSpan.FromMilliseconds(200);
        }

        private void StartAd()
        {
            adWindows = new AdWindow[2];
            for (int i = 0; i < 2; i++)
            {
                if (adWindows[i] == null)
                {
                    adWindows[i] = new AdWindow(this);
                    adWindows[i].Closed += AdWindowOnClosed;
                    adWindows[i].Top = this.Top + (330 * i) + 70;
                    adWindows[i].Left = this.Left + 240;                        
                    adWindows[i].Show();
                }
            }
        }

        private void AdWindowOnClosed(object sender, EventArgs eventArgs)
        {
            if (sender is AdWindow ad)
			{
                ClearAd(ad);
			}
        }

        private void OnButtonStartClick(object sender, EventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
                ButtonStart.Content = "Stop";

                StartAd();
            }
            else
            {
                ClearAllAds();
                timer.Stop();
                ButtonStart.Content = "Start";
            }
        }

        private async void OnTimer(object sender, EventArgs e)
        {
			await System.Threading.Tasks.Task.
				Run(() => mainGrid.Update());

            mainGrid.Render();

			genCounter++;
            lblGenCount.Content = "Generations: " + genCounter;
        }

        private void OnButtonClearClick(object sender, RoutedEventArgs e)
        {
            mainGrid.Clear();
        }

        private void ClearAllAds()
		{
            foreach(var ad in adWindows.Where(x => x != null))
			{
                ClearAd(ad);
			}
		}

        private void ClearAd(AdWindow ad)
		{
            var adId = Array.IndexOf(adWindows, ad);

            if (adId >= 0)
            {
                adWindows[adId].Closed -= AdWindowOnClosed;
                adWindows[adId].Close();
                adWindows[adId] = null;
            }
        }
    }
}
