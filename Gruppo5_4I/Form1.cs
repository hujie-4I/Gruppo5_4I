using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gruppo5_4I
{
    public partial class MainCalc : Form
    {
        private bool isNewNumber;
        private double firstNumber;
        private string currentOperation;
        private double secondNumber;
        private double number;
        private Size originalFormSize;
        private Dictionary<Control, Size> originalSizes = new Dictionary<Control, Size>();
        private Dictionary<Control, Point> originalLocations = new Dictionary<Control, Point>();
        private Dictionary<Control, float> originalFontSizes = new Dictionary<Control, float>();




        public MainCalc()
        {
            
            InitializeComponent();
            this.Load += MainCalc_Load;
            this.SizeChanged += MainCalc_SizeChanged;

            CalcTextBox.ReadOnly = true;
            CalcTextBox.Enabled = true;
            CalcTextBox.Cursor = Cursors.Default;
            

            

            // 绑定事件
            CalcTextBox.GotFocus += CalcTextBox_GotFocus;
            CalcTextBox.MouseDown += CalcTextBox_MouseDown;
            // Inizializza il display all'avvio
            CalcTextBox.Text = "0";
            // Questo è un esempio di come potresti associare gli eventi
            // in modo programmatico se non lo fai dal Designer di Visual Studio.
            // Se li colleghi dal Designer, puoi omettere le seguenti righe.

            // Esempio Associazione (SOLO SE NON USI IL DESIGNER):
            // sette.Click += Number_Click;
            // otto.Click += Number_Click;
            // AddizioneBtn.Click += Operator_Click;
            // UgualeBtn.Click += UgualeBtn_Click;
        }

        private void CalcTextBox_GotFocus(object sender, EventArgs e)
        {
            this.ActiveControl = null; // 点击时取消焦点
        }




        // Metodo da associare al pulsante Decimale





        private void tre_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string digit = button.Text;

            if (isNewNumber)
            {
                // Se stiamo iniziando un nuovo numero, sovrascriviamo il display.
                CalcTextBox.Text = digit;
                isNewNumber = false;
            }
            else
            {
                // Gestione dello '0' iniziale
                if (CalcTextBox.Text == "0" && digit != ".")
                {
                    CalcTextBox.Text = digit;
                }
                else
                {
                    CalcTextBox.Text += digit;
                }
            }
        }

        private void Decimale_Click_1(object sender, EventArgs e)
        {
            if (!CalcTextBox.Text.Contains("."))
            {
                CalcTextBox.Text += ".";
            }
        }

        private void MenoBtn_Click(object sender, EventArgs e)
        {
            if (double.TryParse(CalcTextBox.Text, out number))
            {
                // Se non è la prima operazione, esegui il calcolo precedente prima di impostare la nuova operazione
                if (!string.IsNullOrEmpty(currentOperation) && !isNewNumber)
                {
                    UgualeBtn_Click(sender, e); // Esegui l'operazione in sospeso
                }

                // Memorizza il primo operando (o il risultato dell'operazione precedente)
                firstNumber = double.Parse(CalcTextBox.Text);

                // Imposta la nuova operazione
                Button button = (Button)sender;
                currentOperation = button.Text; // Assumi che il testo del bottone sia il simbolo corretto (+, -, x, /...)

                isNewNumber = true; // Prepara il display per il prossimo numero
            }
        }

        private void UgualeBtn_Click(object sender, EventArgs e)
        {
            // Controlla che ci sia un'operazione da eseguire
            if (string.IsNullOrEmpty(currentOperation) || isNewNumber)
            {
                return;
            }

            // Converte e prende il secondo numero
            if (double.TryParse(CalcTextBox.Text, out secondNumber))
            {
                double result = 0;

                // Esegue l'operazione
                switch (currentOperation)
                {
                    case "+":
                        result = firstNumber + secondNumber;
                        break;
                    case "-":
                        result = firstNumber - secondNumber;
                        break;
                    case "x":
                    case "*": // Considera entrambi i simboli per la moltiplicazione
                        result = firstNumber * secondNumber;
                        break;
                    case "÷":
                        if (secondNumber != 0)
                        {
                            result = firstNumber / secondNumber;
                        }
                        else
                        {
                            CalcTextBox.Text = "Div per 0!";
                            firstNumber = 0;
                            currentOperation = "";
                            isNewNumber = true;
                            return;
                        }
                        break;
                    case "^": // Esempio per la Potenza
                        result = Math.Pow(firstNumber, secondNumber);
                        break;
                    default:
                        // Operazione non gestita
                        return;

                }
                CalcTextBox.Text = result.ToString();

                // Resetta lo stato: il risultato diventa il nuovo firstNumber
                firstNumber = result;
                currentOperation = "";
                isNewNumber = true;
            }
        }

        private void CBtn_Click(object sender, EventArgs e)
        {
            CalcTextBox.Text = "0";
            firstNumber = 0;
            currentOperation = "";
            isNewNumber = true;
        }

        private void CEBtn_Click(object sender, EventArgs e)
        {
            // Cancella solo il numero corrente e prepara per la digitazione di uno nuovo
            CalcTextBox.Text = "0";
            isNewNumber = true;
        }

        private void DelBtn_Click(object sender, EventArgs e)
        {
            if (CalcTextBox.Text.Length > 1)
            {
                CalcTextBox.Text = CalcTextBox.Text.Substring(0, CalcTextBox.Text.Length - 1);
            }
            else
            {
                // Se rimane un solo carattere o è vuoto, visualizza "0"
                CalcTextBox.Text = "0";
                isNewNumber = true;
            }
        }

        private void negativo_Click(object sender, EventArgs e)
        {
            if (double.TryParse(CalcTextBox.Text, out  number))
            {
                CalcTextBox.Text = (-number).ToString();
            }
        }

        private void RadiceBtn_Click(object sender, EventArgs e)
        {
            if (double.TryParse(CalcTextBox.Text, out  number) && number >= 0)
            {
                CalcTextBox.Text = Math.Sqrt(number).ToString();
                isNewNumber = true;
            }
            else if (number < 0)
            {
                CalcTextBox.Text = "Errore";
                isNewNumber = true;
            }
        }

        private void PercBtn_Click(object sender, EventArgs e)
        {
            if (double.TryParse(CalcTextBox.Text, out number))
            {
                // Se non c'è operazione, calcola solo il numero diviso 100 (es. 50 -> 0.5)
                if (string.IsNullOrEmpty(currentOperation) || firstNumber == 0)
                {
                    CalcTextBox.Text = (number / 100).ToString();
                }
                // Se c'è un operando precedente (es. 100 + 5%)
                else
                {
                    // Calcola il valore percentuale del primo numero (es. 5% di 100 è 5)
                    double percentageValue = firstNumber * (number / 100);

                    double result = 0;
                    switch (currentOperation)
                    {
                        case "+":
                            result = firstNumber + percentageValue;
                            break;
                        case "-":
                            result = firstNumber - percentageValue;
                            break;
                        case "x": // 100 x 5% = 5
                            result = firstNumber * (number / 100);
                            break;
                        case "÷": // 100 / 5% = 2000
                            result = firstNumber / (number / 100);
                            break;
                        default:
                            return;
                    }

                    CalcTextBox.Text = result.ToString();
                    firstNumber = result;
                    currentOperation = "";
                    isNewNumber = true;
                }
            }
        }

        private void DivBtn_Click(object sender, EventArgs e)
        {
            if (double.TryParse(CalcTextBox.Text, out number))
            {
                // Se non è la prima operazione, esegui il calcolo precedente prima di impostare la nuova operazione
                if (!string.IsNullOrEmpty(currentOperation) && !isNewNumber)
                {
                    UgualeBtn_Click(sender, e); // Esegui l'operazione in sospeso
                }

                // Memorizza il primo operando (o il risultato dell'operazione precedente)
                firstNumber = double.Parse(CalcTextBox.Text);

                // Imposta la nuova operazione
                Button button = (Button)sender;
                currentOperation = button.Text; // Assumi che il testo del bottone sia il simbolo corretto (+, -, x, /...)

                isNewNumber = true; // Prepara il display per il prossimo numero
            }
        }

        private void PotenzaBtn_Click(object sender, EventArgs e)
        {
            if (double.TryParse(CalcTextBox.Text, out number))
            {
                // Se non è la prima operazione, esegui il calcolo precedente prima di impostare la nuova operazione
                if (!string.IsNullOrEmpty(currentOperation) && !isNewNumber)
                {
                    UgualeBtn_Click(sender, e); // Esegui l'operazione in sospeso
                }

                // Memorizza il primo operando (o il risultato dell'operazione precedente)
                firstNumber = double.Parse(CalcTextBox.Text);

                // Imposta la nuova operazione
                Button button = (Button)sender;
                currentOperation = button.Text; // Assumi che il testo del bottone sia il simbolo corretto (+, -, x, /...)

                isNewNumber = true; // Prepara il display per il prossimo numero
            }
        }

        private void CalcTextBox_TextChanged(object sender, EventArgs e)
        {
            CalcTextBox.Cursor = Cursors.Default;
        }

        private void MainCalc_Load(object sender, EventArgs e)
        {
            CalcTextBox.Cursor = Cursors.Default;
            CalcTextBox.ReadOnly = true;   // 不能编辑，但可以选取
            CalcTextBox.TabStop = false;   // 按 Tab 不会跳进来
            originalFormSize = this.Size;
            this.MinimumSize = new Size(300, 400);  // 窗体最小宽高
            // 记录所有控件初始大小、位置和字体
            RecordControls(this);
            CalcTextBox.SelectionLength = 0;
            CalcTextBox.SelectionStart = 0;
            

        }

        private void RecordControls(Control parent)
        {
            foreach (Control ctl in parent.Controls)
            {
                originalSizes[ctl] = ctl.Size;
                originalLocations[ctl] = ctl.Location;
                originalFontSizes[ctl] = ctl.Font.Size;
                if (ctl.Controls.Count > 0)
                    RecordControls(ctl); // 递归
            }
        }


        private void CalcTextBox_MouseDown(object sender, MouseEventArgs e)
        {

            this.ActiveControl = null; // 点击时取消选中和光标
        }

        private void MainCalc_SizeChanged(object sender, EventArgs e)
        {
            float xRatio = (float)this.Width / originalFormSize.Width;
            float yRatio = (float)this.Height / originalFormSize.Height;

            ScaleControls(this, xRatio, yRatio);
        }
        private void ScaleControls(Control parent, float xRatio, float yRatio)
        {
            foreach (Control ctl in parent.Controls)
            {
                if (originalSizes.ContainsKey(ctl))
                {
                    ctl.Width = Math.Max(1, (int)(originalSizes[ctl].Width * xRatio));
                    ctl.Height = Math.Max(1, (int)(originalSizes[ctl].Height * yRatio));
                    ctl.Left = (int)(originalLocations[ctl].X * xRatio);
                    ctl.Top = (int)(originalLocations[ctl].Y * yRatio);

                    ctl.Font = new Font(ctl.Font.FontFamily, Math.Max(1, originalFontSizes[ctl] * yRatio), ctl.Font.Style);
                }

                if (ctl.Controls.Count > 0)
                    ScaleControls(ctl, xRatio, yRatio);
            }
        }
    }
}
