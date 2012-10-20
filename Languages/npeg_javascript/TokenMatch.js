
if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.TokenMatch = function (name, start, end) {
    var name = name,
        start = start,
        end = end;

    this.getName = function () {return name;};
    this.setName = function ( value ) {	name = value;	};

    this.getStart = function ()	{	return start;	};
    this.setStart = function ( value ) {	start = value;	};

    this.getEnd = function ()	{ return end; };
    this.setEnd = function ( value ){	end = value;	};
}

