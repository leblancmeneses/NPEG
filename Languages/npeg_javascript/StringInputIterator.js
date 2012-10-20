if(!RobustHaven) var RobustHaven={};
if(!RobustHaven.Text) RobustHaven.Text={};
if(!RobustHaven.Text.Npeg) RobustHaven.Text.Npeg={};

RobustHaven.Text.Npeg.StringInputIterator = function (input, len) {
   var len = (len == undefined) ? input.length : len;
   RobustHaven.Text.Npeg.StringInputIterator.superclass.constructor.call(this, len);

    this.input = input;

    this.getText = function(start, end) {
        var n = end - start,
            bytes = new Array();

        if (this.input.length == 0 || end < start) {
            return null;
        }

        for (var i = 0; i < n; i++) {
            if (this.input[start+i] != undefined) {
                bytes[i] = this.input[start+i];
            }
        }

        return bytes;
    };

    this.getTextString = function(start, end) {
        var bytes = this.getText(start, end),
            string = "";
        for (var i=0; i < bytes.length; i++) {
            string += bytes[i];
        }

        return string;
    }

    this.getCurrent = function () {
        if (this.index >= this.length) {
            return -1;
        } else {
            return this.input[this.index];
        }
    };

    this.getNext = function () {
        if (this.index >= this.length) {
            return -1;
        } else {
            this.index += 1;

            if (this.index >= this.length) return -1;
            else return this.input[this.index];
        }
    };

    this.getPrevious = function () {
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
RobustHaven.Text.Npeg.StringInputIterator.stringToBytes = function(str) {
    var ch, st, re = [];
    for (var i = 0; i < str.length; i++ ) {
        ch = str.charCodeAt(i);  // get char
        st = [];                 // set up "stack"
        do {
            st.push( ch & 0xFF );  // push byte to stack
            ch = ch >> 8;          // shift value down by 1 byte
        }
        while ( ch );
        // add stack contents to result
        // done because chars have "wrong" endianness
        re = re.concat( st.reverse() );
    }
    // return an array of bytes
    return re;
}
RobustHaven.Text.Npeg.extend(
    RobustHaven.Text.Npeg.StringInputIterator, RobustHaven.Text.Npeg.InputIterator);