function downloadTestFile(file) {
	xmlhttp = new XMLHttpRequest();
	xmlhttp.open("GET","testdata/" + file,false);
	xmlhttp.send();
	return xmlhttp.responseText;
}

function downloadTestData(identity) {
	result = {}
	result.text = downloadTestFile(identity + ".hron")
	result.actions = downloadTestFile(identity + ".hron.actionlog")
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

	hron.parse(testdata[0].text);	
}
