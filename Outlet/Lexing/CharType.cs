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
            SetRange(CharType.Whitespace, 9, 11); // tab
            SetRange(CharType.Whitespace, 32, 33); // Space
            SetRange(CharType.NewLine, 10, 11); // new line
            SetRange(CharType.NewLine, 13, 14); // carriage return / new line
            SetRange(CharType.Dot, 46, 47); // .
            SetRange(CharType.Quo, 34, 35); // "
            SetRange(CharType.OneChar, 40, 42); // ()
            SetRange(CharType.OneChar, 91, 92); // [
            SetRange(CharType.OneChar, 93, 94); // ]
            SetRange(CharType.OneChar, 123, 124); // {
            SetRange(CharType.OneChar, 125, 126); // }
            SetRange(CharType.OneChar, 58, 60); // : ;
            SetRange(CharType.OneChar, 44, 45); // ,
            SetRange(CharType.Plus, 43, 44); // +
            SetRange(CharType.Minus, 45, 46); // -
            SetRange(CharType.ForwardSlash, 47, 48); // /
            SetRange(CharType.OpEq, 42, 43); // *
            SetRange(CharType.OpEq, 37, 38); // %
            SetRange(CharType.OpEq, 33, 34); // !
            SetRange(CharType.LT, 60, 61); // *
            SetRange(CharType.GT, 62, 63); // *
            SetRange(CharType.Equals, 61, 62); // *
			SetRange(CharType.And, 38, 39);// &
			SetRange(CharType.Or, 124, 125); // |
			SetRange(CharType.OneChar, 94, 95); // ^ xor
			SetRange(CharType.Question, 63, 64); // ? ternary
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
        NewLine,
        Dot,
        Quo,
		Question,
        OneChar, // single character operators ()[]{},;
        // operators that could possibly be followed by an equals sign *%!
        Plus,
        Minus,
        OpEq, 
        Equals, //  =
        LT,
        GT,
		And,
		Or,
        ForwardSlash,
        BackSlash,

	}
}
