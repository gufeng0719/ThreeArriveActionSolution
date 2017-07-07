
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
var req = GetRequest();

require.config({
    baseUrl: "/scripts/",
    waitSeconds: 30,
    map: {
        '*': {
            'css': 'require/css'
        }
    },
});

wx.config({
    debug: true, 
    appId: 'wx7c32c023241aba89',
    timestamp: new Date().getTime(), 
    nonceStr: Math.random(), // 必填，生成签名的随机串
    signature: '',// 必填，签名，见附录1
    jsApiList: [] // 必填，需要使用的JS接口列表，所有JS接口列表见附录2
});

require(['/scripts/weixin/jweixin-1.2.0.js',
    '/scripts/sweetalert/sweetalert.min.js',
    'css!/scripts/sweetalert/sweetalert.css'],
    function () {
        $(function () {
            if (!localStorage) return;
            if (localStorage.getItem("current_user") != null
                && JSON.parse(localStorage.getItem("current_user")).UserRemark == localStorage.getItem("openId")) return;
            signVerification();
        });

        function signVerification() {
            $.ajax({
                type: "post",
                data: {
                    openid: localStorage.getItem("openId")
                },
                url: "../Ajax/sys_UsersManager.ashx?type=checkOpenId",
                success: function (d) {
                    var obj = JSON.parse(d);
                    if (obj.code == 0) {
                        swal({
                            title: "请输入您的手机号码",
                            text: " ",
                            type: "input",
                            showCancelButton: false,
                            closeOnConfirm: false,
                            animation: "slide-from-top",
                            inputPlaceholder: "手机号码"
                        }, function (inputValue) {
                            if (inputValue.length !== 11) {
                                swal.showInputError("请输入11位的手机号码");
                                return false
                            }
                            signIn(inputValue, localStorage.getItem("openId"));
                        });
                    } else {
                        if (localStorage) {
                            localStorage.setItem("current_user", JSON.stringify(obj.model));
                        }
                    }
                },
                error: function (d) {
                    console.log(d);
                }
            })
        }

        function signIn(phone, openId) {
            $.ajax({
                type: "post",
                data: {
                    phone: phone,
                    openId: openId
                },
                url: "../Ajax/sys_UsersManager.ashx?type=signInPhone",
                success: function (d) {
                    if (d == "0") {
                        alert("数据库中不存在的手机号码");
                    } else {
                        var obj = JSON.parse(d);
                        if (localStorage) {
                            localStorage.setItem("current_user", JSON.stringify(obj));
                        }
                        console.log(obj);
                        swal({
                            title: "注册成功!",
                            text: "",
                            timer: 800,
                            showConfirmButton: false
                        });
                    }
                },
                error: function (d) {
                    console.log(d);
                }
            })
        }
    });

