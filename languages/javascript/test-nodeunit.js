// hron.js unit tests
// nodeunit is used as a testing framework, https://github.com/caolan/nodeunit
// tests are run automatically as a grunt task, https://github.com/gruntjs/grunt-contrib-nodeunit
//   >grunt nodeunit
// to run tests manually: 
//   >nodeunit test-nodeunit.js

var fs = require("fs");
var hron = require("./hron.js");

function getTestData(testid) {

	function readTextFile(file) {
		var data = fs.readFileSync(file, "utf8");
		data = data.replace(/^\uFEFF/, '')
		return data;	
	}

	var root = "../../reference-data/";
	var hronTextFile = root + testid + ".hron";
	var hronActionLogFile = root + testid + ".hron.actionlog";

	var hronText = readTextFile(hronTextFile);
	var hronActionLogRef = readTextFile(hronActionLogFile).trim();

	return {
		id: testid,
		text: hronText,
		actionLog: hronActionLogRef
	};	
}

var testdata = { };
testdata.helloworld = getTestData("helloworld");
testdata.simple = getTestData("simple");
testdata.random = getTestData("random");
testdata.large = getTestData("large");

function run(test, testdata) {
	var state = new hron.ParseState(testdata.text);
	state.enableLogging(); 
	hron.parse(state);

	var actionLog = state.actionLog.join("\r\n");
	test.equal(actionLog, testdata.actionLog);
}

exports.testHelloWorld = function(test) {
	run(test, testdata.helloworld);
	test.done();
}

exports.testSimple = function(test) {
	run(test, testdata.simple);
	test.done();
}

exports.testRandom = function(test) {
	run(test, testdata.random);
	test.done();
}

exports.testLarge = function(test) {
	run(test, testdata.large);
	test.done();
}

exports.testSerialization = function(test) {
	var o = {
		intVal: 10,
		floatVal: 59.2,
		stringVal: 'apa\nbepa\ncepa',
		listVal: [6,5.22,'abc', { x:'x1',y:'y1'}],
		objectVal: {
			a: 1,
			b: 2,
			c: {
				d: '3'
			}
		}
	};

	var serializedRef = 
		"=floatVal\n" 	+
		"\t59.2\n"		+
		"=intVal\n" 	+
		"\t10\n"		+
		"=listVal\n" 	+
		"\t6\n" 		+
		"=listVal\n" 	+
		"\t5.22\n" 		+
		"=listVal\n" 	+
		"\tabc\n" 		+	
		"@listVal\n" 	+
		"\t=x\n" 		+	
		"\t\tx1\n" 		+	
		"\t=y\n" 		+	
		"\t\ty1\n" 		+	
		"@objectVal\n" 	+
		"\t=a\n" 		+
		"\t\t1\n" 		+
		"\t=b\n" 		+
		"\t\t2\n" 		+
		"\t@c\n" 		+
		"\t\t=d\n" 		+
		"\t\t\t3\n" 	+
		"=stringVal\n"	+
		"\tapa\n"		+
		"\tbepa\n"		+
		"\tcepa";

	var serialized = hron.serialize(o);
	test.equal(serialized, serializedRef);
	test.done();
}
