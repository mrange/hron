####################################################################
# hron parsing and serialization library
# 
# Daniel Brännström, 2015
####################################################################

import re
import collections

####################################################################
# parsing (deserialization)
####################################################################

class _RegexCache:
    def __init__(self, functor):
        self.functor = functor
        self.cache = [ re.compile(functor(i)) for i in range(0,10) ]
    def get(self, index):
        if index < len(self.cache):
            return self.cache[index]
        else:
            return re.compile(self.functor(index))

class _Patterns:
    preprocessor = re.compile("^!(.*)")
    commentLine = re.compile("^(\\s*)#(.*)")
    emptyLine = re.compile("^(\\s*?)\\r?$")
    nonEmptyLineCache = _RegexCache(lambda indent: "^\\t{" + str(indent) + "}(.*)")
    valueDefCache = _RegexCache(lambda indent: "^\\t{" + str(indent) + "}=(.*)")
    objectDefCache = _RegexCache(lambda indent: "^\\t{" + str(indent) + "}@(.*)")

class _Dynamic:
    pass;

class _DeserializationState:
    def __init__(self, text):
        self.lines = text
        self.currentIndent = 0
        self.actionLog = None
        self.objectStack = [_Dynamic()]
        self.index = 0
    @property
    def currentLine(self):
        return self.lines[self.index]
    @property
    def currentObject(self):
        return self.objectStack[-1]
    @property
    def eos(self):
        return self.index >= len(self.lines)
    def log(self, action, info):
        if action and self.actionLog != None:
            self.actionLog.append(action + ":" + info)
    def skipLine(self, action, info):
        self.log(action, info)
        self.index += 1
    def enableLogging(self):
        self.actionLog = []

def _deserializePreprocessors(state):
    match = _Patterns.preprocessor.search(state.currentLine)
    while match: 
        state.skipLine("PreProcessor", match.group(1))
        match = _Patterns.preprocessor.search(state.currentLine)

def _deserializeValueLines(state):
    reNonEmptyLine = _Patterns.nonEmptyLineCache.get(state.currentIndent)
    stop = False
    result = []
    while(not(stop) and not(state.eos)):
        match = reNonEmptyLine.search(state.currentLine)
        if match:
            result.append(match.group(1))
            state.skipLine("ContentLine", match.group(1))
            continue

        match = _Patterns.commentLine.search(state.currentLine)
        if match: 
            state.skipLine("CommentLine", str(len(match.group(1))) + "," + match.group(2))
            continue

        match = _Patterns.emptyLine.search(state.currentLine)
        if match:
            state.skipLine("EmptyLine", match.group(1))
            continue
        
        stop = True    

    return "\n".join(result)

def _tryDeserializeValue(state):
    re = _Patterns.valueDefCache.get(state.currentIndent)
    match = re.search(state.currentLine)
    result = None
    if match:
        state.skipLine("Value_Begin", match.group(1))
        key = match.group(1)
        state.currentIndent += 1
        value = _deserializeValueLines(state)
        state.currentIndent -= 1
        state.log("Value_End", match.group(1))
        result = (key, value)    

    return result

def _tryDeserializeObject(state):
    re = _Patterns.objectDefCache.get(state.currentIndent)
    match = re.search(state.currentLine)
    result = None
    if match:
        state.skipLine("Object_Begin", match.group(1))
        key = match.group(1)
        state.currentIndent += 1
        state.objectStack.append(_Dynamic())
        _deserializeMembers(state)
        value = state.objectStack.pop();
        state.currentIndent -= 1
        state.log("Object_End", match.group(1))
        result = (key, value)

    return result

def _addPropertyToCurrentObject(state, name, value):
    o = state.currentObject
    if hasattr(o, name):
        a = getattr(o, name)
        if type(a) is list:
            a.append(value)
        else:
            setattr(o, name, [a, value])
    else:
        setattr(o, name, value)

def _deserializeMembers(state):
    stop = False
    while(not(stop) and not(state.eos)):
        value = _tryDeserializeValue(state)
        if value:
            _addPropertyToCurrentObject(state, value[0], value[1])
            continue

        object = _tryDeserializeObject(state)
        if object:
            _addPropertyToCurrentObject(state, object[0], object[1])
            continue

        match = _Patterns.commentLine.search(state.currentLine)
        if match:
            state.skipLine("Comment", str(len(match.group(1))) + "," + str(match.group(2)))
            continue

        match = _Patterns.emptyLine.search(state.currentLine)
        if match:
            state.skipLine("Empty", match.group(1))
            continue

        stop = True            

def _deserialize(arg):
    state = arg if isinstance(arg, _DeserializationState) else _DeserializationState(arg)
    _deserializePreprocessors(state)
    _deserializeMembers(state)
    return state.currentObject

def parse(arg):
    return _deserialize(arg)

####################################################################
# serialization
####################################################################

class _SerializationState:
    def __init__(self):
        self.indent = ""
        self.result = []

    def push(self, str):
        self.result.append(self.indent + str);
    
    def incIndent(self):
        self.indent += "\t";
    
    def decIndent(self):
        self.indent = self.indent[0:-1];
    
def _getKeyValues(object):
    if isinstance(object, dict):
        return list(object.items())
    else:
        return [(a,getattr(object,a)) for a in dir(object) if not a.startswith('__') and not callable(getattr(object,a))]

def _serializeMembers(state, object):
    keyValues = _getKeyValues(object)
    keyValues.sort(key=lambda kv: kv[0])

    for key, value in keyValues:
        if isinstance(value, collections.Iterable) and not(isinstance(value, (str,dict))):
            for elem in iter(value):
                _serializeInstance(state, key, elem)
        else:
            _serializeInstance(state, key, value)

def _serializeInstance(state, name, instance):
    symbol = '=' if isinstance(instance, (str, int, float)) else '@' 
    state.push(symbol + name)
    state.incIndent()

    if isinstance(instance, str):
        for line in instance.split("\n"):
            state.push(line);
    elif isinstance(instance, (int, float)): 
        state.push(str(instance));                
    else:   
        _serializeMembers(state, instance);        
        
    state.decIndent();                

def serialize(object):
    state = _SerializationState();
    _serializeMembers(state, object);
    return "\n".join(state.result);

