
$(function () {
    $.ajax({
        type: "post",
        data: {
            openid: localStorage.getItem("openId"),
            type: "getJsapiTicket"
        },
        url: "../Ajax/weixinInfo.ashx",
        success: function (d) {
            var obj = JSON.parse(d);
            wx.config({
                debug: true,
                appId: obj.appId,
                timestamp: d.timestamp,
                nonceStr: d.nonceStr,
                signature: d.signature,
                jsApiList: d.jsApiList
            });
        },
        error: function (d) {
            console.log(d);
        }
    })
});