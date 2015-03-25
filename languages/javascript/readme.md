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

Developer
-------------
Development has been done on windows and som traces of that is still visible. This can quite easily be addressed if needed.

Grunt is used to run the build tasks which currently include
* Static code analysis by jshint
* Minification (building artifact hron.min.js) by ugglify

```Batchfile
>grunt
```

Tests are run using a powershell script as illustraded below:

```Powershell
>.\run-tests.ps1
```
The test script will download mongoose (for windows) and launch it it the project root folder. This will trigger the default brower to launch and to request index.html which uses test.js as a testing framework and displays the result directly on the page. Test data are downloaded from the local server using XMLHttpRequests.

To publish a new nodejs package execute the line below. Remember to update version information first though otherwise npm will complain.

```Batchfile
>npm publish
```

Version information is currently duplicated in two places:
 - package.json (version tag)
 - hron.js (header)


Possible improvements
----------------------------
A few possible improvements from the top of my head:

 - Replace custom testing framework with nodejs script now that server side modules are supported by the library.
 - Support other package managers than npm such as bower
 - Remove dependency to windows for developer environment

License
-------
This library is released under Microsoft Public License (Ms-PL).
