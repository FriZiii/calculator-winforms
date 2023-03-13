using System.Data;

namespace calculator_winforms
{
    public partial class Form1 : Form
    {
        IFormatProvider cultureUS = new System.Globalization.CultureInfo("en-US");
        const string divisionByZeroErrorMessage = "Impossible";
        private bool commaIsAvailable = true;

        public Form1()
        {
            InitializeComponent();
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

        private void OnNumbersBtnClick(object sender, EventArgs e)
        {
            if (tbScreen.Text.Equals(divisionByZeroErrorMessage))
            {
                tbScreen.Text = string.Empty;
            }

            if (Equals(tbScreen.Text, "0"))
            {
                tbScreen.Text = string.Empty;
            }

            var value = ((Button)sender).Text;

            tbScreen.Text += value;
        }

        private void OnClearBtnClick(object sender, EventArgs e)
        {
            if (!Equals(((Button)sender).Text, "C"))
            {
                int lenght = tbScreen.Text.Length;
                if (lenght > 1)
                {
                    if (tbScreen.Text[lenght - 1] == '.')
                    {
                        commaIsAvailable = true;
                    }
                    tbScreen.Text = tbScreen.Text.Remove(lenght - 1);
                }
                else if (lenght == 1)
                {
                    tbScreen.Text = "0";
                }
            }
            else
            {
                tbScreen.Text = "0";
                commaIsAvailable = true;
            }
        }

        private void OnChangeSignBtnClick(object sender, EventArgs e)
        {

            if (!tbScreen.Text.Equals(divisionByZeroErrorMessage))
            {
                if (!tbScreen.Text.Equals("0"))
                {
                    var value = double.Parse(tbScreen.Text, cultureUS);
                    value *= -1;
                    tbScreen.Text = value.ToString(cultureUS);
                }
            }
        }

        private void OnCommaBtnClick(object sender, EventArgs e)
        {
            if (commaIsAvailable)
            {
                if (tbScreen.Text.Equals(divisionByZeroErrorMessage))
                {
                    tbScreen.Text = "0";
                }
                if (isPreviusAnOperation())
                {
                    tbScreen.Text += "0.";
                    return;
                }
                tbScreen.Text += ".";
                commaIsAvailable = false;
            }
        }

        private void OnOperationsBtnClick(object sender, EventArgs e)
        {
            if (!tbScreen.Text.Equals(divisionByZeroErrorMessage))
            {
                var operation = ((Button)sender).Text;
                commaIsAvailable = true;

                if (isPreviusAnOperation())
                {
                    tbScreen.Text = tbScreen.Text.Remove(tbScreen.Text.Length - 1) + operation;
                    return;
                }
                tbScreen.Text += operation;
            }
        }

        private bool isPreviusAnOperation()
        {
            int lastChar = tbScreen.Text[^1];
            if (lastChar.Equals('+') || lastChar.Equals('-') || lastChar.Equals('÷') || lastChar.Equals('×'))
                return true;
            return false;
        }

        private void OnEqualsBtnClick(object sender, EventArgs e)
        {
            if (!isPreviusAnOperation())
            {
                var expression = tbScreen.Text.Replace("×", "*").Replace("÷", "/");

                try
                {
                    DataTable dt = new();

                    var result = dt.Compute(expression, "").ToString();
                    double resultDouble = Convert.ToDouble(result);
                    if (double.IsInfinity(resultDouble) || result.Equals("NaN"))
                    {
                        tbScreen.Text = divisionByZeroErrorMessage;
                    }
                    else
                    {
                        tbScreen.Text = resultDouble.ToString().Replace(',', '.');
                        if (tbScreen.Text.Contains('.'))
                        {
                            commaIsAvailable = false;
                        }
                        else
                        {
                            commaIsAvailable = true;
                        }
                    }
                }
                catch (DivideByZeroException)
                {
                    tbScreen.Text = divisionByZeroErrorMessage;
                }
            }
        }
    }
}