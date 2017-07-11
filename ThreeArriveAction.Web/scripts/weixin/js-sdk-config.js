
$(function () {
    $.ajax({
        type: "post",
        data: {
            openid: localStorage.getItem("openId"),
            type: "getJsapiTicket",
            url: location.href
        },
        url: "../Ajax/weixinInfo.ashx",
        success: function (d) {
            var obj = JSON.parse(d);
            console.log(obj);
            wx.config({
                appId: obj.appId,
                timestamp: obj.timestamp,
                nonceStr: obj.nonceStr,
                signature: obj.signature,
                jsApiList: obj.jsApiList,
            });
        },
        error: function (d) {
            console.log(d);
        }
    })
});