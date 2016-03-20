using GeneticAlgorithm.Concretes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GA = GeneticAlgorithm.GeneticAlgorithm;

namespace TravellingSalesmanUIwpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _populationSize;
        private short _citiesAmount;
        private double _crossoverProb = 0.6;
        private double _mautationProb = 0.15;

        private bool _closed;
        private bool _paused;
        private readonly object _lock = new object();

        private GA _ga;
        private Chromosome _bestSolutionSoFar;
        private readonly object _algorithmLock = new object();
        public string _selectOperator;     

        public MainWindow()
        {
            InitializeComponent();        
        }

        private void runBtn_Click(object sender, RoutedEventArgs e)
        {
            _populationSize = (int)populField.Value;
            _citiesAmount = (short)citySlider.Value;
            _crossoverProb = crossSlider.Value;
            _mautationProb = mutSlider.Value;

            _ga = new GA(_populationSize, _citiesAmount, canvas.Width, canvas.Height, _selectOperator, _crossoverProb, _mautationProb);
            _bestSolutionSoFar = _ga.GetBestSolutionSoFar();

            Thread thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            while (!_closed)
            {
                if (_paused)
                {
                    lock (_lock)
                    {
                        if (_closed)
                            return;

                        while (_paused)
                        {
                            Monitor.Wait(_lock);

                            if (_closed)
                                return;
                        }
                    }
                }

                lock (_algorithmLock)
                {
                    _ga.GenerateNewGeneration();

                    var currentSolution = _ga.GetBestSolutionSoFar();
                    if (!currentSolution.SequenceEqual(_bestSolutionSoFar))
                    {
                        lock (_lock)
                        {
                            _bestSolutionSoFar = currentSolution;

                            Dispatcher.BeginInvoke(new Action(DrawRoute), DispatcherPriority.ApplicationIdle);
                        }

                        Thread.Sleep(100);
                    }
                }
            }
        }

        private void DrawRoute()
        {
            Chromosome route;
            int citiesNum;

            lock (_lock)
            {
                route = _bestSolutionSoFar;
                citiesNum = route.Route.Count;
            }

            labelDistance.Content = route.RouteLength;

            var canvasChildren = canvas.Children;
            canvasChildren.Clear();

            for(int i = 0; i < citiesNum; ++i)
            {
                Line line = new Line();

                int red = 255 * i / citiesNum;
                int blue = 255 - red;

                Color color = Color.FromRgb((byte)red, 0, (byte)blue);
                line.Stroke = new SolidColorBrush(color);

                line.X1 = route.Route[i].X;
                line.Y1 = route.Route[i].Y;

                if(i == citiesNum - 1)
                {
                    line.X2 = route.Route[0].X;
                    line.Y2 = route.Route[0].Y;
                }
                else
                {
                    line.X2 = route.Route[i + 1].X;
                    line.Y2 = route.Route[i + 1].Y;
                }                

                canvasChildren.Add(line);

                var circle = new Ellipse();
                circle.Stroke = Brushes.Black;
                circle.Fill = i == 0 ? Brushes.Red : Brushes.Yellow;
                circle.Width = 11;
                circle.Height = 11;
                Canvas.SetLeft(circle, route.Route[i].X - 5);
                Canvas.SetTop(circle, route.Route[i].Y - 5);
                Panel.SetZIndex(circle, 57);

                canvasChildren.Add(circle);
            }               
        }

        #region ComboBoxEventHandlers
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> data = new List<string>();
            data.Add("Tournament selection");
            data.Add("Proportional selection");

            var comboBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = data;

            // ... Make the first item selected.
            comboBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
 
            this._selectOperator = comboBox.SelectedItem as string;
        }
        #endregion

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _closed = true;

            lock (_lock)
                Monitor.Pulse(_lock);
        }
    }
}
