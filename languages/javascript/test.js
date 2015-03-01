function downloadFile(file, ondone) {

	xmlhttp = new XMLHttpRequest();
	xmlhttp.open("GET","testdata/" + file, false);  // TODO: fix problem with async load
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
	function addTestResult(description, result, elapsedTime) {
		var ul = document.getElementById("testresults");
		var li = document.createElement("li");
		var t = document.createTextNode(description + " - " + elapsedTime + " ms - " + (result ? "OK" : "Fail"));
		li.appendChild(t);
		ul.appendChild(li);
	}

	runMany(downloadFile, [ identity + ".hron", identity + ".hron.actionlog"], function(files) {
		var state = new hron.ParseState(files[0]);
		state.enableLogging(); 
		var startTime = new Date().getTime();
		hron.parse(state);
		var endTime = new Date().getTime();		

		var actionLogRef = files[1].trim();
		var actionLog = state.actionLog.join("\r\n");
		var success = actionLogRef === actionLog;
		if (!success) {
			document.getElementById("test").innerHTML = identity;
			document.getElementById("logarea").value = actionLog;
			document.getElementById("logarea2").value = actionLogRef;
		}

		addTestResult(identity, success, endTime - startTime);
	});
}


function runTests() {
	tests = [
		"helloworld",
		"random",
		"simple",
		"large", 
	];

	runMany(runSingleTest, tests);
}
