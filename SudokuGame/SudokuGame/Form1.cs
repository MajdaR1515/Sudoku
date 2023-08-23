using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuGame
{

    public partial class Form1 : Form
    {

        private int[,] sudokuBoard = new int[9, 9];
        private int[,] checkBoard = new int[9, 9];
        private TextBox[,] textBoxes = new TextBox[9, 9];
        private Random random = new Random();


        public Form1()
        {
            InitializeComponent();
            LinkTextBoxesToArray();
            AttachEnterEventHandlers();
            InitializeTextBoxes();


        }

        private void LinkTextBoxesToArray() // metoda pro propojení textboxů ve vizuálním ovládacím prvku TableLayoutPanel s dvourozměrným polem textBoxes
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    textBoxes[row, col] = tableLayoutPanel1.GetControlFromPosition(col, row) as TextBox;
                    // Metoda tableLayoutPanel1.GetControlFromPosition(col, row) získá ovládací prvek na dané pozici v tabulce. Používáme ji zde, abychom získali příslušný textbox.
                }
            }
        }



        private bool SolveSudoku()
        {
            int row, col;

            if (!FindEmptyCell(out row, out col)) // Zjištění, zda existuje prázdná buňka
                return true; // Pokud neexistuje, vrátíme true

            List<int> randomNumbers = Enumerable.Range(1, 9).ToList(); // Vytvoření sekvence čísel
            Random random = new Random();

            for (int i = 0; i < randomNumbers.Count; i++) // Procházíme náhodně promíchaný seznam čísel
            {
                int j = random.Next(i, randomNumbers.Count);
                int temp = randomNumbers[i];    // Zamíchání probíhá tak, že pro každý prvek i ve výčtu se zvolí náhodný prvek j mezi i a délkou seznamu. Následně se čísla na pozicích i a j prohodí.
                randomNumbers[i] = randomNumbers[j];
                randomNumbers[j] = temp;

                int num = randomNumbers[i];

                if (IsValidMove(row, col, num)) // Testujeme, zda lze číslo umístit na aktuální pozici
                {
                    sudokuBoard[row, col] = num; // Uložíme číslo na desku, do kontrolního pole a do textového pole
                    checkBoard[row, col] = num;
                    textBoxes[row, col].Text = num.ToString();

                    if (SolveSudoku()) // Rekurzivně voláme SolveSudoku pro další buňky
                        return true;

                    sudokuBoard[row, col] = 0; // Pokud řešení není úspěšné, vrátíme zpět na původní stav
                    textBoxes[row, col].Text = "";
                }
            }

            return false; //Pokud žádné číslo nelze umístit do aktuální buňky
        }

        private bool FindEmptyCell(out int row, out int col) // Metoda pro nalezení prázdné buňky
        {
            for (row = 0; row < 9; row++) // Procházíme řádky
            {
                for (col = 0; col < 9; col++) // Procházíme sloupce
                {
                    if (sudokuBoard[row, col] == 0) // Zde se testuje, zda hodnota v aktuální buňce je prázdná.
                        return true;
                }
            }

            row = -1;
            col = -1;
            return false; // Žádná prázdná buňka nebyla nalezena
        }

        private bool IsValidMove(int row, int col, int num) // Metoda pro ověření čísla
        {
            for (int i = 0; i < 9; i++) // Tato smyčka projde všechny sloupce a řádky, aby ověřila, zda se v daném řádku nebo sloupci již to číslo nachází
            {
                if (sudokuBoard[row, i] == num || sudokuBoard[i, col] == num)
                    return false;
            }

            int startRow = row - row % 3; // Vypočítání počáteční pozice bloku
            int startCol = col - col % 3;
            for (int i = 0; i < 3; i++)
            {                               // Testuje, zda se číslo již nachází v rámci bloku 3x3
                for (int j = 0; j < 3; j++)
                {
                    if (sudokuBoard[startRow + i, startCol + j] == num)
                        return false;
                }
            }

            return true; // Pokud nebyl nalezen konflikt, číslo můžeme vložit
        }




        private void start_butt_Click(object sender, EventArgs e)
        {
            start_butt.Text = "Restart";
            ClearSudokuBoard();
            int countRemove = 40; // Pokud hráč nezadá úroveň, automaticky bude nastavena na střední
            
            if (radioButtonEasy.Checked)
            {
                countRemove = 30; // Počet čísel pro snadnou úroveň
            }
            else if (radioButtonMedium.Checked)
            {
                countRemove = 40; // Počet čísel pro střední úroveň
            }
            else if (radioButtonHard.Checked)
            {
                countRemove = 50; // Počet čísel pro těžkou úroveň
            }
            
            GenerateSudoku(countRemove);
            DisplaySudokuBoard();
        }

        private void Solution_Click(object sender, EventArgs e)
        {
            SolveSudoku();
        }
        



        private void ClearSudokuBoard() // Metoda pro vymazání sudoku
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    sudokuBoard[row, col] = 0;
                    textBoxes[row, col].Text = ""; // Vymazání obsahu TextBoxu
                }
            }
        }

        private void DisplaySudokuBoard()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (sudokuBoard[row, col] != 0)
                    {
                        textBoxes[row, col].Text = sudokuBoard[row, col].ToString();
                        textBoxes[row, col].ReadOnly = true; // Zakázat úpravy pro již vyplněné buňky
                        textBoxes[row, col].Enabled = false; // Zakázat psaní pro již vyplněné buňky

                    }
                    else
                    {
                        textBoxes[row, col].Text = ""; // Prázdný řetězec pro nevyplněná pole
                        textBoxes[row, col].ReadOnly = false; // Znovu povolit úpravy pro nově odstraněné buňky
                        textBoxes[row, col].Enabled = true; // Povolit psaní pro nově odstraněné buňky
                    }
                }
            }
        }
        




        private void GenerateSudoku(int countRemove) // Metoda pro odstranění prvků dle obtížnosti
        {
            SolveSudoku(); // Vyřešíme kompletní Sudoku

            List<Tuple<int, int>> cellsToRemove = new List<Tuple<int, int>>(); //  Seznam, který bude obsahovat buňky, z nichž budou odstraněna čísla

            while (cellsToRemove.Count < countRemove)
            {
                int row = random.Next(9); // Generují se náhodné hodnoty řádku a sloupce pro výběr buňky k odstranění
                int col = random.Next(9);

                if (!cellsToRemove.Contains(Tuple.Create(row, col))) // Test zda už není odstraněná
                {
                    int backup = sudokuBoard[row, col];
                    sudokuBoard[row, col] = 0;

                    if (!HasUniqueSolution()) // Test zda odstranění v buňce vytváří jednoznačné řešení
                    {
                        sudokuBoard[row, col] = backup; // Není jednoznačné řešení, vrátíme číslo zpět
                    }
                    else
                    {
                        cellsToRemove.Add(Tuple.Create(row, col));
                    }
                }
            }
            foreach (var cellToRemove in cellsToRemove) // Odstranění čísel
            {
                int row = cellToRemove.Item1;
                int col = cellToRemove.Item2;
                sudokuBoard[row, col] = 0;

            }
        }

        private bool HasUniqueSolution() // Zjišťuje jednoznačnost řešení hry 
        {
            int solutions = CountSolutions(); 
            return solutions == 1;
        }

        private int CountSolutions() // Počítá počet všech možných řešení
        {
            int row, col;
            if (!FindEmptyCell(out row, out col)) // Existence prázdné buňky v sudoku
                return 1;

            int count = 0;

            for (int num = 1; num <= 9; num++)
            {
                if (IsValidMove(row, col, num)) // Test čísla 
                {
                    sudokuBoard[row, col] = num;
                    count += CountSolutions(); // Rekurzivní volání metody CountSolutions() pro další pokračování v hledání řešení

                    if (count > 1)
                        return count; 

                    sudokuBoard[row, col] = 0;
                }
            }

            return count;
        }





        private void checkButton_Click(object sender, EventArgs e)
        {
            bool allCorrect = true;
            bool allFilled = true;

            for (int row = 0; row < 9; row++) // kontrola čísel
            {
                for (int col = 0; col < 9; col++)
                {
                    int value;
                    if (int.TryParse(textBoxes[row, col].Text, out value)) // Převedení hodnoty z textboxu do value
                    {
                        if (value == checkBoard[row, col]) // Pokud se rovná, uzamkne se 
                        {
                            textBoxes[row, col].ReadOnly = true;
                            textBoxes[row, col].Enabled = false;

                        }
                        else
                        {
                            textBoxes[row, col].ForeColor = Color.Red; // Pokud se nerovná, zvýrazní číslici červenou barvou
                            allCorrect = false; 
                        }

                    }
                    else
                    {
                        textBoxes[row, col].ForeColor = Color.Red; // Zbarví písmena červeně
                        allCorrect = false;
                        
                    }

                    if (string.IsNullOrWhiteSpace(textBoxes[row, col].Text)) // Test zda je textbox prázdný
                    {
                        allFilled = false;
                    }

                }
            }
            if (allCorrect) // Pokud jseou všechna pole vyplněná a vše je správně, ukončila se hra
            {
                if (allFilled)
                {
                    MessageBox.Show("Gratuluji, úspěšně jsi vyřešil sudoku!");
                }
            }
        }


        private void TextBox_KeyPress(object sender, KeyPressEventArgs e) // Nemůžeme napsat písmeno
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Zamezí zadání písmen a jiných znaků
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e) // Nemůžeme napsat nějaká jiná číslo než čísla 1-9 
        {
            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                int value = int.Parse(textBox.Text);
                if (value < 1 || value > 9)
                {
                    textBox.Text = ""; // Smazání textu, pokud je hodnota mimo rozsah
                }
            }
        }

        private void InitializeTextBoxes() // Propojení TextBox_TextChanged a TextBox_KeyPress s TableLayoutPanel s textboxy
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    textBoxes[row, col].KeyPress += TextBox_KeyPress; // Přiřazení události pro všechny textboxy
                    textBoxes[row, col].TextChanged += TextBox_TextChanged; // Přiřazení události pro všechny textboxy
                }
            }
        }

        private void AttachEnterEventHandlers() // Metoda pro propojení změny pro vstup do textboxu v TableLayoutPanelu
        {
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Enter += TextBox_Enter;
                }
            }
        }

        private void TextBox_Enter(object sender, EventArgs e) // Metoda pro změnu textboxu, když do něj vstoupím
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.ForeColor == Color.Red) // Změna zpátky na černou z červené pokud hráč klikne na pole s červenou číslicí, aby ji opravil
            {
                textBox.ForeColor = Color.Black;
            }
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}

