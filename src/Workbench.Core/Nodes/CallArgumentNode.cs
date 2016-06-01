using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Workbench.Core.Nodes
{
    public class CallArgumentNode : AstNode
    {
        public CallArgumentNameNode Name { get; private set; }
        public CallArgumentValueNode Value { get; private set; }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Name = (CallArgumentNameNode) AddChild("name", treeNode.ChildNodes[0]);
            Value = (CallArgumentValueNode) AddChild("value", treeNode.ChildNodes[1]);
        }
    }
}
