/*
<!doctype html> -> Esta etiqueta debe de estar en la página, 
                   donde se define el document type. De
				   lo contraio aparecera un warning de 
				   Active-X
*/

function cybs_dfprofiler(merchantID, environment, redirectUrl) {

    var org_id = '';
    var sessionID = new Date().getTime();

    var strBackground = '';
    var strPng = '';
    var strJs = '';
    var strSwf = '';

    if (environment == 'live') {
        org_id = 'k8vif92e';
        strBackground = "url(" + redirectUrl + "FingerprintRedirectPng1?orgId=" + org_id + "&sessionId=" + merchantID + sessionID + ")";
        strPng = redirectUrl + "FingerprintRedirectPng2?orgId=" + org_id + "&sessionId=" + merchantID + sessionID;
        strJs = redirectUrl + "FingerprintRedirectJs?orgId=" + org_id + "&sessionId=" + merchantID + sessionID;
        strSwf = redirectUrl + "FingerprintRedirectSwf?orgId=" + org_id + "&sessionId=" + merchantID + sessionID;
    } else {
        org_id = '1snn5n9w';
        strBackground = "url(https://h.online-metrix.net/fp/clear.png?org_id=" + org_id + "&session_id=" + merchantID + sessionID + "&m=1)";
        strPng = "https://h.online-metrix.net/fp/clear.png?org_id=" + org_id + "&session_id=" + merchantID + sessionID + "&m=2";
        strJs = "https://h.online-metrix.net/fp/check.js?org_id=" + org_id + "&session_id=" + merchantID + sessionID;
        strSwf = "https://h.online-metrix.net/fp/fp.swf?org_id=" + org_id + "&session_id=" + merchantID + sessionID;
    }

    var paragraphTM = document.createElement("p");
    paragraphTM.style.background = strBackground;
    paragraphTM.height = "0";
    paragraphTM.width = "0";
    paragraphTM.hidden = "true";
    document.body.appendChild(paragraphTM);

    var img = document.createElement("img");
    img.src = strPng;
    document.body.appendChild(img);

    var tmscript = document.createElement("script");
    tmscript.src = strJs;
    tmscript.type = "text/javascript";
    document.body.appendChild(tmscript);

    var objectTM = document.createElement("object");
    objectTM.type = "application/x-shockwave-flash";
    /*objectTM.classid = "clsid:D27CDB6E-AE6D-11cf-96B8-444553540000";*/
    objectTM.data = strSwf;
    objectTM.width = "1";
    objectTM.height = "1";
    objectTM.id = "thm_fp";
    var param = document.createElement("param");
    param.name = "movie";
    param.value = strSwf;
    objectTM.appendChild(param);
    document.body.appendChild(objectTM);

    return sessionID;
}