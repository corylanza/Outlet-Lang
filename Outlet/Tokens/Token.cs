using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {

	public static class Token {

		public static bool ContainsKey(string text) => Tokens.ContainsKey(text);
		public static IToken Get(string text) => Tokens[text];


		private static Dictionary<string, IToken> Tokens = new Dictionary<string, IToken>() {
			// Keywords
			{"class", Keyword.Class },
			{"static", Keyword.Static },
			{ "var", Keyword.Var },
			{ "operator", Keyword.Operator },
			{"if", Keyword.If },
			{"then", Keyword.Then },
			{"else", Keyword.Else },
            {"for", Keyword.For },
			{"in", Keyword.In },
			{"while", Keyword.While },
			{"return", Keyword.Return },
            {"true", Keyword.True }, // TODO FIX THIS
            {"false", Keyword.False },
			{ "null", Keyword.Null },
			// Operators
			{"++", Operator.PostInc },
			{"--", Operator.PostDec },
			{"+", Operator.Plus },
			{"-", Operator.Minus },
			{"/", Operator.Divide },
			{"*", Operator.Times },
			{"%", Operator.Modulus },
			{"<", Operator.LT },
			{">", Operator.GT },
			{"<<", Operator.LShift },
			{">>", Operator.RShift },
			{"~", Operator.Complement },
			{"&", Operator.BitAnd },
			{"|", Operator.BitOr },
			{"^", Operator.BitXor },
			{"and", Operator.LogicalAnd },
			{"or", Operator.LogicalOr },
			{"&&", Operator.LogicalAnd },
			{"||", Operator.LogicalOr },
			{"not", Operator.Not },
			{"!", Operator.Not },
			{"=", Operator.Equal },
			{"=>", Operator.Lambda },
			{"+=", Operator.PlusEqual },
			{"-=", Operator.MinusEqual },
			{"/=", Operator.DivEqual },
			{"*=", Operator.MultEqual },
			{"%=", Operator.ModEqual },
			{"<=", Operator.LTE },
			{">=", Operator.GTE },
			{"!=", Operator.NotEqual },
			{"==", Operator.BoolEquals },
			{"is", Operator.Is },
			{"isnt", Operator.Isnt },
			{"?", Operator.Question },
			{".", Operator.Dot },
			// Delimeters
			{"(", Delimeter.LeftParen },
			{")", Delimeter.RightParen },
			{"[", Delimeter.LeftBrace },
			{"]", Delimeter.RightBrace },
			{"}", Delimeter.RightCurly },
			{"{", Delimeter.LeftCurly },
			{",", Delimeter.Comma },
			{":", Delimeter.Colon },
			{";", Delimeter.SemiC },
		};
	}

	public interface IToken {}

	
}
