window.copyText = function (text) {
    navigator.clipboard.writeText(text)
        .then(() => console.log('Copied:', text))
        .catch(err => console.error('Clipboard write failed', err));
};