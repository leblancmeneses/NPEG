if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.IAstNodeVisitor = function() {
    this.visitEnter = function(node) {};
    this.visitExecute = function(node) {};
    this.visitLeave = function(node) {};
}