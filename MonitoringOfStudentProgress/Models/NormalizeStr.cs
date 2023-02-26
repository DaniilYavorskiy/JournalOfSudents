using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalOfStudents.Models {
    internal class NormalizeStr {
        public static string NormalizeTitleStr(string input) {
            int len = input.Length, index = 0;
            var src = input.ToCharArray();
            byte skip = 0;
            for (int i = 0; i < len; i++) {
                char ch = src[i];
                switch (ch) {
                case '\u0020':                               
                    if (skip == 0) continue;
                    src[index++] = ' ';
                    skip = 0;
                    continue;
                default:
                    if (skip == 0) {
                        if (ch >= 'a' && ch <= 'z') ch -= ' ';
                        else if (ch >= 'а' && ch <= 'я') ch -= ' ';
                        skip = 1;
                    } else {
                        if (ch >= 'A' && ch <= 'Z') ch += ' ';
                        else if (ch >= 'А' && ch <= 'Я') ch += ' ';
                        skip = 2;
                    }
                    src[index++] = ch;
                    continue;
                }
            }
            if (index > 0 && skip == 0) index -= 1;
            return new string(src, 0, index);
        }
    }
}
