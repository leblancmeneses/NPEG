
if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{
	TokenMatch: function (name, start, end)
	{
	    this.name = name;
	    this.start = start;
	    this.end = end;     
	    		
	    this.getName = function () { 	return this.name; };
		this.setName = function ( value )	{	this.name = value;	};
			
	    this.getStart = function ()	{	return this.start;	};
		this.setStart = function ( value ) {	this.start = value;	};
	
	    this.getEnd = function ()	{		return this.end;	};
		this.setEnd = function ( value ){	this.end = value;	};        
	}
}

