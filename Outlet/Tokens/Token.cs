using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Tokens {

	public abstract class Token {

		public static bool ContainsKey(string text) => Tokens.ContainsKey(text);
		public static Token Get(string text) => Tokens[text];


		private static readonly Dictionary<string, Token> Tokens = new Dictionary<string, Token>() {
			// Keywords
			{"class", Keyword.Class },
			{"static", Keyword.Static },
			{"extends", Keyword.Extends },
			//{"operator", Keyword.Operator },
			{"if", Keyword.If },
			{"then", Keyword.Then },
			{"else", Keyword.Else },
            {"for", Keyword.For },
			{"in", Keyword.In },
			{"while", Keyword.While },
			{"return", Keyword.Return },
            {"using", Keyword.Using },
            {"var", Keyword.Var },
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
			{"as", Operator.As },
			{"is", Operator.Is },
			{"isnt", Operator.Isnt },
			{"?", Operator.Question },
			{".", Operator.Dot },
			//{"..", Operator.ExcRange},
			//{"...", Operator.IncRange},
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
}
