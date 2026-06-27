using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace MyWpfApp46;

/// <summary>
/// プログラムの情報を保持するクラス
/// </summary>
public class ProgramInfo
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public string Icon { get; set; } = "";
}

/// <summary>
/// カテゴリーの情報を保持するクラス
/// </summary>
public class CategoryInfo
{
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public List<ProgramInfo> Programs { get; set; } = new List<ProgramInfo>();
}

/// <summary>
/// MainWindow.xaml の相互作用ロジック
/// </summary>
public partial class MainWindow : Window
{
    private List<CategoryInfo>? _categories;

    public MainWindow()
    {
        InitializeComponent();
        LoadMenu();
    }

    /// <summary>
    /// menu.jsonからメニュー情報を読み込み、初期表示を行う
    /// </summary>
    private void LoadMenu()
    {
        try
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "menu.json");

            // 実行ファイルと同じディレクトリにない場合は、プロジェクトディレクトリを探す（デバッグ用）
            if (!File.Exists(jsonPath))
            {
                jsonPath = "menu.json";
            }

            if (File.Exists(jsonPath))
            {
                string json = File.ReadAllText(jsonPath);
                _categories = JsonConvert.DeserializeObject<List<CategoryInfo>>(json);

                if (_categories != null)
                {
                    CreateCategoryButtons();
                    // 最初のカテゴリーを表示
                    if (_categories.Count > 0)
                    {
                        ShowPrograms(_categories[0]);
                    }
                }
            }
            else
            {
                MessageBox.Show("menu.json が見つかりませんでした。");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("メニューの読み込み中にエラーが発生しました: " + ex.Message);
        }
    }

    /// <summary>
    /// カテゴリーボタンを動的に生成する
    /// </summary>
    private void CreateCategoryButtons()
    {
        if (_categories == null) return;

        CategoryPanel.Children.Clear();
        // タイトルを再追加
        CategoryPanel.Children.Add(new TextBlock
        {
            Text = "カテゴリー",
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(5, 0, 0, 10),
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"))
        });

        foreach (var category in _categories)
        {
            Button btn = new Button
            {
                Content = category.Name,
                Margin = new Thickness(0, 2, 0, 2),
                Padding = new Thickness(10, 5, 10, 5),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };
            btn.Click += (s, e) => ShowPrograms(category);
            CategoryPanel.Children.Add(btn);
        }
    }

    /// <summary>
    /// 指定されたカテゴリーのプログラム一覧を表示する
    /// </summary>
    /// <param name="category">表示するカテゴリー</param>
    private void ShowPrograms(CategoryInfo category)
    {
        ProgramPanel.Children.Clear();

        foreach (var program in category.Programs)
        {
            Button btn = new Button
            {
                Width = 120,
                Height = 100,
                Margin = new Thickness(5),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDDDDD")),
                BorderThickness = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            StackPanel stack = new StackPanel();

            // アイコンの代わりにテキストを表示（画像がある場合はImageを追加可能）
            stack.Children.Add(new TextBlock
            {
                Text = "📄",
                FontSize = 32,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 5)
            });

            stack.Children.Add(new TextBlock
            {
                Text = program.Name,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 0, 5, 5)
            });

            btn.Content = stack;
            btn.Click += (s, e) => LaunchProgram(program.Path);

            ProgramPanel.Children.Add(btn);
        }
    }

    /// <summary>
    /// プログラムを起動する
    /// </summary>
    /// <param name="path">起動するプログラムのパス</param>
    private void LaunchProgram(string path)
    {
        try
        {
            Process.Start(path);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"プログラムの起動に失敗しました: {path}\n{ex.Message}");
        }
    }
}
