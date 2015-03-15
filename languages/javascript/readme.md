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
npm install hron.js
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
var hron = require('hron');
// Parsing
var object = hron.parse(hronString);
// Serializing
var hronString = hron.serialize(object);
```

License
-------
This library is released under Microsoft Public License (Ms-PL).
