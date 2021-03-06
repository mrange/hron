/*!
 * HRON v0.8.0
 * A Javascript module for parsing and serializing HRON
 * https://github.com/mrange/hron
 * Microsoft Public License (Ms-PL)
 * by Daniel Brännström
 */

(function(exports) {

	'use strict';

	////////////////////////////////////////////////////////////////////
	// Utilities
	////////////////////////////////////////////////////////////////////

	function isOfArrayType(o) {
		return Object.prototype.toString.call(o) === '[object Array]';
	}

	// ref: http://gomakethings.com/vanilla-javascript-version-of-jquery-extend/
	function extend (defaults, options) {
	    var extended = {};
	    var prop;
	    for (prop in defaults) {
	        if (Object.prototype.hasOwnProperty.call(defaults, prop)) {
	            extended[prop] = defaults[prop];
	        }
	    }
	    for (prop in options) {
	        if (Object.prototype.hasOwnProperty.call(options, prop)) {
	            extended[prop] = options[prop];
	        }
	    }
	    return extended;
	}

	////////////////////////////////////////////////////////////////////
	// Deserialization (parsing)
	////////////////////////////////////////////////////////////////////

	var reCommentLine = new RegExp("^(\\s*)#(.*)");
	var reEmptyLine = new RegExp("^(\\s*?)\\r?$");
	var rePreprocessorLine = new RegExp("^!(.*)");

	function RegExCache(regexFactory) {
		var cacheSize = 10;
		var cache = [];
		for(var indent = 0; indent < cacheSize; ++indent) {
			cache.push(regexFactory(indent));
		}
		this.get = function(indent) {
			return indent < cacheSize ? cache[indent] : regexFactory(indent);
		};
	}

	var reNonEmptyLineCache = new RegExCache(function(indent) {
		return new RegExp("^\\t{" + indent + "}(.*)");
	});

	var reValueDefCache = new RegExCache(function(indent) {
		return new RegExp("^\\t{" + indent + "}=(.*)");
	});

	var reObjectDefCache = new RegExCache(function(indent) {
		return new RegExp("^\\t{" + indent + "}@(.*)");
	});

	function deserializePreprocessors(state) {
		/* jshint -W084 */
		var match;
		while(match = state.currentLine().match(rePreprocessorLine)) {
			state.skipLine("PreProcessor", match[1]);
		}
		/* jshint +W084 */
	}

	function deserializeValueLines(state) {
		var reNonEmptyLine = reNonEmptyLineCache.get(state.currentIndent);
		var match;
		var stop = false;
		var result = [];
		while(!stop && !state.eos()) {
			match = state.currentLine().match(reNonEmptyLine); 
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

	function tryDeserializeValue(state) {
		var re = reValueDefCache.get(state.currentIndent);
		var match = state.currentLine().match(re);
		var result;
		if (match) {
			state.skipLine("Value_Begin", match[1]);
			result = { key: match[1] };
			++state.currentIndent;
			result.value = deserializeValueLines(state);
			--state.currentIndent;
			state.logAction("Value_End", match[1]);
		}

		return result;
	}

	function tryDeserializeObject(state) {
		var re = reObjectDefCache.get(state.currentIndent);
		var match = state.currentLine().match(re);
		var result;
		if (match) {
			state.skipLine("Object_Begin", match[1]);
			result = { key: match[1] };
			++state.currentIndent;
			state.objectStack.push({});
			deserializeMembers(state);
			result.value = state.currentObject();
			state.objectStack.pop();
			--state.currentIndent;
			state.logAction("Object_End", match[1]);
		}

		return result;
	}

	function addPropertyToCurrentObject(state, name, value) {
		var o = state.currentObject();
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

	function deserializeMembers(state) {
		var stop = false;
		while(!stop && !state.eos()) {
			var value = tryDeserializeValue(state);
			if (value) {
				addPropertyToCurrentObject(state, value.key, value.value);
				continue;
			}		

			var object = tryDeserializeObject(state);
			if (object) {
				addPropertyToCurrentObject(state, object.key, object.value);
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

	function DeserializationState(text, options) {
		var defaults = { actionLog : null };
		var settings = extend(defaults, options);
		var lines = text.replace(/\n$/,"").split("\n"); // use replace to avoid empty line at the end
		var lineIndex = 0;
		this.currentIndent = 0;	
		this.objectStack = [{}];
		this.logAction = function(action, info) {
			if (action && settings.actionLog) {
				settings.actionLog.push(action + ":" + (info || ""));
			}
		};
		this.currentObject = function() {
			return this.objectStack[this.objectStack.length-1];
		};
		this.currentLine = function() {
			return lines[lineIndex];
		};
		this.skipLine = function(action, logtext) {
			this.logAction(action, logtext);
			++lineIndex;
		};
		this.eos = function() {
			return lineIndex >= lines.length;	
		};	
	}

	function deserialize(text, options) {
		var state = new DeserializationState(text, options);
		deserializePreprocessors(state);
		deserializeMembers(state);
		return state.currentObject();
	}

	////////////////////////////////////////////////////////////////////
	// Serialization
	////////////////////////////////////////////////////////////////////

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

	function serializeMembers(state, object) {
		var keys = [];
		var key;
		for (key in object) {
			keys.push(key);
		}
		keys.sort();

		var len = keys.length;
		for (var i=0; i<len; ++i) { 
			key = keys[i];
			var value = object[key];
			if (isOfArrayType(value)) {
				var len_ = value.length;
				for (var j = 0; j < len_; ++j) {
					serializeInstance(state, key, value[j]);
				}
			}
			else {
				serializeInstance(state, key, value);
			}
		}
	}

	function serializeInstance(state, name, instance) {		
		var symbol = typeof instance === 'object' ? '@' : '='; 
		state.push(symbol + name);
		state.incIndent();
		
		switch(typeof instance) {
			case 'string':
				var lines = instance.split('\n');
				var len = lines.length;
				for (var i = 0; i < len; ++i) {
					state.push(lines[i]);
				}
				break;
			case 'object':			
				serializeMembers(state, instance);				
				break;
			default:
				if (instance) {
					state.push(String(instance));
				}
				break;
		}
		
		state.decIndent();				
	}

	function serialize(object) {
		var state = new SerializationState();
		serializeMembers(state, object);
		return state.result.join('\n');
	}


	////////////////////////////////////////////////////////////////////
	// Exports
	////////////////////////////////////////////////////////////////////

	exports.parse = deserialize;
	exports.serialize = serialize;

})(typeof exports === 'undefined'? this.hron = {} : exports);
