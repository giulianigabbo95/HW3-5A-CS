using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordCloudGenerator
{
    public partial class Form1 : Form
    {
        Dictionary<string, int> keyWords = new Dictionary<string, int>();
        Dictionary<string, int> blackList = new Dictionary<string, int>();
        int totalWords = 0;

        public Form1()
        {
            InitializeComponent();

            LoadBlacklist();
        }

        // Eventi
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            BrowseAndLoad();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateWordCloud2();
        }

        // Metodi
        private void LoadBlacklist()
        {
            // Carica l'elenco delle parole da non conteggiare
            var lines = File.ReadAllLines(@"Dati\Black List.txt");
            foreach (var line in lines)
            {
                if (!blackList.ContainsKey(line))
                    blackList.Add(line, 0);
            }
        }

        private void BrowseAndLoad()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtFileName.Text = openFileDialog1.FileName;

                    var _keyWords = new Dictionary<string, int>();

                    // Carica le linee di testo del file di testo selezionato
                    var lines = File.ReadLines(openFileDialog1.FileName);
                    // Cicla su ciascuna linea
                    foreach (var line in lines)
                    {
                        // rimpiazza alcuni caratteri speciali con uno spazio
                        var cleanLine = line.Replace(':', ' ');
                        cleanLine = cleanLine.Replace(';', ' ');
                        cleanLine = cleanLine.Replace(',', ' ');
                        cleanLine = cleanLine.Replace('.', ' ');
                        cleanLine = cleanLine.Replace('_', ' ');
                        cleanLine = cleanLine.Replace('=', ' ');
                        cleanLine = cleanLine.Replace('*', ' ');
                        cleanLine = cleanLine.Replace('\'', ' ');



                        // Separa le parole della linea depurata dai caratteri speciali
                        var words = cleanLine.Split(' ');

                        // scorre l'elenco delle parole trovate 
                        foreach (var word in words)
                        {
                            // Elimina gli spazi e forza il minuscolo
                            var cleanWord = word.ToLower().Trim();

                            // Se la parola priva di spazi 1) non compare nella BlackList, 2) ha una lunghezza >=2 e 3) non rappresenta un numero
                            // essa iene aggiunta al dizionario e il relativo contatore viene incrementato di 1.
                            if (!blackList.ContainsKey(cleanWord) && word.Length >= 2 && !decimal.TryParse(word, out decimal n))
                            {
                                if (!_keyWords.ContainsKey(cleanWord))
                                    _keyWords.Add(cleanWord, 0);

                                _keyWords[cleanWord]++;
                            }
                        }
                    }

                    // Filtra solo le parole comparse più di una volta per creare il dizionario finale
                    keyWords = (from item in _keyWords where item.Value > 1 select item).ToDictionary(x => x.Key, x => x.Value);

                    // Imposta il numero totale di parole 
                    totalWords = keyWords.Count();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore.\n\nError message: {ex.Message}\n\n" + $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void GenerateWordCloud()
        {
            richTextBox1.Text = string.Empty;

            // Calcol gli elementi con il minimo e il massimo delle occorrene e divide per 5 per creare 5 classi di appartenenza
            var minOccur = (from item in keyWords select item).Min(x => x.Value);
            var maxOccur = (from item in keyWords select item).Max(x => x.Value);

            var splitter = (maxOccur - minOccur) / 5;

            // Cicla su tutte le coppie (parola, contatore) del dizionario
            foreach (KeyValuePair<string, int> word in keyWords)
            {
                var fontSize = 8;
                var fontStyle = FontStyle.Regular;
                var color = Color.Black;

                // Controlla il numero di volte che ogni parola compare nel testo e in funzione di esso assegna una dimensione e un colore 
                if (word.Value >= splitter * 3)
                {
                    fontSize = 48;
                    fontStyle = FontStyle.Bold;
                    color = Color.Orange;
                }
                else if (word.Value >= splitter * 2)
                {
                    fontSize = 24;
                    fontStyle = FontStyle.Italic;
                    color = Color.Red;
                }
                else if (word.Value >= splitter)
                {
                    fontSize = 20;
                    fontStyle = FontStyle.Underline;
                    color = Color.Blue;
                }
                else if (word.Value >= Math.Round((double)splitter / 2, 0))
                {
                    fontSize = 16;
                    fontStyle = FontStyle.Regular;
                    color = Color.LightGreen;
                }
                else if (word.Value >= Math.Round((double)splitter / 3, 0))
                {
                    fontSize = 12;
                    fontStyle = FontStyle.Regular;
                    color = Color.Cyan;
                }

                // Inserisce la parola nel RichTextBox
                richTextBox1.AppendText(word.Key);
                richTextBox1.AppendText(" ");

                // Seleziona la parola e cambia dimensione e colore
                richTextBox1.Select(richTextBox1.Text.Length - word.Key.Trim().Length - 1, word.Key.Trim().Length);
                richTextBox1.SelectionFont = new Font("Tahoma", fontSize, fontStyle);
                richTextBox1.SelectionColor = color;
            }
        }

        private void GenerateWordCloud2()
        {
            int[] fontSizes = new int[] { 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48 };
            Color[] fontColors = new Color[] { Color.Black, Color.PaleVioletRed, Color.CadetBlue, Color.Coral, Color.DarkCyan, Color.DarkKhaki, Color.DarkOrange, Color.DarkSlateGray,
                                               Color.DarkOliveGreen, Color.IndianRed, Color.OrangeRed, Color.LightPink, Color.LightSalmon, Color.LightGreen, Color.LightBlue,
                                               Color.Tomato, Color.Turquoise, Color.Violet, Color.Plum, Color.YellowGreen, Color.Sienna, Color.Silver };
            richTextBox1.Text = string.Empty;

            // Calcola gli elementi con il minimo e il massimo delle occorrene e divide per il numero di dimensioni possibili per creare altrettante classi di appartenenza
            var minOccur = (from item in keyWords select item).Min(x => x.Value);
            var maxOccur = (from item in keyWords select item).Max(x => x.Value);

            var splitter = (maxOccur - minOccur) / fontSizes.Count();

            List<Tuple<int, int, int>> styles = new List<Tuple<int, int, int>>();
            styles.Add(new Tuple<int, int, int>(0, 0, splitter));
            int i = 1;
            while (i < fontSizes.Count() - 1)
            {
                styles.Add(new Tuple<int, int, int>(i, splitter * i, splitter * (i + 1)));
                i++;
            }
            styles.Add(new Tuple<int, int, int>(i, splitter * i, maxOccur));

            // Cicla su tutte le coppie (parola, contatore) del dizionario
            foreach (KeyValuePair<string, int> word in keyWords)
            {
                // Controlla il numero di volte che ogni parola compare nel testo e in funzione di esso assegna una dimensione e un colore 
                var style = styles.Where(x => x.Item2 < word.Value && x.Item3 >= word.Value).FirstOrDefault();

                var fontSize = fontSizes[style.Item1];
                var color = fontColors[style.Item1];
                var fontStyle = FontStyle.Regular;

                // Inserisce la parola nel RichTextBox
                richTextBox1.AppendText(word.Key.Trim());
                richTextBox1.AppendText(" ");

                // Seleziona la parola e cambia dimensione e colore
                richTextBox1.Select(richTextBox1.Text.Length - word.Key.Trim().Length - 1, word.Key.Trim().Length);
                richTextBox1.SelectionFont = new Font("Tahoma", fontSize, fontStyle);
                richTextBox1.SelectionColor = color;
            }
        }
    }
}
