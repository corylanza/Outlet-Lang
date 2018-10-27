﻿using System;
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
			// Primitives
			{"int", Primitive.Int },
			{"float", Primitive.Float },
			{"bool", Primitive.Bool },
			{"string", Primitive.String },
			{"object", Primitive.Object },
			{"type", Primitive.MetaType },
			// Keywords
			{"class", Keyword.Class },
			{"static", Keyword.Static },
			{ "func", Keyword.Func },
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
			{"+", Operator.Plus },
			{"-", Operator.Minus },
			{"/", Operator.Divide },
			{"*", Operator.Times },
			{"%", Operator.Modulus },
			{"<", Operator.LT },
			{">", Operator.GT },
			{"<<", Operator.LShift },
			{">>", Operator.RShift },
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

	/*
	public enum TokenType {
		None = 0,
		// One Character
		LeftParen, RightParen, LeftBrace, RightBrace,
		LeftCurly, RightCurly, Colon,
		Comma, Dot, SemiC,

		// Operators that can have = appended
		Plus, Minus, Divide, Multiply, LT, GT, Not, Equal,
		PlusEqual, MinusEqual, MultEqual, DividedEqual, LTE, GTE, NotEqual, BoolEquals,

		// multichar
		Identifier, OString, OInt, OFloat,

		// keywords
		If, Else, Null, For, While, Return, True, False,
		// functions, increment ops, bitshifts, and logical ops not represented yet


		EOF
	}*/

	public interface IToken {}

	
}
