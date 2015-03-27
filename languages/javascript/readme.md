HRON Javascript parser and serializer
=====================================
The hron.js javascript library provides functions for parsing and serializing HRON. The library can be used both as a client library within a browser and as a server library from node.js. 

Install
-------
A packaged source file includes everything you need to use hron.js.

* [hron.min.js](./hron.min.js) for production
* [hron.js](./hron.js) for development

If you prefer node package manager, run the following:

```Batchfile
>npm install hron.js
```

Using the library, client side
------------------------------
The following snippet is a minimal sample of the HRON library usage
```HTML
<script src="hron.js"></script>
<script>
// Parsing
var object = hron.parse(hronString);
// Serializing
var hronString = hron.serialize(object);
</script>
```

Using the library, server side
------------------------------
The following snippet is a minimal sample of the HRON library usage

```javascript
var hron = require('hron.js');
// Parsing
var object = hron.parse(hronString);
// Serializing
var hronString = hron.serialize(object);
```

Value interpretation
-------------------
The hron format handles string values only. Values can be converted to other types using the build in type converters in javascript. For example, give the following hron document

    =stringVal
        test
    =numericVal
        10
    =boolVal
        false

The following is valid

```javascript
var object = hron.parse(...);
assert.equal(object.stringVal, "test");
assert.equal(Number(object.numericVal), 10);
assert.equal(Boolean(object.boolVal), false);
```

Advanced
------------------
The hron.parse method also accepts an additional, optional argument for parse options as illustrated below.

```javascript
var actionLog = [];
var object = hron.parse(hronString, { actionLog: actionLog })
```

The following options are available:

| option           | default       | meaning                     |
| ---------------- | ------------- | --------------------------- |
| actionLog        | null          | this is for testing purposes only. the parameter can be set to a list object to retrieve internal log |

Developer
-------------
Development has been done on windows and som traces of that is still visible. This can quite easily be addressed if needed.

Grunt is used to run the build tasks which currently include
* Unit testing by nodeunit
* Static code analysis by jshint
* Minification (building artifact hron.min.js) by ugglify

```Batchfile
>grunt
```

To publish a new nodejs package execute the line below. Remember to update version information in package.json first though otherwise npm will complain.

```Batchfile
>npm publish
```

Possible improvements
----------------------------
 - Support other package managers than npm such as bower
 - Test both server side and client side

License
-------
This library is released under Microsoft Public License (Ms-PL).
