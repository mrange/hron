function downloadFile(file, ondone) {

	xmlhttp = new XMLHttpRequest();
	xmlhttp.open("GET","testdata/" + file, false);
	console.log("request made for " + file);
	xmlhttp.onreadystatechange = function() {
		console.log("state changed for " + file + " to " + xmlhttp.readyState);
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
		var success = actionLogRef === actionLog;
		if (!success) {
			console.log("====================================");
			console.log(identity);
			console.log("====================================");			
			console.log("============ action log ============");
			console.log(actionLog);
			console.log("============ action log ============");
			console.log(actionLogRef);
		}

		addTestResult(identity, success);
	});
}


function runTests() {
	tests = [
		//"helloworld",
		"random",
		//"simple"
	];

	runMany(runSingleTest, tests);
}
