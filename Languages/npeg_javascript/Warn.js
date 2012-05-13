if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{	
	Warn: function (message, position)
	{
		if (typeof message == "undefined") 
		{
			this.message = "";
		}
		else 
		{
			if (typeof position != "undefined") 
				this.position = position;
			this.message = message;
		}
	    		
	    this.getMessage = function () { 	return this.message; };
		this.setMessage = function ( value )	{	this.message = value;	};
			
	    this.getPosition = function ()	{	return this.position;	};
		this.setPosition = function ( value ) {	this.position = value;	};	
	         
	}
}