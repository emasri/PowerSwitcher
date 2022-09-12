using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetPowerPlans();
        }
        string program = "powercfg";
        string args = "/list";
        string idPattern = "[a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{10,}";
        string namePattern = "(?<=\\().+?(?=\\))";
        List<(string name, string value)> plans = new List<(string name, string value)>();

        Brush btnBrush = new SolidColorBrush(Color.FromArgb(190, 249, 249, 249));
        void GetPowerPlans()
        {
            Process powerCfg = new Process();
            powerCfg.StartInfo.FileName = program;
            powerCfg.StartInfo.Arguments = args;
            powerCfg.StartInfo.RedirectStandardOutput = true;
            powerCfg.Start();
            string plansOutput = powerCfg.StandardOutput.ReadToEnd();
            string[] lines = plansOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)[2..];
            
            foreach (string plan in lines)
            {
                var name = Regex.Match(plan, namePattern).Value;
                var code = Regex.Match(plan, idPattern).Value;
                plans.Add(( name, code) );
                AddPlanBtn(name, code);
            }
        }
        void AddPlanBtn(string name, string code)
        {
            Button btn = new Button();
            btn.Name = name.Replace(" ", "");
            btn.Content = name;
            btn.FontSize = 18;
            btn.Width = 735;
            btn.Height = 50;
            btn.HorizontalContentAlignment = HorizontalAlignment.Left;
            btn.Padding = new Thickness(50, 5, 1, 5);
            btn.Background = btnBrush;
            btn.BorderBrush = Brushes.White;
            btn.Click += new RoutedEventHandler((sender, args) => { 
                Process p1 = new Process();
                p1.StartInfo.FileName = program;
                p1.StartInfo.Arguments = $" /setactive {code}";
                p1.Start();
                mainWindow.Close();
            });
            plansView.Items.Add(btn);
        }

        private void mainWindow_Deactivated(object sender, EventArgs e)
        {

            //mainWindow.Close();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void plansView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
