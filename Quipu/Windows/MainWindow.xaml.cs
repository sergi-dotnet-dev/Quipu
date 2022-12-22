using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.Win32;
using Quipu.Code.Models;
using Quipu.Code.Services;

namespace Quipu;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private CancellationTokenSource _token = new CancellationTokenSource();
    private List<UrlTagModel> models = new();
    public MainWindow()
    {
        InitializeComponent();
        CancelCountingButton.IsEnabled = false;
        countingProgressBar.Visibility = Visibility.Hidden;
    }
    private void CancelCountingButton_Click(Object senser, RoutedEventArgs e)
    {
        _token.Cancel();
    }

    private async void ChooseFileButton_Click(Object sender, RoutedEventArgs e)
    {
        CancelCountingButton.IsEnabled = true;
        models.Clear();
        urlsList.ItemsSource = models;
        var filetext = await ChooseFileAsync();
        var splittedText = await new SeparatorsSplitService().Parse(filetext);
        List<Task> tasks = new();

        for (Int32 i = 0; i < splittedText.Length; i++)
        {
            if (_token.Token.IsCancellationRequested)
                break;

            //LoadHtmlPage(splittedText[i]);
            var content = await new HttpClient().GetStringAsync(splittedText[i]);
            countingProgressBar.Visibility = Visibility.Visible;

            var url = splittedText[i];
            var curentIndex = i;
            var con = content;

            tasks.Add(Task.Run(async () => models.Add(new(url, "<a>", await TagCount("<a", con, _token.Token)))));
        }
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Внимание", "Подсчёт остановлен.", button: MessageBoxButton.OK);
        }

        urlsList.ItemsSource = models.OrderByDescending(i => i.TagCount);

        CancelCountingButton.IsEnabled = false;
        countingProgressBar.Visibility = Visibility.Hidden;
    }

    private async Task<Int32> TagCount(String tag, String pageContent, CancellationToken token)
        => await Task.Run(() => Regex.Matches(pageContent, tag).Count, token);

    /// <summary>
    /// Calls dialog window to choose file to read
    /// </summary>
    private async Task<String> ChooseFileAsync()
    {
        OpenFileDialog folderBrowserDialog = new OpenFileDialog();
        if (folderBrowserDialog.ShowDialog() == true)
            return await File.ReadAllTextAsync(folderBrowserDialog.FileName);
        else return String.Empty;
    }
    private async Task LoadHtmlPage(String url)
        => await Task.Run(() => Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }));
}