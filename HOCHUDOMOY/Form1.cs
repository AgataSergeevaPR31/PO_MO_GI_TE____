using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace HOCHUDOMOY
{
    public partial class Form1 : Form
    {
        private RichTextBox richTextBox = new RichTextBox();
        private ToolStripStatusLabel statusLabel = new ToolStripStatusLabel();

        public Form1()
        {
            InitializeComponent();
            this.Text = "Блокнот";
            this.Width = 800;
            this.Height = 600;

            //создаем меню
            MenuStrip menuStrip = new MenuStrip();
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            //меню "Файл"
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("Файл");
            menuStrip.Items.Add(fileMenu);
            fileMenu.DropDownItems.Add("Создать", null, NewFile);
            fileMenu.DropDownItems.Add("Открыть", null, OpenFile);
            fileMenu.DropDownItems.Add("Сохранить", null, SaveFile);
            fileMenu.DropDownItems.Add("Выход", null, ExitApp);

            //меню "Правка"
            ToolStripMenuItem editMenu = new ToolStripMenuItem("Правка");
            menuStrip.Items.Add(editMenu);
            editMenu.DropDownItems.Add("Отменить", null, Undo);
            editMenu.DropDownItems.Add("Вырезать", null, Cut);
            editMenu.DropDownItems.Add("Копировать", null, Copy);
            editMenu.DropDownItems.Add("Вставить", null, Paste);
            editMenu.DropDownItems.Add("Удалить", null, DeleteText);
            editMenu.DropDownItems.Add("Найти", null, FindText);
            editMenu.DropDownItems.Add("Заменить", null, ReplaceText);
            editMenu.DropDownItems.Add("Выделить всё", null, SelectAll);
            editMenu.DropDownItems.Add("Вставить картинку", null, InsertImage);

            //меню "Формат"
            ToolStripMenuItem formatMenu = new ToolStripMenuItem("Формат");
            menuStrip.Items.Add(formatMenu);
            formatMenu.DropDownItems.Add("Шрифт", null, ChangeFont);
            formatMenu.DropDownItems.Add("Время и дата", null, InsertDateTime);

            richTextBox.Location = new Point(0, menuStrip.Height + 10); // Отступ сверху
            richTextBox.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - menuStrip.Height - 30); // Учитываем StatusStrip
            richTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox.TextChanged += UpdateStatus;
            this.Controls.Add(richTextBox);

            //Добавляем StatusStrip
            StatusStrip statusStrip = new StatusStrip();
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
            UpdateStatus(null, null);
        }

        private void InsertImage(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png;*.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image image = Image.FromFile(openFileDialog.FileName);
                    Clipboard.SetImage(image);
                    richTextBox.Paste();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при вставке изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void UpdateStatus(object sender, EventArgs e)
        {
            int charCount = richTextBox.Text.Length; // Количество символов
            int lineCount = richTextBox.Lines.Length; // Количество строк
            statusLabel.Text = $"Символов: {charCount} | Строк: {lineCount}";
        }


        //методы для меню "Файл"
        private void NewFile(object sender, EventArgs e)
        {
            richTextBox.Clear(); // Очищаем текстовое поле
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void SaveFile(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, richTextBox.Text);
            }
        }

        private void ExitApp(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //методы для меню "Правка"
        private void Undo(object sender, EventArgs e)
        {
            if (richTextBox.CanUndo)
                richTextBox.Undo();
        }

        private void Cut(object sender, EventArgs e)
        {
            richTextBox.Cut();
        }

        private void Copy(object sender, EventArgs e)
        {
            richTextBox.Copy();
        }

        private void Paste(object sender, EventArgs e)
        {
            richTextBox.Paste();
        }

        private void DeleteText(object sender, EventArgs e)
        {
            richTextBox.SelectedText = "";
        }

        private void FindText(object sender, EventArgs e)
        {
            string searchText = Prompt.ShowDialog("Введите текст для поиска:", "Найти");
            int index = richTextBox.Text.IndexOf(searchText);
            if (index >= 0)
            {
                richTextBox.Select(index, searchText.Length);
                richTextBox.Focus();
            }
            else
            {
                MessageBox.Show("Текст не найден.");
            }
        }

        private void ReplaceText(object sender, EventArgs e)
        {
            string searchText = Prompt.ShowDialog("Введите текст для замены:", "Найти");
            string replaceText = Prompt.ShowDialog("Введите новый текст:", "Заменить");
            if (string.IsNullOrEmpty(replaceText))
            {
                MessageBox.Show("Замена не выполнена, так как новый текст не введен.", "Предупреждение");
                return;
            }

            richTextBox.Text = richTextBox.Text.Replace(searchText, replaceText);
        }

        private void SelectAll(object sender, EventArgs e)
        {
            richTextBox.SelectAll();
        }

        //методы для меню "Формат"
        private void ChangeFont(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                if (richTextBox.SelectionLength > 0)
                {
                    richTextBox.SelectionFont = fontDialog.Font;
                }
                else
                {
                    richTextBox.Font = fontDialog.Font;
                }
            }
        }

        private void InsertDateTime(object sender, EventArgs e)
        {
            richTextBox.AppendText(DateTime.Now.ToString());
        }

        //вспомогательный класс для ввода текста
        public static class Prompt
        {
            public static string ShowDialog(string text, string caption)
            {
                Form prompt = new Form()
                {
                    Width = 400,
                    Height = 150,
                    Text = caption
                };
                Label label = new Label() { Left = 10, Top = 10, Text = text, Width = 360 };
                TextBox textBox = new TextBox() { Left = 10, Top = 40, Width = 360 };
                Button confirmation = new Button() { Text = "OK", Left = 280, Width = 90, Top = 70 };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(label);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.ShowDialog();
                return textBox.Text;
            }
        }
    }
}
