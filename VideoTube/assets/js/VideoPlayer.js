function getQueryVar() {
    var queryStr = unescape(window.location.search) + '&';
    var regex = new RegExp('.*?[&\\?]' + arguments[0] + '=(.*?)&.*');
    var val = queryStr.replace(regex, "$1");
    return val == queryStr ? false : val;
}
(function () {
    var video = document.querySelector('.videoPlayer');
    let id = getQueryVar('id');
    if (id === false)
        return;
    $.post("ajax/getVData", { videoId: parseInt(id) })
        .done(invokeVideo);
    function invokeVideo() {
        if (arguments[0] == null || arguments[0] == "")
            return;
        var assetURL = arguments[0]//'uploads/videos/frag_bunny.mp4';
        var mimeCodec = 'video/mp4; codecs="avc1.42E01E, mp4a.40.2"';

        if ('MediaSource' in window && MediaSource.isTypeSupported(mimeCodec)) {
            var mediaSource = new MediaSource();
            //console.log(mediaSource.readyState); // closed
            video.src = URL.createObjectURL(mediaSource);
            mediaSource.addEventListener('sourceopen', sourceOpen);
        } else {
            console.error('Unsupported MIME type or codec: ', mimeCodec);
        }

        function sourceOpen(_) {
            URL.revokeObjectURL(video.src)
            //console.log(this.readyState); // open
            var mediaSource = this;
            var sourceBuffer = mediaSource.addSourceBuffer(mimeCodec);

            (async function () {
                let buf = (await fetch(assetURL).then(a => a.arrayBuffer()))
                sourceBuffer.addEventListener('updateend', updateVideo);
                function updateVideo(_) {
                    if (!sourceBuffer.updating && mediaSource.readyState == 'open') {
                        mediaSource.endOfStream();
                        video.play();
                    }
                    console.log(mediaSource.readyState); // ended
                }
                sourceBuffer.appendBuffer(buf);
            })();
        };
    }
})();

