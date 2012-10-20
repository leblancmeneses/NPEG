if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.ParsingExpressionGrammarException = function(message) {
    this.toString = function() {
        return message;
    }

    this.getMessage = function() {return message;}
}
