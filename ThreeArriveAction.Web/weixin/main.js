
require.config({
    baseUrl: "/scripts/",
    waitSeconds: 30,
    map: {
        '*': {
            'css': 'require/css'
        }
    },
});

require(['/scripts/jquery/jquery-1.10.2.min.js',
    '/scripts/sweetalert/sweetalert.min.js',
    'css!/scripts/sweetalert/sweetalert.css', ],
    function () {
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
                        openid: req["openid"]
                    },
                    url: "../Ajax/sys_UsersManager.ashx?type=checkOpenId",
                    success: function (d) {
                        if (d == 0) {
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
                                signIn(inputValue, req["openid"]);
                            });
                        }
                    },
                    error: function (d) {
                        console.log(d);
                    }
                })
            }
        }

        function signIn(phone，openId) {
            $.ajax({
                type: "post",
                data: {
                    phone: phone,
                    openId: openId
                },
                url: "../Ajax/sys_UsersManager.ashx?type=signInPhone",
                success: function (d) {
                    var obj = JSON.parse(d);
                    console.log(obj);
                },
                error: function (d) {
                    console.log(d);
                }
            })
        }
    });

