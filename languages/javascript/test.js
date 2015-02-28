function downloadTestFile(file) {
	xmlhttp = new XMLHttpRequest();
	xmlhttp.open("GET","testdata/" + file,false);
	xmlhttp.send();
	return xmlhttp.responseText;
}

function downloadTestData(identity) {
	result = {}
	result.text = downloadTestFile(identity + ".hron")
	result.actionLog = downloadTestFile(identity + ".hron.actionlog")
	return result
}

function runTests() {
	testdataFiles = [
		"helloworld",
		"random",
		"simple"
	];

	testdata = []

	testdataFiles.forEach(function(file) {
		item = downloadTestData(file)
		testdata.push(item)	
	})

	var state = new hron.ParseState(testdata[0].text);
	state.enableLogging(); 
	hron.parse(state);
	
	var actionLogRef = testdata[0].actionLog.trim();
	var actionLog = state.actionLog.join("\r\n");
	if (actionLogRef === actionLog) {
		console.log("success");
	}	
	else {
		console.log("fail");	
	}
}
