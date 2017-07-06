
$(function () {
    signVerification();
});

function GetRequest() {
    var url = location.search; //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}

function signVerification() {
    var cu = localStorage.getItem("current_user");
    var req = GetRequest();
    if (!cu) {
        $.ajax({
            type: "post",
            data: {
                type: "checkOpenId",
                openid: req["openid"]
            },
            url: "../Ajax/sys_UsersManager.ashx",
            success: function (d) {
                var obj = JSON.parse(d);
                if (obj.length <= 0) {
                    alert("需要登录");
                }
            },
            error: function (d) {
                console.log(d);
            }
        })
    }
}

function signIn() {
    $.ajax({
        type: "post",
        data: {

        },
        url: "todo",
        success: function (d) {
            var obj = JSON.parse(d);

        },
        error: function (d) {
            console.log(d);
        }
    })
}