if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{
	IAstNodeVisitor : function()
	{
		this.VisitEnter = null;
		this.VisitExecute = null;
		this.VisitLeave = null;
	}
}