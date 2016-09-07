using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GestureRecognizerProcessException
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GestureRecognizer _gestures;

        public MainPage()
        {
            this.InitializeComponent();
            ZoomCanvas c = new ZoomCanvas(_layoutRoot);
            _layoutRoot.Children.Add(c);
        }
    }

    public class ZoomCanvas : Canvas
    {
        private Canvas _rootCanvas = new Canvas();
        private static Random _rand = new Random();
        private GestureRecognizer _gestures = new GestureRecognizer();
        private FrameworkElement _reference;

        public ZoomCanvas(FrameworkElement gestureReference)
        {
            _reference = gestureReference;

            _gestures.GestureSettings =
                GestureSettings.ManipulationScale |
                GestureSettings.ManipulationTranslateX |
                GestureSettings.ManipulationTranslateY |
                GestureSettings.ManipulationTranslateInertia;

            Children.Add(_rootCanvas);
            _rootCanvas.Background = null;
            _rootCanvas.IsHitTestVisible = false;

            for (var i = 0; i < 500; i++)
            {
                var ellipse = new Ellipse();
                ellipse.Width = _rand.NextDouble() * 10.0;
                ellipse.Height = _rand.NextDouble() * 10.0;
                Canvas.SetLeft(ellipse, _rand.NextDouble() * 800);
                Canvas.SetTop(ellipse, _rand.NextDouble() * 800);
                ellipse.Fill = new SolidColorBrush(Color.FromArgb(255, (Byte)_rand.Next(0, 255), (Byte)_rand.Next(0, 255), (Byte)_rand.Next(0, 255)));
                _rootCanvas.Children.Add(ellipse);
            }

            this.PointerPressed += OnPointerPressed;
            this.PointerReleased += OnPointerReleased;
            this.PointerMoved += OnPointerMoved;

            this.Background = new SolidColorBrush(Colors.Beige);

            _gestures.ManipulationStarted += OnManipulationStarted;
            _gestures.ManipulationCompleted += OnManipulationCompleted;
            _gestures.ManipulationUpdated += OnManipulationUpdated;
        }

        private void OnManipulationUpdated(GestureRecognizer sender, ManipulationUpdatedEventArgs args)
        {
            foreach (var child in _rootCanvas.Children)
            {
                Ellipse ellipse = child as Ellipse;
                var newWidth = ellipse.ActualWidth / args.Delta.Scale;
                var newHeight = ellipse.ActualHeight / args.Delta.Scale;

                var newX = Canvas.GetLeft(ellipse) + args.Delta.Translation.X * (_gestures.IsInertial ? 0.1 : 1.0);
                var newY = Canvas.GetTop(ellipse) + args.Delta.Translation.Y * (_gestures.IsInertial ? 0.1 : 1.0);

                ellipse.Width = newWidth;
                ellipse.Height = newHeight;
                Canvas.SetLeft(ellipse, newX);
                Canvas.SetTop(ellipse, newY);
            }
        }

        private void OnManipulationCompleted(GestureRecognizer sender, ManipulationCompletedEventArgs args)
        {
            
        }

        private void OnManipulationStarted(GestureRecognizer sender, ManipulationStartedEventArgs args)
        {
            
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                this._gestures.ProcessMoveEvents(e.GetIntermediatePoints(this._reference));
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                dialog.ShowAsync();
            }

            e.Handled = true;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this._gestures.ProcessUpEvent(e.GetCurrentPoint(this._reference));

            e.Handled = true;
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this._gestures.ProcessDownEvent(e.GetCurrentPoint(this._reference));

            e.Handled = true;
        }
    }
}
