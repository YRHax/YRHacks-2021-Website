async function brLoad(type, name, defaultUri) {
    const response = await fetch(defaultUri + '.br', {cache: 'no-cache'});
    if (!response.ok) {
        throw new Error(response.statusText);
    }
    const originalResponseBuffer = await response.arrayBuffer();
    const originalResponseArray = new Int8Array(originalResponseBuffer);
    const decompressedResponseArray = BrotliDecode(originalResponseArray);
    const contentType = type ===
    'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
    return new Response(decompressedResponseArray,
        {headers: {'content-type': contentType}});
}
function UrlExists(url)
{
    var http = new XMLHttpRequest();
    http.open('HEAD', url, false);
    http.send();
    return http.status!==404;
}
function load(type, name, defaultUri, integrity) {
    let st = new Date().getTime();
    if (type == "dotnetjs")
        return defaultUri;
    let f = null;
    if(!started){
        let bar = $("#l-bar");
        document.getElementById('l-bar')
            .style.width = '0%';
        document.getElementById('app-load')
            .innerHTML = 'Loading Dependencies...';
        bar.removeClass("indeterminate");
        bar.addClass("determinate");
    }
    started = true;
    if(useStaticBrotli || (testStaticBrotli && UrlExists(defaultUri + ".br"))){
        testStaticBrotli = false;
        useStaticBrotli = true;
        f = brLoad(type, name, defaultUri);
    }
    else{
        testStaticBrotli = false;
        f = fetch(defaultUri,
            {
                cache: 'no-cache',
                integrity: integrity
            });
    }
    resourceCount++;
    f.then(() => {
        loadedResources++;
        const value = Math.floor(100 * loadedResources / resourceCount);
        const pct = value + '%';
        document.getElementById('app-load')
            .innerHTML = 'Loading Dependencies — ' + pct;
        document.getElementById('l-bar')
            .style.width = pct;
        document.getElementById('app-load-status')
            .innerHTML = 'Loaded — ' + loadedResources + '/' + resourceCount + ' (' + pct + ') ' + name;
        if(loadedResources === resourceCount){
            ended = true;
            let time = new Date().getTime() - st;
            console.log("Finished Loading Resources. Took " + time + "ms.");
            document.getElementById('app-load-status').innerHTML = '';
            document.getElementById('app-load').innerHTML = 'Rendering...';
        }
    });
    return f;
}
let loadedResources = 0;
let resourceCount = 0;
let started = false;
let ended = false;
let testStaticBrotli = true;
let useStaticBrotli = false;
Blazor.start({
    loadBootResource: load
});