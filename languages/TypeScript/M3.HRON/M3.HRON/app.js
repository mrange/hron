window.onload = function () {
    var btn = document.getElementById("clickme");
    btn.onclick = function () {
        var hron = document.getElementById("hron").innerText;

        var result = HRON.parseHron(hron);

        var x = result.doc;

        alert(x);
    };
};
//# sourceMappingURL=app.js.map
