(function(exports) {
	var reCommentLine = new RegExp("^(\\s*)#(.*)");
	var reEmptyLine = new RegExp("^(\\s*?)\\r?$");

	function isOfArrayType(o) {
		return Object.prototype.toString.call(o) === '[object Array]';
	}

	function parsePreprocessors(state) {
		var match;
		while(match = state.currentLine().match(/^!(.*)/)) {
			state.skipLine("PreProcessor", match[1]);
		}
	}

	function parseValueLines(state) {
		var reNonEmptyLine = new RegExp("^\\t{" + state.currentIndent + "}(.*)");
		var match;
		var stop = false;
		var result = [];
		while(!stop && !state.eos()) {
			match = state.currentLine().match(reNonEmptyLine) 
			if (match) {
				result.push(match[1]);
				state.skipLine("ContentLine", match[1]);
				continue;
			}	

			match = state.currentLine().match(reCommentLine);
			if (match) {
				state.skipLine("CommentLine", match[1].length + "," + match[2]);
				continue;
			}

			match = state.currentLine().match(reEmptyLine);
			if (match) {
				state.skipLine("EmptyLine", match[1]);
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
			state.skipLine("Value_Begin", match[1]);
			result = { key: match[1] }
			++state.currentIndent;
			result.value = parseValueLines(state);
			--state.currentIndent;
			state.log("Value_End", match[1]);
		}

		return result;
	}

	function tryParseObject(state) {
		var re = new RegExp("^\\t{" + state.currentIndent + "}@(.*)");
		var match = state.currentLine().match(re);
		var result;
		if (match) {
			state.skipLine("Object_Begin", match[1]);
			result = { key: match[1] }
			++state.currentIndent;
			state.objectStack.push({});
			parseMembers(state);
			result.value = state.currentObject();
			state.objectStack.pop();
			--state.currentIndent;
			state.log("Object_End", match[1]);
		}

		return result;
	}

	function parseMembers(state) {
		var stop = false;
		while(!stop && !state.eos()) {
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

			var match = state.currentLine().match(reCommentLine); 
			if (match) {
				state.skipLine("Comment", match[1].length + "," + match[2]);
				continue;
			}

			match = state.currentLine().match(reEmptyLine);
			if (match) {
				state.skipLine("Empty", match[1]);
				continue;
			}		

			stop = true;
		}
	}

	exports.ParseState = function(text) {
		this.lines = text.split("\n");
		this.index = 0;
		this.currentIndent = 0;	
		this.objectStack = [{}];
		this.actionLog = null;
		this.enableLogging = function() {
			this.actionLog = [];
		}
		this.log = function(action, info) {
			if (action && this.actionLog) {
				this.actionLog.push(action + ":" + (info || ""));
			}
		};
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
		this.skipLine = function(action, logtext) {
			this.log(action, logtext);
			++this.index;
		};
		this.eos = function() {
			return this.index >= this.lines.length;	
		};	
	}

	exports.parse = function(arg) {
		var state = arg instanceof this.ParseState ? arg : new this.ParseState(arg);
		parsePreprocessors(state);
		parseMembers(state);
		return state.currentObject();
	}

	////////////////////////////////////////////////

	function SerializationState() {
		this.indent = "";
		this.result = [];
		this.push = function(str) {
			this.result.push(this.indent + str);
		};
		this.incIndent = function() {
			this.indent += "\t";
		};
		this.decIndent = function() {
			this.indent = this.indent.substring(0, this.indent.length-1);
		};
	}

	function _serializeMembers(state, object) {
		var keys = [];
		for (var key in object) {
			keys.push(key);
		}
		keys.sort();

		for (var i=0; i<keys.length; ++i) { 
			var key = keys[i];
			var value = object[key];
			if (isOfArrayType(value)) {
				var len = value.length;
				for (var j = 0; j < len; ++j) {
					var symbol = typeof value[j] === 'object' ? '@' : '='; 
					state.push(symbol + key);
					state.incIndent();
					_serialize(state, value[j]);
					state.decIndent();				
				}
			}
			else {
				var symbol = typeof value === 'object' ? '@' : '='; 
				state.push(symbol + key);
				state.incIndent();
				_serialize(state, value);
				state.decIndent();				
			}
		}
	}

	function _serialize(state, object) {
		if (object == null) {
			state.push('null');
			return;
		}
		
		switch(typeof object) {
			case 'string':
				var lines = object.split('\n');
				var len = lines.length;
				for (var i = 0; i < len; ++i) {
					state.push(lines[i]);
				}
				break;
			case 'object':			
				_serializeMembers(state, object);				
				break;
			default:
				state.push(String(object));
				break;
		}
	}

	exports.serialize = function (object) {
		var state = new SerializationState();
		_serializeMembers(state, object);
		return state.result.join('\n');
	}

})(this.hron = {});
