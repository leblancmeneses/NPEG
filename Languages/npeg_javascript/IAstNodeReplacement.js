if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{
	IAstNodeReplacement : function()
	{
		this.implementIAstNodeVisitor = IAstNodeVisitor;
		this.implementIAstNodeVisitor();
		this.VisitEnter = null;
		this.VisitExecute = null;
		this.VisitLeave = null;
	}
	IAstNodeReplacement.prototype  = new AstNode;
}
