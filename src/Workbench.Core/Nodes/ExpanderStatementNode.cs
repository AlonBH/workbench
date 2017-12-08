using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using Irony.Interpreter.Ast;

namespace Workbench.Core.Nodes
{
    public class ExpanderStatementNode : AstNode
    {
        public CounterDeclarationNode CounterDeclaration { get; private set; }
        public ExpanderScopeNode ExpanderScope { get; private set; }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            // The expander statement can be empty and still valid...
            if (!treeNode.ChildNodes.Any()) return;
            // Child 0 is the | (pipe)
            CounterDeclaration = (CounterDeclarationNode) AddChild("counter declaration", treeNode.ChildNodes[1]);
            // Child 2 is the in keyword
            ExpanderScope = (ExpanderScopeNode) AddChild("scope", treeNode.ChildNodes[3]);
        }
    }
}
