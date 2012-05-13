
if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{		
	AstNode: function ()
	{
	    this.children = new Array();
	    
	    this.getParent = function (){	return this.parent;	};
		this.setParent = function ( value )	{	this.parent = value;	};
			
	    this.getChildren = function () {	return this.children;	};
		this.setChildren = function ( value )	{	this.children = value;	};
	
	    this.getToken = function () { return this.token; 	};
		this.setToken = function ( value )	{	this.token = value; }; 
		
		this.Accept = function (visitor)
		{
			visitor.VisitEnter(this);
			
			var isFirstTime = true;
			for (var node in this.Children)
			{
			    if (!isFirstTime)
			    {
			        visitor.VisitExecute(this);
			    }
			
			    isFirstTime = false;
			    node.Accept(visitor);
			}
			
			visitor.VisitLeave(this);
		}; 
	}	
}

