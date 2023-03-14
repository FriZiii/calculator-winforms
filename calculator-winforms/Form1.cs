using System.Data;
using System.Drawing;
using System.Security.Policy;
using System.Timers;

namespace calculator_winforms
{
    public partial class Form1 : Form
    {
        IFormatProvider cultureUS = new System.Globalization.CultureInfo("en-US");
        const string divisionByZeroErrorMessage = "Impossible";
        public string currentValue;

        public Form1()
        {
            InitializeComponent();
            currentValue = tbScreen.Text;
        }

        private void ScaleFontSize(object sender, EventArgs e)
        {
            if (ActiveForm.Size.Width >= 476 && ActiveForm.Size.Height >= 620)
            {
                foreach (Control control in ActiveForm.Controls)
                {
                    ChangeFontSize(control, 25.75F, 35.25F);
                }
            }
            else
            {
                foreach (Control control in ActiveForm.Controls)
                {
                    ChangeFontSize(control, 15.75F, 25.25F);
                }
            }
        }

        public void ChangeFontSize(Control control, float btnSize, float tbSize)
        {
            switch (control)
            {
                case Button button:
                    button.Font = new Font("Nirmala UI", btnSize, FontStyle.Bold, GraphicsUnit.Point);
                    break;
                case TextBox textBox:
                    textBox.Font = new Font("Nirmala UI", tbSize, FontStyle.Bold, GraphicsUnit.Point);
                    break;
            }
            foreach (Control child in control.Controls)
            {
                ChangeFontSize(child, btnSize, tbSize);
            }
        }

        private void LockBtnWhenError()
        {
            if (tbScreen.Text.Equals(divisionByZeroErrorMessage))
            {
                btnAdd.Enabled = false;
                btnDivide.Enabled = false;
                btnMultiply.Enabled = false;
                btnSubtract.Enabled = false;

                btnEquals.Enabled = false;
                btnChangeSign.Enabled = false;

                btnClearSign.Enabled = false;
            }
            else
            {
                btnAdd.Enabled = true;
                btnDivide.Enabled = true;
                btnMultiply.Enabled = true;
                btnSubtract.Enabled = true;

                btnEquals.Enabled = true;
                btnChangeSign.Enabled = true;

                btnClearSign.Enabled = true;
            }
        }

        private void ScrollScreen()
        {
            tbScreen.SelectionStart = tbScreen.TextLength;
            tbScreen.ScrollToCaret(); 
        }

        private void ScreenChangedText(object sender, EventArgs e)
        {
            LockBtnWhenError();
            ScrollScreen();
        }

        private void OnNumbersBtnClick(object sender, EventArgs e)
        {
            if (tbScreen.Text.Equals(divisionByZeroErrorMessage))
            {
                tbScreen.Text = string.Empty;
            }

            if (Equals(tbScreen.Text, "0"))
            {
                currentValue = string.Empty;
                tbScreen.Text = currentValue;
            }

            var value = ((Button)sender).Text;

            currentValue += value;
            tbScreen.Text += value;
        }

        private void OnClearBtnClick(object sender, EventArgs e)
        {
            if (!Equals(((Button)sender).Text, "C"))
            {
                int lenght = tbScreen.Text.Length;
                if (lenght > 1)
                {
                    currentValue = currentValue.Remove(currentValue.Length - 1);
                    tbScreen.Text = tbScreen.Text.Remove(lenght - 1);
                }
                else if (lenght == 1)
                {
                    tbScreen.Text = "0";
                }
            }
            else
            {
                currentValue = "0";
                tbScreen.Text = currentValue;
            }
        }

        private void OnChangeSignBtnClick(object sender, EventArgs e)
        {

            if (!tbScreen.Text.Equals(divisionByZeroErrorMessage))
            {
                if (!tbScreen.Text.Equals("0") && !IsPreviusAnOperation())
                {
                    var value = double.Parse(currentValue, cultureUS);
                    value *= -1;

                    tbScreen.Text = tbScreen.Text.Remove(tbScreen.Text.Length - (currentValue.Length));

                    currentValue = value.ToString(cultureUS);
                    tbScreen.Text += currentValue.ToString(cultureUS);
                }
            }
        }

        private void OnCommaBtnClick(object sender, EventArgs e)
        {
            if (!currentValue.Contains('.'))
            {
                if (tbScreen.Text.Equals(divisionByZeroErrorMessage) || IsPreviusAnOperation())
                {
                    currentValue = "0.";
                    tbScreen.Text = currentValue;
                    return;
                }
                currentValue += ".";
                tbScreen.Text += ".";
            }
        }

        private void OnOperationsBtnClick(object sender, EventArgs e)
        {
            if (!tbScreen.Text.Equals(divisionByZeroErrorMessage))
            {
                var operation = ((Button)sender).Text;

                if (IsPreviusAnOperation())
                {
                    tbScreen.Text = tbScreen.Text.Remove(tbScreen.Text.Length - 1) + operation;
                    return;
                }
                currentValue = string.Empty;
                tbScreen.Text += operation;
            }
        }

        private bool IsPreviusAnOperation()
        {
            int lastChar = tbScreen.Text[^1];
            if (lastChar.Equals('+') || lastChar.Equals('-') || lastChar.Equals('÷') || lastChar.Equals('×'))
                return true;
            return false;
        }

        private void OnEqualsBtnClick(object sender, EventArgs e)
        {
            if (!IsPreviusAnOperation())
            {
                var expression = tbScreen.Text.Replace("×", "*").Replace("÷", "/");

                try
                {
                    DataTable dt = new();

                    var result = dt.Compute(expression, "").ToString();
                    double resultAsDouble = Convert.ToDouble(result);
                    if (double.IsInfinity(resultAsDouble) || result.Equals("NaN"))
                    {
                        tbScreen.Text = divisionByZeroErrorMessage;
                        return;
                    }
                    tbScreen.Text = resultAsDouble.ToString("G6").Replace(',', '.');
                    currentValue = tbScreen.Text;
                }
                catch (DivideByZeroException)
                {
                    tbScreen.Text = divisionByZeroErrorMessage;
                }
                catch(OverflowException)
                {
                    tbScreen.Text = divisionByZeroErrorMessage;
                }
            }
        }
    }
}