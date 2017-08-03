
function getRequest() {
    var url = location.search; //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") !== -1) {
        var str = url.substr(1);
        var strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
req = getRequest();

sendTemplateMsg = function (obj, callback) {
    $.ajax({
        type: "post",
        url: "../Ajax/weixinInfo.ashx",
        data: {
            type: "sendTemplateMsg",
            obj: obj
        },
        complete: function (d) {
            callback(d.responseText);
        }
    });
}


var scrolled = false;
// 下拉加载
window.onscroll = function() {
    event.stopPropagation()
    if (!scrolled && typeof (vm) !== "undefined") {
        var t = document.documentElement.scrollTop || document.body.scrollTop;
        var t1 = t + $(window).height();
        var h = document.body.scrollHeight;
        if (!scrolled) {
            //下拉加载
            if (t1 >= h - 30) {
                if (vm.getpage) {
                    scrolled = true;
                    vm.getpage(vm.page + 1);
                }
            }
        }
    }
}

