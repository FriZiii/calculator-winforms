using System.Data;
using System.Timers;

namespace calculator_winforms
{
    public partial class Form1 : Form
    {
        IFormatProvider cultureUS = new System.Globalization.CultureInfo("en-US");
        const string divisionByZeroErrorMessage = "Impossible";
        private bool commaIsAvailable = true;
        public string currentValue = "0";

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
                    if (tbScreen.Text[lenght - 1] == '.')
                    {
                        commaIsAvailable = true;
                    }
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
                commaIsAvailable = true;
            }
        }

        private void OnChangeSignBtnClick(object sender, EventArgs e)
        {

            if (!tbScreen.Text.Equals(divisionByZeroErrorMessage))
            {
                if (!tbScreen.Text.Equals("0") && !isPreviusAnOperation())
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
            if (commaIsAvailable && !currentValue.Contains('.'))
            {
                if (tbScreen.Text.Equals(divisionByZeroErrorMessage))
                {
                    currentValue = "0";
                    tbScreen.Text = currentValue;
                }
                if (isPreviusAnOperation())
                {
                    currentValue = "0.";
                    tbScreen.Text += currentValue;
                    commaIsAvailable = false;
                    return;
                }
                currentValue += ".";
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
                currentValue = string.Empty;
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
                    double resultAsDouble = Convert.ToDouble(result);
                    if (double.IsInfinity(resultAsDouble) || result.Equals("NaN"))
                    {
                        tbScreen.Text = divisionByZeroErrorMessage;
                        return;
                    }
                    tbScreen.Text = resultAsDouble.ToString().Replace(',', '.');
                    currentValue = tbScreen.Text;
                    if (tbScreen.Text.Contains('.'))
                    {
                        commaIsAvailable = false;
                    }
                    else
                    {
                        commaIsAvailable = true;
                    }
                }
                catch (DivideByZeroException)
                {
                    tbScreen.Text = divisionByZeroErrorMessage;
                }
            }
        }

        private System.Windows.Forms.Timer timer1;

        private void InitTimer(object sender, EventArgs e)
        {
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 100; // in miliseconds
            timer1.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(currentValue.ToString());
        }
    }
}