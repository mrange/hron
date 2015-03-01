function downloadFile(file, ondone) {
	var xmlhttp = new XMLHttpRequest();
	xmlhttp.open("GET","testdata/" + file);
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

	function addActionLog(identity, actionLog, actionLogRef) {
		var textAreaStyle = "width: 500px; height: 300px;";
		var ta1 = document.createElement("textarea");
		ta1.setAttribute("style", textAreaStyle);
		ta1.value = actionLog;
		var ta2 = document.createElement("textarea");
		ta2.setAttribute("style", textAreaStyle);
		ta2.value = actionLogRef;
		var actionLogs = document.getElementById("actionLogs");		
		actionLogs.appendChild(document.createTextNode(identity + " - actionlog"));		
		actionLogs.appendChild(document.createElement("br"));
		actionLogs.appendChild(ta1);
		actionLogs.appendChild(document.createElement("br"));
		actionLogs.appendChild(document.createTextNode(identity + " - actionlogRef"));		
		actionLogs.appendChild(document.createElement("br"));
		actionLogs.appendChild(ta2);
		actionLogs.appendChild(document.createElement("br"));
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

		addTestResult(identity, success, endTime - startTime);
		if (!success) {
			addActionLog(identity, actionLog, actionLogRef);			
		}
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
