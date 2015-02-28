(function(exports) {
	var reCommentLine = new RegExp("^\\s*#");
	var reEmptyLine = new RegExp("^\\s*$");

	function isOfArrayType(o) {
		return Object.prototype.toString.call(o) === '[object Array]';
	}

	function ParseState(text) {
		this.lines = text.split("\n").slice(1);
		this.index = 0;
		this.currentIndent = 0;	
		this.objectStack = [{}];
		this.currentObject = function() {
			return this.objectStack[this.objectStack.length-1];
		};
		this.addPropertyToCurrentObject = function(name, value) {
			var o = this.currentObject();
			if (isOfArrayType(o[name])) {
				o[name].push(value);
			}
			else if (o[name]) {
				o[name] = [o[name], value];
			}
			else {
				o[name] = value;
			}
		}
		this.currentLine = function() {
			return this.lines[this.index];
		};
		this.skipLine = function() {
			console.log(this.currentLine());
			++this.index;
		};
		this.eos = function() {
			return this.index >= this.lines.length;	
		};	
	}

	function parsePreprocessors(state) {
		while(state.currentLine().match(/^!/))
			state.skipLine();
	}

	function parseValueLines(state) {
		var reNonEmptyLine = new RegExp("^\\t{" + state.currentIndent + "}(.*)");
		var match;
		var stop = false;
		var result = [];
		while(!stop && !state.eos()) {
			// nonempty_line | comment_line | empty_line
			match = state.currentLine().match(reNonEmptyLine) 
			if (match) {
				result.push(match[1]);
				state.skipLine();
				continue;
			}	

			match = state.currentLine().match(reCommentLine);
			if (match) {
				state.skipLine();
				continue;
			}

			var match = state.currentLine().match(reEmptyLine);
			if (match) {
				state.skipLine();
				continue;
			}

			stop = true;
		} 

		return result.join("\n");
	}

	function tryParseValue(state) {
		var re = new RegExp("^\\t{" + state.currentIndent + "}=(.*)");
		var match = state.currentLine().match(re);
		var result;
		if (match) {
			state.skipLine();
			result = { key: match[1] }
			++state.currentIndent;
			result.value = parseValueLines(state);
			--state.currentIndent;
		}

		return result;
	}

	function tryParseObject(state) {
		var re = new RegExp("^\\t{" + state.currentIndent + "}@(.*)");
		var match = state.currentLine().match(re);
		var result;
		if (match) {
			state.skipLine();
			result = { key: match[1] }
			++state.currentIndent;
			state.objectStack.push({});
			parseMembers(state);
			result.value = state.currentObject();
			state.objectStack.pop();
			--state.currentIndent;
		}

		return result;
	}

	function parseMembers(state) {
		var stop = false;
		while(!stop && !state.eos()) {
			// value | object | comment | empty
			var value = tryParseValue(state);
			if (value) {
				state.addPropertyToCurrentObject(value.key, value.value);
				continue;
			}		

			var object = tryParseObject(state);
			if (object) {
				state.addPropertyToCurrentObject(object.key, object.value);
				continue;
			}

			if (state.currentLine().match(reCommentLine)) {
				state.skipLine();
				continue;
			}

			if (state.currentLine().match(reEmptyLine)) {
				state.skipLine();
				continue;
			}		

			stop = true;
		}
	}

	exports.parse = function(text) {
		var state = new ParseState(text);
		parsePreprocessors(state);
		parseMembers(state);
		return state.currentObject();
	}
})(this.hron = {});
