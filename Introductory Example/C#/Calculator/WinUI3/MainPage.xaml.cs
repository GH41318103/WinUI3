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