
window.onload = () => {

    var btn = document.getElementById("clickme")
    btn.onclick = () => 
        {
            var hron = document.getElementById("hron").innerText;

            var result = HRON.parseHron(hron)

            var asString = HRON.toString(result.doc)
            var asObject = HRON.toObject(result.doc)

            alert(asString)
        }

};