using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WpfApp1
{

    public class LamdaCollection<T> : Collection<T>
        where T : DependencyObject, new()
    {
        public LamdaCollection(int count)
        {
            while (count-- > 0)
                Add(new T());
        }

        public LamdaCollection<T> WithProperty<U>(DependencyProperty property, Func<int, U> generator) //метод присваивания различных свойств
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].SetValue(property, generator(i));
            }
            return this;
        }

        public LamdaCollection<T> WithXY<U>(Func<int, U> xGen, Func<int, U> yGen)
        {
            for (int i = 0; i < Count; i++)
            {
                this[i].SetValue(Canvas.LeftProperty, xGen(i));
                this[i].SetValue(Canvas.TopProperty, yGen(i));
            }
            return this;
        }
    }

    public class LambdaDoubleAnimation : DoubleAnimation
    {
        public Func<double, double> ValueGenerator { get; set; }
        protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock)
        {
            return ValueGenerator(base.GetCurrentValueCore(defaultOriginValue, defaultDestinationValue, animationClock));
        }
    }
   
    public class LambdsDoubleAnimationCollection : Collection<LambdaDoubleAnimation>  
    {
        public void ButtonAnimation_Completed(object sender, EventArgs e)
        {
//TODO: first animation ended = > Needed to push button2
          //  MessageBox.Show("Анимация завершена");
            MainWindow.CanClicked = true;
        }
        public LambdsDoubleAnimationCollection(
            int count,
            int DeltaWidth,
            Func<int, double> from,
            Func<int, double> to,
            Func<int, Duration> duration,
            Func<int, Func<double, double>> valueGenerator)
        {
            for (int i = 0; i < count; i++)
            {
                var lda = new LambdaDoubleAnimation // конструктор для анмаций
                {
                    From = from(i),
                    To = to(i + (int)(DeltaWidth * 23.5 / 350)),
                    Duration = duration(i),
                    ValueGenerator = valueGenerator(i),
                   //SpeedRatio = 2.0,
                    FillBehavior = FillBehavior.Stop,
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(1)
                    
                };
                Add(lda);
                if (i == count - 1)
                {
                      lda.Completed += ButtonAnimation_Completed;
                }
            }
        }

        public void BeginApplyAimation
            (UIElement[] targets, DependencyProperty property)
        {
            for (int i = 0; i < Count; i++)
            {
                targets[i].BeginAnimation(property, Items[i]);//применяем анимации
                

            }
        }
    }
    public class LambdsDoubleAnimationCollectionBackward : Collection<LambdaDoubleAnimation>
    {
        public LambdsDoubleAnimationCollectionBackward(
            int count,
            int DeltaWidth,
            Func<int, double> from,
            Func<int, double> to,
            Func<int, Duration> duration,
            Func<int, Func<double, double>> valueGenerator)
        {
            for (int i = 0; i > -count; i--)
            {
                var lda = new LambdaDoubleAnimation // конструктор для анмаций
                {
                    From = from(i),
                    To = to(i + (int)(DeltaWidth * 23.5 / 350)),
                    Duration = duration(i),
                    ValueGenerator = valueGenerator(i),
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(1)
                };
                Add(lda);
               
            }
        }
        public void BeginApplyAimation
            (UIElement[] targets, DependencyProperty property)
        {
            for (int i = 0; i < Count; i++)
            {
                targets[i].BeginAnimation(property, Items[i]);//применяем анимации
            }
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private readonly LamdaCollection<Ellipse> circles;
        private readonly LamdaCollection<Ellipse> EyeLeft;
        private readonly LamdaCollection<Ellipse> EyeRight;
        private static readonly Random rand = new Random();
        private int StartWidth = (int)(0.48 * System.Windows.SystemParameters.WorkArea.Width);
        private const int StartHeight = 250;

        public static bool CanClicked = false;
        public MainWindow()
        {
            InitializeComponent();

            const int count = 100;

            MyWindow.Width = System.Windows.SystemParameters.PrimaryScreenWidth - 30;
            MyWindow.Height = System.Windows.SystemParameters.PrimaryScreenHeight - 100;

            circles = new LamdaCollection<Ellipse>(count)  //заинициализировали новую лямда коллекцию
                .WithProperty(Shape.StrokeThicknessProperty, i => 5.0 * i / 100)
                .WithProperty(Shape.StrokeProperty, i => new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)))
                .WithProperty(WidthProperty, i => i * 1.1) // размерчик
                .WithProperty(HeightProperty, i => i * 1.1)
                .WithProperty(Shape.FillProperty,
                               i => new SolidColorBrush(Color.FromArgb((byte)(rand.Next(254, 255)), (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))))
                .WithXY(
                    i => StartWidth + (Math.Pow(i, 1.1) * Math.Sin(i / 4.0 * Math.PI)),
                    i => StartHeight + (Math.Pow(i, 1.1) * Math.Cos(i / 4.0 * Math.PI)));

            EyeLeft = new LamdaCollection<Ellipse>(count)
                .WithProperty(WidthProperty, i => i * 1.0 / 5.5)
                .WithProperty(HeightProperty, i => i * 1.3 / 5.5)
                .WithProperty(Shape.FillProperty,
                              i => new SolidColorBrush(Color.FromArgb((byte)(255), (byte)(0), (byte)(0), (byte)(0))))
                .WithXY(
                    i => StartWidth + 32 * i / 100 + (Math.Pow(i, 1.1) * Math.Sin(i / 4.0 * Math.PI)),
                    i => StartHeight + 25 * i / 100 + (Math.Pow(i, 1.1) * Math.Cos(i / 4.0 * Math.PI)));


            EyeRight = new LamdaCollection<Ellipse>(count)
                .WithProperty(WidthProperty, i => i * 1.0 / 5.5)
                .WithProperty(HeightProperty, i => i * 1.3 / 5.5)
                .WithProperty(Shape.FillProperty,
                    i => new SolidColorBrush(Color.FromArgb((byte)(255), (byte)(0), (byte)(0), (byte)(0))))
            .WithXY(
                    i => StartWidth + 60 * i / 100 + (Math.Pow(i, 1.1) * Math.Sin(i / 4.0 * Math.PI)),
                    i => StartHeight + 25 * i / 100 + (Math.Pow(i, 1.1) * Math.Cos(i / 4.0 * Math.PI)));

            int i2 = 0;
            do
            {
                MyCanvas1.Children.Add(circles[i2]);
                MyCanvas1.Children.Add(EyeLeft[i2]);
                MyCanvas1.Children.Add(EyeRight[i2]);
                i2++;
            } while (i2 < count);


        }

        public LambdsDoubleAnimationCollection PropertysLambdsAnimatoinCollection(double DeltaFrom, double DeltaTo)
        {
            var element = new LambdsDoubleAnimationCollection(
             circles.Count,
             StartWidth,
             i => StartWidth + DeltaFrom + Math.Pow(i, 1.1) * Math.Sin(i / 4.0 * Math.PI),///* i / 100, /5*i  //TRY
             i => 15.0 * i * DeltaTo,
             i => new Duration(TimeSpan.FromSeconds(4)),
             i => j => 100.0 / j);
            return element;
         
        }
        public LambdsDoubleAnimationCollection PropertysLambdsAnimatoinCollection(double DeltaFrom, bool IfI, double DeltaTo)
        {
            var element = new LambdsDoubleAnimationCollection(
             circles.Count,
             StartWidth,
             i => StartWidth + DeltaFrom * i + Math.Pow(i, 1.1) * Math.Sin(i / 4.0 * Math.PI),
             i => 15.0 * i * DeltaTo,
             i => new Duration(TimeSpan.FromSeconds(4)),
             i => j => 100.0 / j);
            return element;
        }
        public LambdsDoubleAnimationCollectionBackward PropertysLambdsAnimatoinCollection(bool IfBackward, double DeltaFrom, double DeltaTo)
        {
            var element = new LambdsDoubleAnimationCollectionBackward(
             circles.Count,
             StartWidth,
             i => StartWidth + DeltaFrom + Math.Pow((-i), 1.1) * Math.Sin(i / 4.0 * Math.PI),//*i/100,
             i => 15.0 * i * DeltaTo, //* i / 100,
             i => new Duration(TimeSpan.FromSeconds(4)),
             i => j => 100.0 / j);
            return element;
        }
        public LambdsDoubleAnimationCollectionBackward PropertysLambdsAnimatoinCollection(bool IfBackward, double DeltaFrom, bool IfI, double DeltaTo)
        {
            var element = new LambdsDoubleAnimationCollectionBackward(
             circles.Count,
             StartWidth,
             i => StartWidth + DeltaFrom * i + Math.Pow((-i), 1.1) * Math.Sin(i / 4.0 * Math.PI),//*i/100,
             i => 15.0 * i * DeltaTo, //* i / 100,
             i => new Duration(TimeSpan.FromSeconds(4)),
             i => j => 100.0 / j);
            return element;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var c = PropertysLambdsAnimatoinCollection(1, 1);
            var eye = PropertysLambdsAnimatoinCollection(32.0 / 100.0, true, 1.012);
            var eye2 = PropertysLambdsAnimatoinCollection(60.0 / 100.0, true, 1.026);

            c.BeginApplyAimation(circles.Cast<UIElement>().ToArray(),
                Canvas.LeftProperty);

            eye.BeginApplyAimation(EyeLeft.Cast<UIElement>().ToArray(),
                Canvas.LeftProperty);//LeftProperty

            eye2.BeginApplyAimation(EyeRight.Cast<UIElement>().ToArray(),
                Canvas.LeftProperty);

            if (CanClicked)
            {
                Button_Click_1(sender, e);
                CanClicked = false;
            }

        }         
        public void Button_Click_1(object sender, RoutedEventArgs e)
        {


            var c = PropertysLambdsAnimatoinCollection(true, 1, 1);
            var eye = PropertysLambdsAnimatoinCollection(true, -32.0 / 100.0, true, 1.012);
            var eye2 = PropertysLambdsAnimatoinCollection(true, -60.0 / 100.0, true, 1.04);

            c.BeginApplyAimation(circles.Cast<UIElement>().ToArray(),
                Canvas.LeftProperty);

            eye.BeginApplyAimation(EyeLeft.Cast<UIElement>().ToArray(),
                Canvas.LeftProperty);//LeftProperty

            eye2.BeginApplyAimation(EyeRight.Cast<UIElement>().ToArray(),
                Canvas.LeftProperty);


        }
        public void ButtonClickProcedure()
        {
            var c = PropertysLambdsAnimatoinCollection(true, 1, 1);
            c.BeginApplyAimation(circles.Cast<UIElement>().ToArray(),
            Canvas.LeftProperty);
        }


    }


}
