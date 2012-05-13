if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{	
	InputIterator : function(length)
	{
		this.index = 0;
		this.length = length;
		
		this.getIndex = function () { 	return this.index; };
		this.setIndex = function ( value )	{	this.index = value;	};
		
		this.getLength = function () { 	return this.length; };
		
		this.Text = null;
		this.Current = null;		
		this.Next = null;		
		this.Previous = null;
	}
	InputIterator.prototype.constructor = InputIterator;
}