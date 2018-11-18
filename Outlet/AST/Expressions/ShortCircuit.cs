﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;

namespace Outlet.AST {
	public class ShortCircuit : Expression {

		public readonly Expression Left;
		public readonly Expression Right;
		public readonly bool isand; // if true the operator is and, if not it is or

		public ShortCircuit(Expression left, BinaryOperator op, Expression right) {
			Left = left;
			Right = right;
			if (op == Operator.LogicalAnd) isand = true;
			else if (op == Operator.LogicalOr) isand = false;
			else throw new Exception("ShortCircuit expression is only valid for logical and and or");
		}
		/*
		public override Operand Eval(Scope scope) {
			bool b = Left.Eval(scope).Value;
			if (isand) {
				if (b) {
					bool r = Right.Eval(scope).Value;
					return new Const(r);
				} return new Const(false);
			} else {
				if (!b) {
					bool r = Right.Eval(scope).Value;
					return new Const(r);
				} return new Const(true);
			}
		}

		public override void Resolve(Scope scope) {
			Left.Resolve(scope);
			Right.Resolve(scope);
		}*/

		public override T Accept<T>(IVisitor<T> visitor) {
			return visitor.Visit(this);
		}

		public override string ToString() {
			if (isand) return "(" + Left.ToString() + " && " + Right.ToString() + ")";
			return "(" + Left.ToString() + " || " + Right.ToString() + ")";
		}
	}
}
