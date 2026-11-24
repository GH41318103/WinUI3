//此檔案為 MainPage 的 ViewModel，負責處理計算機的運作邏輯，並與 View (MainPage.xaml) 進行數據綁定
// 使用 CommunityToolkit.Mvvm 套件來簡化 MVVM 模式的實作
// 主要功能包括：
// - 處理數字輸入和小數點
// - 記錄上一次的計算結果 (Ans)
// - 支援基本的四則運算 (+, -, ×, ÷)
// - 提供清除、回退和使用 Ans 的功能
// 透過 ObservableObject 和 RelayCommand，實現數據綁定和命令綁定，使 UI 與邏輯分離
// ObservableObject功能是提供屬性變更通知，讓 UI 可以自動更新顯示
// RelayCommand功能是提供命令綁定，讓按鈕點擊可以觸發對應的邏輯處理
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WinUI3.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _expressionText = ""; // 上方算式顯示

        [ObservableProperty]
        private string _displayText = "0";   // 下方主數字顯示

        private double _firstNumber = 0;
        private double _lastAnswer = 0;      // 記憶上一次的結果 (Ans)
        private string _operator = "";
        private bool _isNewEntry = true;

        [RelayCommand]
        private void InputNumber(string value)
        {
            // 若剛算完一題直接按數字，清空上方舊算式
            if (_isNewEntry && string.IsNullOrEmpty(_operator))
            {
                ExpressionText = "";
            }

            if (_isNewEntry)
            {
                DisplayText = (value == ".") ? "0." : value;
                _isNewEntry = false;
            }
            else
            {
                if (value == "." && DisplayText.Contains(".")) return;
                DisplayText += value;
            }
        }

        [RelayCommand]
        private void UseAns()
        {
            if (_isNewEntry && string.IsNullOrEmpty(_operator))
            {
                ExpressionText = "";
            }
            DisplayText = _lastAnswer.ToString();
            _isNewEntry = false;
        }

        [RelayCommand]
        private void Backspace()
        {
            if (_isNewEntry || DisplayText == "錯誤") return;

            if (DisplayText.Length > 1)
                DisplayText = DisplayText[..^1];
            else
                DisplayText = "0";
        }

        [RelayCommand]
        private void SetOperator(string op) // 設定運算符號
        {
            if (double.TryParse(DisplayText, out double num)) // 解析目前顯示的數字
            {
                _firstNumber = num; // 記錄第一個數字
                _operator = op; // 記錄運算符號
                ExpressionText = $"{_firstNumber} {_operator}"; // 更新上方算式顯示
                _isNewEntry = true; // 設定為新輸入狀態，等待下一個數字輸入
            }
        }

        [RelayCommand]
        private void Calculate() // 計算結果
        {
            if (string.IsNullOrEmpty(_operator)) return; // 若沒有運算符號，直接返回

            if (double.TryParse(DisplayText, out double secondNumber)) // 解析第二個數字
            {
                double result = 0;
                switch (_operator) // 根據運算符號進行計算
                {
                    case "+": result = _firstNumber + secondNumber; break;
                    case "-": result = _firstNumber - secondNumber; break;
                    case "×": result = _firstNumber * secondNumber; break;
                    case "÷":
                        if (secondNumber == 0)
                        {
                            ExpressionText = $"{_firstNumber} ÷ 0 =";
                            DisplayText = "錯誤";
                            _isNewEntry = true;
                            _operator = "";
                            return;
                        }
                        result = _firstNumber / secondNumber;
                        break;
                }

                ExpressionText = $"{_firstNumber} {_operator} {secondNumber} =";
                DisplayText = result.ToString();
                _lastAnswer = result; // 存入 Ans
                _isNewEntry = true;
                _operator = "";
            }
        }

        [RelayCommand]
        private void Clear()
        {
            ExpressionText = "";
            DisplayText = "0";
            _firstNumber = 0;
            _operator = "";
            _isNewEntry = true;
        }
    }
}
