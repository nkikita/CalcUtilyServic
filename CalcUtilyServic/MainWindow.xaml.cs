using System;
using CalcUtilyServic.Data;
using CalcUtilyServic.Models;
using CalcUtilyServic.Services;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CalcUtilyServic
{
    public partial class MainWindow : Window
    {
        private readonly CalculationService _calcService;

        public MainWindow()
        {
            InitializeComponent();
            LoadHistory();
            SQLitePCL.Batteries.Init();

            Loaded += MainWindow_Loaded;
            _calcService = new CalculationService();

            txtResidentsFirstHalf.Text = "1";
            txtResidentsSecondHalf.Text = "1";

            txtHwsPrev.Text = "100";
            txtHwsCurr.Text = "110";

            txtGwsPrev.Text = "50";
            txtGwsCurr.Text = "55";

            txtEdPrev.Text = "200";
            txtEdCurr.Text = "230";

            txtEnPrev.Text = "150";
            txtEnCurr.Text = "180";
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            УчитыватьЖителейCheckBox.Checked += УчитыватьЖителейCheckBox_Checked;
            УчитыватьЖителейCheckBox.Unchecked += УчитыватьЖителейCheckBox_Unchecked;

            txtResidentsFirstHalf.IsEnabled = УчитыватьЖителейCheckBox.IsChecked == true;
            txtResidentsSecondHalf.IsEnabled = УчитыватьЖителейCheckBox.IsChecked == true;
        }
        private void УчитыватьЖителейCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            txtResidentsFirstHalf.IsEnabled = true;
            txtResidentsSecondHalf.IsEnabled = true;
        }

        private void УчитыватьЖителейCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            txtResidentsFirstHalf.IsEnabled = false;
            txtResidentsSecondHalf.IsEnabled = false;
        }
        private void LoadHistory()
        {
            using (var db = new AppDbContext())
            {
                var readings = db.Readings
                    .OrderByDescending(r => r.Date)
                    .Take(20)
                    .ToList();
                dgHistory.ItemsSource = readings;
            }
        }


        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool hasHwsMeter = chkHwsHasMeter.IsChecked ?? false;
                bool hasGwsMeter = chkGwsHasMeter.IsChecked ?? false;
                bool hasElecMeter = chkElecHasMeter.IsChecked ?? false;

                decimal hwsPrev = decimal.TryParse(txtHwsPrev.Text, out var hwsP) ? hwsP : 0;
                decimal hwsCurr = decimal.TryParse(txtHwsCurr.Text, out var hwsC) ? hwsC : 0;

                decimal gwsPrev = decimal.TryParse(txtGwsPrev.Text, out var gwsP) ? gwsP : 0;
                decimal gwsCurr = decimal.TryParse(txtGwsCurr.Text, out var gwsC) ? gwsC : 0;

                decimal edPrev = decimal.TryParse(txtEdPrev.Text, out var edP) ? edP : 0;
                decimal edCurr = decimal.TryParse(txtEdCurr.Text, out var edC) ? edC : 0;

                decimal enPrev = decimal.TryParse(txtEnPrev.Text, out var enP) ? enP : 0;
                decimal enCurr = decimal.TryParse(txtEnCurr.Text, out var enC) ? enC : 0;


                var service = new CalculationService();
                int residentsFirstHalf = 0;
                int residentsSecondHalf = 0;

                if (УчитыватьЖителейCheckBox.IsChecked == true)
                {
                    residentsFirstHalf = int.TryParse(txtResidentsFirstHalf.Text, out var res1) ? res1 : 0;
                    residentsSecondHalf = int.TryParse(txtResidentsSecondHalf.Text, out var res2) ? res2 : 0;
                }

                var total = service.CalculateTotal(
                    hasHwsMeter, hwsPrev, hwsCurr,
                    hasGwsMeter, gwsPrev, gwsCurr,
                    hasElecMeter,
                    edPrev, edCurr,
                    enPrev, enCurr,
                    residentsFirstHalf, residentsSecondHalf);

                txtResult.Text = $"Итоговая сумма: {total:F2} ₽";

                var reading = new MeterReading
                {
                    Date = DateTime.Now,
                    ResidentsCount = residentsFirstHalf + residentsSecondHalf, 
                    HWS_Prev = hwsPrev,
                    HWS_Curr = hwsCurr,
                    GWS_Prev = gwsPrev,
                    GWS_Curr = gwsCurr,
                    Elec_Day_Prev = edPrev,
                    Elec_Day_Curr = edCurr,
                    Elec_Night_Prev = enPrev,
                    Elec_Night_Curr = enCurr,
                    TotalAmount = total,
                };

                try
                {
                    using (var db = new AppDbContext())
                    {
                        db.Database.EnsureCreated();
                        db.Readings.Add(reading);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    Exception inner = ex.InnerException;
                    while (inner != null)
                    {
                        Console.WriteLine("Внутреннее исключение:");
                        Console.WriteLine(inner.Message);
                        inner = inner.InnerException;
                    }
                }

                LoadHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка ввода данных: " + ex.Message);
            }
        }


    }
}