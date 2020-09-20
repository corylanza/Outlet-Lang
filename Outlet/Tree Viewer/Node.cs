using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outlet.TreeViewer
{
    public abstract class Node
    {
        public List<Node> Children { get; private set; }
        public abstract string Text { get; }

        protected Node(params Node[] children)
        {
            Children = children.ToList();
        }
    }

    public class OperationNode : Node
    {
        public string Operation { get; set; }
        public override string Text => Operation;

        public OperationNode(string op, params Node[] children) : base(children)
        {
            Operation = op;
        }
    }

    public class PrimitiveNode : Node
    {
        public string Value { get; set; }
        public override string Text => Value;

        public PrimitiveNode(string val) : base()
        {
            Value = val;
        }
    }

    public class VariableNode : Node
    {
        private string Id { get; set; }
        public override string Text => Id;

        public VariableNode(string id) : base()
        {
            Id = id;
        }
    }

    public class ControlFlow : Node
    {
        public override string Text => "";

        public ControlFlow() : base()
        {

        }
    }
}
