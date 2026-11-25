// MainPage.xaml.cs 負責處理直接的畫面與使用者介面互動（View），
// 而 MainPageViewModel.cs 則專注於管理資料狀態與商業邏輯（ViewModel）。
// 這兩者的分工源自於 .NET 開發（如 MAUI、WPF、WinUI）中常見的 MVVM（Model-View-ViewModel）設計模式。
/*
 
 
實際運作流程差異
1. MainPage.xaml.cs 的運作（傳統事件驅動）
當使用者在畫面上點擊按鈕時，UI 元件直接觸發寫在 .xaml.cs 裡的方法，程式碼必須親自去抓畫面上其他元件的值，計算完後再手動塞回去。
• 適用情境：純粹的視覺動畫、畫面剛載入時的焦點設定（Focus）、或極度依賴特定 UI 控制項的操作。
• 範例邏輯：csharp
// MainPage.xaml.cs
private void OnButtonClicked(object sender, EventArgs e)
{
    // 直接操作 UI 元件名稱
    string input = NameEntry.Text; 
    GreetingLabel.Text = $"你好, {input}";
}
2. MainPageViewModel.cs 的運作（資料驅動）
畫面（.xaml）透過 {Binding} 綁定到 ViewModel 的屬性與命令。當使用者輸入文字或點擊按鈕時，框架會自動把資料同步到 ViewModel；當 ViewModel 裡的資料改變時，會發出「屬性已變更」的通知，畫面便會自動刷新。
• 適用情境：API 資料串接、表單驗證、列表資料處理、按鈕點擊後的商業邏輯判斷。
• 範例邏輯：csharp
// MainPageViewModel.cs (以 CommunityToolkit.Mvvm 為例)
public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName;

    [ObservableProperty]
    private string greetingMessage;

    [RelayCommand]
    private void SayHello()
    {
        // 只處理資料，完全不碰 UI 元件
        GreetingMessage = $"你好, {UserName}";
    }
}
兩者如何串接在一起？
通常會在 MainPage.xaml.cs 的建構函式（Constructor）中，將 ViewModel 指定給畫面的 BindingContext（或 DataContext），兩者就正式建立連線：
csharp
// MainPage.xaml.cs
public MainPage()
{
    InitializeComponent();
    // 將 ViewModel 綁定到這個頁面
    this.BindingContext = new MainPageViewModel(); 
}
 */
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace WinUI3
{
    public sealed partial class MainPage : Page
    {
        private double _firstNumber = 0; // 儲存第一個數字
        private double _lastAnswer = 0; // 儲存上一次的計算結果 (Ans)
        private string _operator = ""; // 儲存運算子 (+, -, ×, ÷)
        private bool _isNewEntry = true; // 標記是否為新輸入，若為 true，則下一次輸入會清空顯示螢幕
        public MainPage()
        {
            this.InitializeComponent();
        }

        // 點擊數字或小數點
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string value = button.Content.ToString();

            // 若剛按完等號且直接輸入新數字，清空上方舊算式
            if (_isNewEntry && string.IsNullOrEmpty(_operator))
            {
                ExpressionText.Text = ""; // 清空上方算式
            }

            // 若為新輸入，直接顯示數字或小數點
            if (_isNewEntry) 
            {
                DisplayText.Text = (value == ".") ? "0." : value; // 若輸入的是小數點，則顯示 "0."
                _isNewEntry = false;
            }
            else
            {
                if (value == "." && DisplayText.Text.Contains(".")) return;
                DisplayText.Text += value;
            }
        }

        // 點擊 Ans 帶入上次結果
        private void Ans_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewEntry && string.IsNullOrEmpty(_operator))
            {
                ExpressionText.Text = "";
            }
            DisplayText.Text = _lastAnswer.ToString();
            _isNewEntry = false;
        }

        // 點擊 ⌫ 退格刪除一個字元
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewEntry || DisplayText.Text == "錯誤") return;

            if (DisplayText.Text.Length > 1)
                DisplayText.Text = DisplayText.Text[..^1];
            else
                DisplayText.Text = "0";
        }

        // 點擊 +, -, ×, ÷
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (double.TryParse(DisplayText.Text, out double num))
            {
                _firstNumber = num;
                _operator = button.Content.ToString();
                ExpressionText.Text = $"{_firstNumber} {_operator}"; // 更新上方算式
                _isNewEntry = true;
            }
        }

        // 點擊 = 計算結果
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_operator)) return;

            if (double.TryParse(DisplayText.Text, out double secondNumber))
            {
                double result = 0;
                switch (_operator)
                {
                    case "+": result = _firstNumber + secondNumber; break;
                    case "-": result = _firstNumber - secondNumber; break;
                    case "×": result = _firstNumber * secondNumber; break;
                    case "÷":
                        if (secondNumber == 0)
                        {
                            ExpressionText.Text = $"{_firstNumber} ÷ 0 =";
                            DisplayText.Text = "錯誤";
                            _isNewEntry = true;
                            _operator = "";
                            return;
                        }
                        result = _firstNumber / secondNumber;
                        break;
                    default:
                        return;
                }

                ExpressionText.Text = $"{_firstNumber} {_operator} {secondNumber} ="; // 顯示完整算式
                DisplayText.Text = result.ToString();
                _lastAnswer = result; // 記憶結果供 Ans 使用
                _isNewEntry = true;
                _operator = "";
            }
        }

        // 點擊 C 清除歸零
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ExpressionText.Text = "";
            DisplayText.Text = "0";
            _firstNumber = 0;
            _operator = "";
            _isNewEntry = true;
        }
    }
}