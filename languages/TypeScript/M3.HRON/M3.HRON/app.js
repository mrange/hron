window.onload = function () {
    var btn = document.getElementById("clickme");
    btn.onclick = function () {
        var hron = document.getElementById("hron").innerText;

        var result = HRON.parseHron(hron);

        var asString = HRON.toString(result.doc);
        var asObject = HRON.toObject(result.doc);

        alert(asString);
    };
};
//# sourceMappingURL=app.js.map
