
function submit(callback) {
    if (typeof (vm.needglocalId) != "undefined" && vm.localIds.length <= 0) {
        alert("请上传整体照片");
        return;
    }
    if ((vm.needlocalIds !== false) && vm.localIds.length <= 0) {
        alert("请上传照片");
        return;
    }

    var paths = [];
    vm.localIds.forEach((localId) => {
        window.wx.uploadImage({
            localId: localId.toString(),
            isShowProgressTips: 1,
            success: function (res) {
                $.ajax({
                    url: "../Ajax/weixinInfo.ashx",
                    type: "post",
                    data: {
                        type: "downFile",
                        mediaId: res.serverId,
                        openId: localStorage.getItem("openId")
                    },
                    success: function (d) {
                        paths.push(d);
                        if (paths.length === vm.localIds.length) {
                            if (vm.glocalId && vm.glocalId.length > 0) {
                                window.wx.uploadImage({
                                    localId: vm.glocalId[0].toString(),
                                    isShowProgressTips: 1,
                                    success: function (gres) {
                                        $.ajax({
                                            url: "../Ajax/weixinInfo.ashx",
                                            type: "post",
                                            data: {
                                                type: "downFile",
                                                mediaId: gres.serverId,
                                                openId: localStorage.getItem("openId")
                                            },
                                            success: function (gd) {
                                                paths.push(gd);
                                                window.wx.getLocation({
                                                    type: 'gcj02',
                                                    success: function (res) {
                                                        var gcj02tobd09 = coordtransform.gcj02tobd09(res.longitude, res.latitude);
                                                        callback(paths, gcj02tobd09[1], gcj02tobd09[0]);
                                                    }, error: function () {
                                                        callback(paths, 0, 0);
                                                    }
                                                });
                                            },
                                            error: function (gd) {
                                                console.log(gd);
                                            }
                                        });
                                    }
                                });
                            } else {
                                window.wx.getLocation({
                                    type: 'gcj02',
                                    success: function (res) {
                                        var gcj02tobd09 = coordtransform.gcj02tobd09(res.longitude, res.latitude);
                                        callback(paths, gcj02tobd09[1], gcj02tobd09[0]);
                                    }, error: function () {
                                        callback(paths, 0, 0);
                                    }
                                });
                            }
                        }
                    }, error: function (d) {
                        console.log(d);
                    }
                });
            }
        });
    });
    if (vm.needlocalIds === false && vm.localIds.length <= 0) {
        callback([], 0, 0);
    }
};

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
            window.wx.config({
                appId: obj.appId,
                timestamp: obj.timestamp,
                nonceStr: obj.nonceStr,
                signature: obj.signature,
                jsApiList: obj.jsApiList
            });
        },
        error: function (d) {
            console.log(d);
        }
    });
});

