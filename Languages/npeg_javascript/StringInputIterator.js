if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg = 
{
	StringInputIterator : function(input)
	{	
		InputIterator.prototype.constructor.call(this, input.length);
		this.input = input;
		
		this.Text = function(start, end)
		{
			var n = end - start;
			var bytes = new Array();
			for(i = 0; i < n; i++)
			{
				bytes[i] = this.input[start+i];
			}
			return bytes;
		};
		
		this.Current = function
		{
			if (this.index >= this.length) return -1;
            else 
            {
                return this.input[this.index];
            }		
		};
		
		this.Next = function () 
		{
			if (this.index >= this.length) return -1;
            else 
            {
                this.index += 1;

                if (this.index >= this.length) return -1;
                else return this.input[this.index];
            }		
		};
		
		this.Previous = function () 
		{ 
			if (this.index <= 0)
            {
                return -1;
            }
            else 
            {
                if(this.length <= 0)throw "Error" ;
                this.index -= 1;
                return this.input[this.index];
            }
		};
	}
	StringInputIterator.prototype = new InputIterator;
}