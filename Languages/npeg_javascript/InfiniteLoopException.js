if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.InfiniteLoopException = function(message, pos) {
    var message = (typeof message == "undefined") ? "Supplied grammar rules caused an infinite loop." : message;
    RobustHaven.Text.Npeg.InfiniteLoopException.superclass.constructor.call(this, message, pos);
}
RobustHaven.Text.Npeg.extend(
    RobustHaven.Text.Npeg.InfiniteLoopException, RobustHaven.Text.Npeg.ParsingFatalTerminalException);