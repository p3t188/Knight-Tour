using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Knight_Tour
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Csiko csiko;
        private List<Lepes> csikoLepesek;

        private Display display;
        private DispatcherTimer timer;
        private int step_count;
        
        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += OnStepEvent;

            display = new Display(GameArea);

            dgSteps.ItemsSource = display.Steps.DefaultView;

            slSpeed.Minimum = 5;
            slSpeed.Maximum = 500;
        }

        // A timer beállításaiak megfelelő időközönként felrajzolunk egy lépést.
        private void OnStepEvent(object sender, EventArgs e)
        {
            if (step_count < csikoLepesek.Count)
            {
                display.Step((csikoLepesek[step_count].Y << 3) + csikoLepesek[step_count].X);
                if (++step_count == csikoLepesek.Count)
                {
                    timer.Stop();
                    display.isRun = false;
                    lMessage.Content = "A huszár\nmegérkezett!";
                }
            }
        }

        // Huszárvándorlás indítása.
        private void BtnGo_Click(object sender, RoutedEventArgs e)
        {
            csiko = new Csiko((display.startPos & 0x07), (display.startPos >> 3));
            csikoLepesek = csiko.UjLepes();
            display.Reset(display.startPos);

            if (csikoLepesek.Count == 0) return;

            display.isRun = true;
            step_count = 0;
            timer.Interval = TimeSpan.FromMilliseconds(slSpeed.Value);
            timer.Start();

            lMessage.Content = "A huszár\nvándorol!";
        }
    }
}
