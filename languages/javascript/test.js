/*
function downloadFile(file, ondone) {

	xmlhttp = new XMLHttpRequest();
	xmlhttp.open("GET","testdata/" + file,false);
	xmlhttp.onreadystatechange = function() {
  		if (xmlhttp.readyState==4 && xmlhttp.status==200)
    		ondone(xmlhttp.responseText);
    	else
    		ondone(null);
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

function runSingleTest(identity) {
	function addTestResult(description, result) {
		var x = document.getElementById("testresults");
		var t = document.createTextNode(description + " - " + (result ? "OK" : "<b>Fail</b>"));
		x.appendChild(t);
	}

	runMany(downloadFile, [ identity + ".hron", identity + ".hron.actionlog"], function(files) {
		var state = new hron.ParseState(testdata[0].text);
		state.enableLogging(); 
		hron.parse(state);
		
		var actionLogRef = testdata[0].actionLog.trim();
		var actionLog = state.actionLog.join("\r\n");
		addTestResult("helloworld", actionLogRef === actionLog);
	});
}

function runTests() {
	tests = [
		"helloworld",
		"random",
		"simple"
	];

	runMany(runSingleTest, tests);
}
function xxx(dsd) {

}
*/

function downloadFile(file, ondone) {

	xmlhttp = new XMLHttpRequest();
	xmlhttp.open("GET","testdata/" + file,false);
	xmlhttp.onreadystatechange = function() {
  		if (xmlhttp.readyState==4 && xmlhttp.status==200)
    		ondone(xmlhttp.responseText);
    	else
    		ondone(null);
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

function runSingleTest(identity) {
	function addTestResult(description, result) {
		var x = document.getElementById("testresults");
		var t = document.createTextNode(description + " - " + (result ? "OK" : "Fail"));
		x.appendChild(t);		
	}

	runMany(downloadFile, [ identity + ".hron", identity + ".hron.actionlog"], function(files) {
		var state = new hron.ParseState(files[0]);
		state.enableLogging(); 
		hron.parse(state);
		
		var actionLogRef = files[1].trim();
		var actionLog = state.actionLog.join("\r\n");
		addTestResult(identity, actionLogRef === actionLog);
	});
}


function runTests() {
	tests = [
		"helloworld",
		"random",
		"simple"
	];

	runMany(runSingleTest, tests);
}
