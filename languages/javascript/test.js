function downloadFile(file, ondone) {
	var xmlhttp = new XMLHttpRequest();
	xmlhttp.open("GET","testdata/" + file);
	xmlhttp.onreadystatechange = function() {
  		if (xmlhttp.readyState==4) {
  			if (xmlhttp.status==200) {
				ondone(xmlhttp.responseText);
  			}	
  			else {
  				ondone(null);	
  			}
  		}     		    		
	}
	xmlhttp.send();
}

function runMany(functor, dataArray, ondone) {
	var counter = dataArray.length;
	if (counter == 0) { ondone(null); return; }
	var result = [];
	for(i=0; i<dataArray.length; ++i) {
		result.push(null);
	}
	dataArray.forEach(function(input, index) {
		functor(input, function(output) {
			--counter;
			if (output) 
				result[index] = output;			
			if (ondone && counter == 0) 
				ondone(result);
		});
	});
}

function addTestResult(description, result, elapsedTime) {
	var ul = document.getElementById("testresults");
	var li = document.createElement("li");
	var t = document.createTextNode(description + " - " + elapsedTime + " ms - " + (result ? "OK" : "Fail"));
	li.appendChild(t);
	ul.appendChild(li);
}

function addLog(identity, log, logRef) {
	var textAreaStyle = "width: 500px; height: 300px;";
	var ta1 = document.createElement("textarea");
	ta1.setAttribute("style", textAreaStyle);
	ta1.value = log;
	var ta2 = document.createElement("textarea");
	ta2.setAttribute("style", textAreaStyle);
	ta2.value = logRef;
	var logs = document.getElementById("logs");		
	logs.appendChild(document.createTextNode(identity + " - produced log"));		
	logs.appendChild(document.createElement("br"));
	logs.appendChild(ta1);
	logs.appendChild(document.createElement("br"));
	logs.appendChild(document.createTextNode(identity + " - expected log"));		
	logs.appendChild(document.createElement("br"));
	logs.appendChild(ta2);
	logs.appendChild(document.createElement("br"));
}

function runSingleTest(identity) {
	runMany(downloadFile, [ identity + ".hron", identity + ".hron.actionlog"], function(files) {
		var state = new hron.ParseState(files[0]);
		state.enableLogging(); 
		var startTime = new Date().getTime();
		hron.parse(state);
		var endTime = new Date().getTime();		

		var actionLogRef = files[1].trim();
		var actionLog = state.actionLog.join("\r\n");
		var success = actionLogRef === actionLog;

		addTestResult(identity, success, endTime - startTime);
		if (!success) {
			addLog(identity, actionLog, actionLogRef);			
		}
	});
}

function runDeserializationTests() {
	tests = [
		"helloworld",
		"random",
		"simple",
		"large", 
	];

	runMany(runSingleTest, tests);	
}

function runSerializationTests() {
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

	var startTime = new Date().getTime();
	var serialized = hron.serialize(o);
	var endTime = new Date().getTime();		
	var success = serialized === serializedRef;
	var identity = "serialization";
	addTestResult(identity, success, endTime - startTime);
	if (!success) {
		addLog(identity, serialized, serializedRef);			
	}
}

function runTests() {
	runDeserializationTests();
	runSerializationTests();
}
