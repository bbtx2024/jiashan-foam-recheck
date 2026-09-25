using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using QA.Business.Manager;
using QA.Business.Message;
//using QA.Pages;
//using QA.Pages.Views;

namespace QA.IntelligentEquipment.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        private readonly IEventAggregator _eventAggregator;
        private DispatcherTimer timer;
        private ParamManager _paramManager;
        public ShellView()
        {
            InitializeComponent();

            this.MaxWidth = SystemParameters.WorkArea.Width;
            this.MaxHeight = SystemParameters.WorkArea.Height;

            _eventAggregator = IoC.Get<IEventAggregator>();
            _paramManager = IoC.Get<ParamManager>();
            // 设置定时器  
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(_paramManager.OtherSettingParam.QuitTime) // 每秒检查一次  
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            this.MouseMove += Window_MouseMove;
        }

        private void Shell_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && this.WindowState != WindowState.Maximized)
            {
                this.DragMove();
            }

            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                this.WindowState = this.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
        }

        private void ShotcutKey_TestFunction1(object sender, CanExecuteRoutedEventArgs e)
        {
            //Console.WriteLine("Shortcut Key Test1");
        }

        private void ShotcutKey_TestFunction2(object sender, CanExecuteRoutedEventArgs e)
        {
            //Console.WriteLine("Shortcut Key Test2");
        }

        private void ShotcutKey_UpdateFireware(object sender, CanExecuteRoutedEventArgs e)
        {
            //Console.WriteLine("Shortcut Key Update");
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop(); // 如果定时器已经在运行，则停止它  
            }

            timer.Start(); // 然后重新启动定时器  
        }

        //定时触发切换界面事件
        private void Timer_Tick(object sender, EventArgs e)
        {
            _eventAggregator.Publish(new TimeMessage() { }, action => { Task.Run(action); });
        }

        // 确保在窗口关闭时停止定时器  
        protected override void OnClosed(EventArgs e)
        {
            timer.Stop();
            base.OnClosed(e);
        }
    }
}
