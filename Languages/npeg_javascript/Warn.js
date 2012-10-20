if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.Warn = function (message, position) {
    var message = (message != undefined) ? message : '',
        position = (position != undefined) ? position : null;

    this.getMessage = function () { 	return message; };
    this.setMessage = function ( value )	{	message = value;	};

    this.getPosition = function ()	{	return position;	};
    this.setPosition = function ( value ) {	position = value;	};
}