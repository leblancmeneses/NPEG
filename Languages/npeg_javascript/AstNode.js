
if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.AstNode = function (token) {
    var token = token,
        children = [],
        parent;

    this.children = new Array();

    this.getParent = function (){	return parent;	};
    this.setParent = function ( value )	{	parent = value;	};

    this.getChildren = function () {	return children;	};
    this.addChild = function(child) {
        children[children.length] = child;
    }

    this.getToken = function () { return token; 	};
    this.setToken = function ( value )	{	token = value; };

    this.accept = function (visitor) {
        visitor.visitEnter(this);

        var isFirstTime = true;
        for (var i = 0; i < children; i++) {
            var node = children[i];

            if (!isFirstTime) {
                visitor.visitExecute(this);
            }

            isFirstTime = false;
            node.accept(visitor);
        }

        visitor.visitLeave(this);
    };
}

