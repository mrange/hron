
window.onload = () => {

    var btn = document.getElementById("clickme")
    btn.onclick = () => 
        {
            var hron = document.getElementById("hron").innerText;

            var result = HRON.parseHron(hron)

            var x : any = result.doc

            alert(x)
        }

};