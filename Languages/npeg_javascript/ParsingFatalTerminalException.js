if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.ParsingFatalTerminalException = function(message, pos) {
    var m_errorPos = pos,
        message = (typeof message == undefined) ? '' : message;

    RobustHaven.Text.Npeg.ParsingFatalTerminalException.superclass.constructor.call(this, message);

    this.getErrorPosition = function() {
        return m_errorPos;
    }
}
RobustHaven.Text.Npeg.extend(
    RobustHaven.Text.Npeg.ParsingFatalTerminalException, RobustHaven.Text.Npeg.ParsingExpressionGrammarException);