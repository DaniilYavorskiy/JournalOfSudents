using JournalOfStudents.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;

namespace JournalOfStudents.ViewModels {
    public class MainWindowViewModel: ViewModelBase {
        public static MainWindowViewModel? inst;
        private readonly int[] cb_data = { -1, -1, -1, -1 };

        private String cb_avg = "Error", status = "Не все поля заполнены", student_name = "";
        private bool active_add = false, active_grind = false;
        private string last_student_name = "";
        public String CbAvg {
            get => cb_avg;
            set => this.RaiseAndSetIfChanged(ref cb_avg, value);
        }
        public String Status {
            get => status;
            set => this.RaiseAndSetIfChanged(ref status, value);
        }
        public String StudentName {
            get => student_name;
            set { student_name = value; Cb_calc(); }
        }
        public bool ActiveAdd {
            get => active_add;
            set => this.RaiseAndSetIfChanged(ref active_add, value);
        }
        public bool ActiveGrind {
            get => active_grind;
            set => this.RaiseAndSetIfChanged(ref active_grind, value);
        }


        private void Cb_calc() {
            int S = 0;
            foreach (int i in cb_data) {
                if (i == -1) {
                    CbAvg = "Error";
                    Status = "Не все поля заполнены";
                    ActiveAdd = false;
                    ActiveGrind = false;
                    return;
                }
                S += i;
            }
            CbAvg = String.Format("{0:F2}", (float) S / 4);
            String name = NormalizeStr.NormalizeTitleStr(student_name);
            if (name.Split(" ").Length != 3) {
                Status = "Ошибка в имени";
                ActiveAdd = false;
                ActiveGrind = false;
                return;
            }
            int new_pos = FindPosList(name);
            bool repeat_fio = new_pos == -1;
            Status = repeat_fio ? "Имя уже существует" : name;
            ActiveAdd = !repeat_fio;
            ActiveGrind = repeat_fio;
            last_student_name = name;
        }



        private readonly ObservableCollection<Student> studentList = new() {
            new Student("Зайцев А. П.", new int[] {2, 1, 2, 0}),
            new Student("Ежова К. М.", new int[] {1, 2, 0, 2}),
            new Student("Григорьев В.И.", new int[] {1, 2, 0, 2})
        };
        public ObservableCollection<Student> StudentList { get => studentList; }

        private void Upd_stud_list() {
            Cb_calc();
            GlobalUpdate();
        }
        private int FindPosList(String name, bool eq = false) {
            int L = 0, R = studentList.Count;
            while (L < R) {
                int M = (L + R) / 2;
                int cmp = string.Compare(name, studentList[M].Name, StringComparison.Ordinal);
                if (cmp == 0) return eq ? M : -1;
                if (cmp < 0) R = M;
                else L = M + 1;
            }
            return eq ? -1 : L;
        }



        private string glob_1 = "", glob_2 = "", glob_3 = "", glob_4 = "", glob_avg = "";
        public string GlobA { get => glob_1; set => this.RaiseAndSetIfChanged(ref glob_1, value); }
        public string GlobB { get => glob_2; set => this.RaiseAndSetIfChanged(ref glob_2, value); }
        public string GlobC { get => glob_3; set => this.RaiseAndSetIfChanged(ref glob_3, value); }
        public string GlobD { get => glob_4; set => this.RaiseAndSetIfChanged(ref glob_4, value); }
        public string GlobAVG { get => glob_avg; set => this.RaiseAndSetIfChanged(ref glob_avg, value); }
        public void GlobalUpdate() {
            int a = 0, b = 0, c = 0, d = 0, L = studentList.Count;
            if (L == 0) {
                GlobA = GlobB = GlobC = GlobD = GlobAVG = "Error";
                return;
            }
            foreach (Student student in studentList) {
                a += student.ScoreA;
                b += student.ScoreB;
                c += student.ScoreC;
                d += student.ScoreD;
            }
            float a_val = (float) a / L;
            float b_val = (float) b / L;
            float c_val = (float) c / L;
            float d_val = (float) d / L;
            float avg_val = (float) (a + b + c + d) / (4 * L);

            GlobA = String.Format("{0:F3}", a_val);
            GlobB = String.Format("{0:F3}", b_val);
            GlobC = String.Format("{0:F3}", c_val);
            GlobD = String.Format("{0:F3}", d_val);
            GlobAVG = String.Format("{0:F4}", avg_val);
        }

        public int Cb_1 {
            get => cb_data[0] + 1;
            set { cb_data[0] = value - 1; Cb_calc(); }
        }
        public int Cb_2 {
            get => cb_data[1] + 1;
            set { cb_data[1] = value - 1; Cb_calc(); }
        }
        public int Cb_3 {
            get => cb_data[2] + 1;
            set { cb_data[2] = value - 1; Cb_calc(); }
        }
        public int Cb_4 {
            get => cb_data[3] + 1;
            set { cb_data[3] = value - 1; Cb_calc(); }
        }



        private void FuncAddStudent() {
            int pos = FindPosList(last_student_name);
            if (pos == -1) return;
            studentList.Insert(pos, new Student(last_student_name, cb_data));
            Upd_stud_list();
        }
        private void FuncGrindStudent() {
            int pos = FindPosList(last_student_name, true);
            if (pos == -1) return;
            studentList.RemoveAt(pos);
            Upd_stud_list();
        }


        
        String base_path = "../../../../BDStud.asd"; 
        public void Set_test_mode() => base_path = "../../../../TestBDStud.asd";
        private void FuncExport() {
            byte[] compressed = Gzip.PackAllStudents(studentList);
            File.WriteAllBytes(base_path, compressed);
            Status = "База данных сохранена";
        }
        private void FuncImport() {
            if (!File.Exists(base_path)) {
                Status = "Файл базы данных не найден";
                return;
            }
            byte[] compressed = File.ReadAllBytes(base_path);
            Gzip.UnpackAllStudents(studentList, compressed);
            Upd_stud_list();
            Status = "База данных загружена";
        }

        public MainWindowViewModel() {
            inst = this;
            GlobalUpdate();

            AddStudent = ReactiveCommand.Create<Unit, Unit>(_ => { FuncAddStudent(); return new Unit(); });
            GrindStudent = ReactiveCommand.Create<Unit, Unit>(_ => { FuncGrindStudent(); return new Unit(); });
            Export = ReactiveCommand.Create<Unit, Unit>(_ => { FuncExport(); return new Unit(); });
            Import = ReactiveCommand.Create<Unit, Unit>(_ => { FuncImport(); return new Unit(); });
        }
        public ReactiveCommand<Unit, Unit> AddStudent { get; }
        public ReactiveCommand<Unit, Unit> GrindStudent { get; }
        public ReactiveCommand<Unit, Unit> Export { get; }
        public ReactiveCommand<Unit, Unit> Import { get; }
    }
}
