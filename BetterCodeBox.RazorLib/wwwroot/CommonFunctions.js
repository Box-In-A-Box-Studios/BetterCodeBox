triggerFileInput = function (id) {
    if (id == null || id == '') {
        return;
    }
    console.log('triggerFileInput: ' + id);
    document.getElementById(id).click();
}

saveAsFile = function (filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

scrollToElement = function (id) {
    console.log('scrollToElement: ' + id);
    var element = document.getElementById(id);
    if (element == null) {
        return;
    }
    element.scrollIntoView({ behavior: "smooth" });
}