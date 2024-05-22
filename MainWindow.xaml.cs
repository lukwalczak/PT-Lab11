using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.Win32;


namespace PT_Lab11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;

        private delegate long FactorialDelegate(int number);

        private delegate long NumeratorDelegate(int N, int K);

        public MainWindow()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private async void CalculateNewtonButton_Click(object sender, RoutedEventArgs e)
        {
            int N = int.Parse(NTextBox.Text);
            int K = int.Parse(KTextBox.Text);
            // Task<T>
            CalculateNewtonSymbol(N, K);

            // Delegate
            NumeratorDelegate numeratorDelegate = (n, k) =>
            {
                long result = 1;
                for (int i = 0; i < k; i++)
                {
                    result *= (n - i);
                }

                return result;
            };

            FactorialDelegate factorialDelegate = (number) =>
            {
                long result = 1;
                for (int i = 2; i <= number; i++)
                {
                    result *= i;
                }

                return result;
            };

            var numeratorTask = Task.Run(() => numeratorDelegate(N, K));
            var denominatorTask = Task.Run(() => factorialDelegate(K));
            await Task.WhenAll(numeratorTask, denominatorTask);
            long result2 = numeratorTask.Result / denominatorTask.Result;

            // Async-await
            long result3 = await CalculateNewtonSymbolAsync(N, K);

            NewtonResultTextBox2.Text = result2.ToString();
            NewtonResultTextBox3.Text = result3.ToString();
        }

        private Task<long> CalculateNumeratorAsync(int N, int K)
        {
            return Task.Run(() =>
            {
                long result = 1;
                for (int i = 0; i < K; i++)
                {
                    result *= (N - i);
                }

                return result;
            });
        }

        private Task<long> CalculateFactorialAsync(int number)
        {
            return Task.Run(() =>
            {
                long result = 1;
                for (int i = 2; i <= number; i++)
                {
                    result *= i;
                }

                return result;
            });
        }

        private async Task<long> CalculateNewtonSymbolAsync(int N, int K)
        {
            var numeratorTask = CalculateNumeratorAsync(N, K);

            var denominatorTask = CalculateFactorialAsync(K);

            await Task.WhenAll(numeratorTask, denominatorTask);

            return numeratorTask.Result / denominatorTask.Result;
        }

        private long CalculateFactorial(int number)
        {
            long result = 1;
            for (int i = 2; i <= number; i++)
            {
                result *= i;
            }

            return result;
        }

        private long CalculateNumerator(int N, int K)
        {
            long result = 1;
            for (int i = 0; i < K; i++)
            {
                result *= (N - i);
            }

            return result;
        }

        private void CalculateNewtonSymbol(int N, int K)
        {
            Task<long> numeratorTask = Task.Run(() => CalculateNumerator(N, K));

            Task<long> denominatorTask = Task.Run(() => CalculateFactorial(K));

            Task.WhenAll(numeratorTask, denominatorTask).ContinueWith(t =>
            {
                long result = numeratorTask.Result / denominatorTask.Result;
                Dispatcher.Invoke(() => NewtonResultTextBox1.Text = result.ToString());
            });
        }

        private void CalculateFibonacciButton_Click(object sender, RoutedEventArgs e)
        {
            int n = int.Parse(FibonacciIndexTextBox.Text);
            ProgressBar.Value = 0;
            worker.RunWorkerAsync(n);
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int n = (int)e.Argument;
            long a = 0, b = 1;
            for (int i = 0; i < n; i++)
            {
                long temp = a;
                a = b;
                b = temp + b;
                Thread.Sleep(5);
                worker.ReportProgress((i + 1) * 100 / n);
            }

            e.Result = a;
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FibonacciResultTextBox.Text = e.Result.ToString();
        }

        private async void CompressButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}