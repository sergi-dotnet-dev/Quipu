using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Win32;
using Quipu.Code.Models;
using Quipu.Code.Services;
//Необходимо разработать клиентское приложение, которое:




//Обязательные требования:



//∙             Приложение должно поддерживать запуск и отмену операции подсчёта количества тэгов

//∙             Приложение должно оставаться отзывчивым во время работы. Оно должно каким-либо способом показывать пользователю о том, что процесс выполняется

//∙             Приложение должно каким-либо образом визуально выделить тот Url, по которому было насчитано наибольшее количество тэгов
namespace Quipu;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private CancellationToken _token = new CancellationToken();
    private ObservableCollection<UrlTagModel> models = new();
    public MainWindow()
    {
        InitializeComponent();
    }
    private void CancelCountingButton_Click()
    private async Task ChooseFileButton_Click(Object sender, EventArgs e)
    {
        var filetext = await ChooseFileAsync();
        var splittedText = await new SeparatorsSplitService().Parse(filetext);
        await LoadHtmlPage(splittedText, _token);
        Task[] tasks = new Task[splittedText.Length];
        for (Int32 i = 0; i < splittedText.Length; i++)
        {

            var content = await new HttpClient().GetStringAsync(splittedText[i]);
            tasks[i] = Task.Factory.StartNew(async () =>

            models.Add(new UrlTagModel(splittedText[i], "<a>", await TagCount("<a>", content, _token))), _token);
        }
        Task.WaitAll(tasks);
        urlsList.ItemsSource = models.OrderByDescending(i => i.TagCount);
        
    }

    private async Task<Int32> TagCount(String tag, String pageContent, CancellationToken token)
        => await Task.Run(() => Regex.Matches(pageContent, tag).Count);

    /// <summary>
    /// Calls dialog window to choose directory to scan
    /// </summary>
    private async Task<String> ChooseFileAsync()
    {
        OpenFileDialog folderBrowserDialog = new OpenFileDialog();
        if (folderBrowserDialog.ShowDialog() == true)
            return await File.ReadAllTextAsync(folderBrowserDialog.FileName);
        else return String.Empty;
    }
    private async Task LoadHtmlPage(String[] url, CancellationToken token)
    {
        Task[] tasks = new Task[url.Length];
        for (Int32 i = 0; i < url.Length; i++)
            tasks[i] = Task.Factory.StartNew(() => System.Diagnostics.Process.Start(url[i]));

        await Task.WhenAll(tasks);
    }
}
