using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Lexing {

    public static class CharClass {

        private static CharType[] ASCII = new CharType[128];

        static CharClass() {
            void SetRange(CharType c, int start, int end) {
                for(int i = start; i < end; i++) {
                    ASCII[i] = c;
                }
            }
            SetRange(CharType.Letter, 65, 91); // A-Z
            SetRange(CharType.Letter, 97, 123); // a-z
            SetRange(CharType.Number, 48, 58); // 0-9
            SetRange(CharType.Whitespace, 9, 11); // tab and new line
            SetRange(CharType.Whitespace, 32, 33); // Space
            SetRange(CharType.Dot, 46, 47);
            SetRange(CharType.Quo, 34, 35);

        }

        public static CharType Get(int c) {
            if(c >= 0 && c < 128) return ASCII[c];
            else throw new NotImplementedException("illegal char");
        }
    }

	public enum CharType {
        Illegal = 0,
        Letter,
		Number,
		Whitespace,
        Dot,
        Quo,
        Op1, // single character operator ()
        Op2, // two character operator
        ForwardSlash,
        BackSlash,

	}
}
