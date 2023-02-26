using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using JournalOfStudents.Models;
using JournalOfStudents.ViewModels;
using JournalOfStudents.Views;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UITestsForJournalOfStudents {
    public class Tests {
        private MainWindow mainWindow = AvaloniaApp.GetMainWindow();
        private Button button_AS, button_GS, button_E, button_I;
        private TextBox text_box;
        private ComboBox cb_1, cb_2, cb_3, cb_4;
        private TextBlock cb_avg, status, glob_a, glob_b, glob_c, glob_d, glob_avg;
        private ListBox stud_list;
        public Tests() {
            var buttons = mainWindow.GetVisualDescendants().OfType<Button>();

            button_AS = buttons.First(b => b.Name == "addStudent");
            button_GS = buttons.First(b => b.Name == "grindStudent");
            button_E = buttons.First(b => b.Name == "export");
            button_I = buttons.First(b => b.Name == "import");

            var text_boxes = mainWindow.GetVisualDescendants().OfType<TextBox>();
            text_box = text_boxes.First(b => b.Name == "studentName");

            var combo_boxes = mainWindow.GetVisualDescendants().OfType<ComboBox>();

            cb_1 = combo_boxes.First(b => b.Name == "cb_1");
            cb_2 = combo_boxes.First(b => b.Name == "cb_2");
            cb_3 = combo_boxes.First(b => b.Name == "cb_3");
            cb_4 = combo_boxes.First(b => b.Name == "cb_4");

            var text_blocks = mainWindow.GetVisualDescendants().OfType<TextBlock>();

            cb_avg = text_blocks.First(b => b.Name == "cbAvg");
            status = text_blocks.First(b => b.Name == "status");
            glob_a = text_blocks.First(b => b.Name == "glob_a");
            glob_b = text_blocks.First(b => b.Name == "glob_b");
            glob_c = text_blocks.First(b => b.Name == "glob_c");
            glob_d = text_blocks.First(b => b.Name == "glob_d");
            glob_avg = text_blocks.First(b => b.Name == "globAvg");

            var list_boxes = mainWindow.GetVisualDescendants().OfType<ListBox>();
            stud_list = list_boxes.First(b => b.Name == "studentList");

            MainWindowViewModel? MWVM = mainWindow.DataContext as MainWindowViewModel;
            if (MWVM != null) MWVM.Set_test_mode();
        }
        private void Press_AS() => button_AS.Command.Execute(button_AS.CommandParameter);
        private void Press_GS() => button_GS.Command.Execute(button_GS.CommandParameter);
        private void Press_I() => button_E.Command.Execute(button_E.CommandParameter);
        private void Press_E() => button_I.Command.Execute(button_I.CommandParameter);
        private void Write(String data) => text_box.Text = data;
        private void SetCombo(int a, int b, int c, int d) {
            cb_1.SelectedIndex = a;
            cb_2.SelectedIndex = b;
            cb_3.SelectedIndex = c;
            cb_4.SelectedIndex = d;
        }
        private string Status() => status.Text;
        private string Globals() => String.Format("{0} {1} {2} {3} {4}", glob_a.Text, glob_b.Text, glob_c.Text, glob_d.Text, glob_avg.Text);
        private string SBAvg() => cb_avg.Text;
        private string GetVisible() => (button_AS.IsEnabled ? "+" : "-") + (button_GS.IsEnabled ? "+" : "-");
        private string TextAll() => String.Format("{0}|{1}|{2}|{3}", Status(), Globals(), SBAvg(), GetVisible());

        private string GetColor(ComboBox box) {
            var border = box.Parent as Border;
            if (border == null) return "Trash";
            var color = border.Background as ISolidColorBrush;
            if (color == null) return "Scrap";
            return color.Color.ToString();
        }
        private string GetColor(TextBlock block) {
            var border = block.Parent as Border;
            if (border == null) return "Trash";
            var color = border.Background as ISolidColorBrush;
            if (color == null) return "Scrap";
            return color.Color.ToString();
        }
        private string AllColors() => String.Format("{0}|{1}|{2}|{3}|{4}", GetColor(glob_a), GetColor(glob_b), GetColor(glob_c), GetColor(glob_d), GetColor(glob_avg));
        
        private string RowLoader(object? row) {
            if (row == null) return "Trash";
            var stud = row as Student;
            if (stud == null) return "Scrap";
            return stud.Pack();
        }
        private string SuperLoader() {
            StringBuilder sb = new();
            bool first = true;
            foreach (var item in stud_list.Items) {
                if (first) first = false;
                else sb.Append("~");
                sb.Append(RowLoader(item));
            }
            return sb.ToString();
        }

        [Fact]
        public async void GeneralTest() {
            await Task.Delay(10);
            Assert.Equal("Есть пустые КС|1,333 1,333 0,333 1,667 1,1667|Error|--", TextAll());
            Assert.Equal("Yellow|Yellow|Red|Green|Yellow", AllColors());
            Assert.Equal("А А А|2,0,1,1~В В В|1,2,0,2~И И И|1,2,0,2", SuperLoader());

            SetCombo(2, 1, 0, 3);
            await Task.Delay(1);
            Assert.Equal("Есть пустые КС|1,333 1,333 0,333 1,667 1,1667|Error|--", TextAll());

            SetCombo(2, 1, 1, 3);
            await Task.Delay(1);
            Assert.Equal("Невалидное ФИО|1,333 1,333 0,333 1,667 1,1667|0,75|--", TextAll());

            Write("   fF  FSd     dS  ");
            await Task.Delay(1);
            Assert.Equal("0 | Ff Fsd Ds|1,333 1,333 0,333 1,667 1,1667|0,75|+-", TextAll());

            Press_AS();
            await Task.Delay(1);
            Assert.Equal("Такое ФИО уже есть|1,250 1,000 0,250 1,750 1,0625|0,75|-+", TextAll());
            Assert.Equal("Yellow|Yellow|Red|Green|Yellow", AllColors());

            Write("  Зайцев АЛЕКСАНДР ИгоРЕвИч");
            SetCombo(3, 3, 3, 3);
            await Task.Delay(1);
            Assert.Equal("4 | Зайцев Александр Игоревич|1,250 1,000 0,250 1,750 1,0625|2,00|+-", TextAll());

            Press_AS();
            await Task.Delay(1);
            Assert.Equal("Такое ФИО уже есть|1,400 1,200 0,600 1,800 1,2500|2,00|-+", TextAll());
            Assert.Equal("Yellow|Yellow|Red|Green|Yellow", AllColors());
            Assert.Equal("Ff Fsd Ds|1,0,0,2~А А А|2,0,1,1~В В В|1,2,0,2~И И И|1,2,0,2~Зайцев Александр Игоревич|2,2,2,2", SuperLoader());

            String path = "../../../../TestStudBase.asd";
            if (File.Exists(path)) File.Delete(path);
            Assert.False(File.Exists(path));
            Press_I();
            Assert.True(File.Exists(path));

            Write("Зайцев АЛЕКСАНДР ИгоРЕвИч");
            await Task.Delay(1);
            Assert.Equal("Такое ФИО уже есть|1,400 1,200 0,600 1,800 1,2500|2,00|-+", TextAll());

        }
    }
}