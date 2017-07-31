//require.config({
//    baseUrl: "/scripts/",
//    waitSeconds: 30,
//    map: {
//        '*': {
//            'css': 'require/css'
//        }
//    }
//});

//require(['/scripts/sweetalert/sweetalert.min.js',
//        'css!/scripts/sweetalert/sweetalert.css'],
//function () {
$(function () {
    if (!localStorage.getItem("openId")) {
        window.open("login.html?page=" + encodeURIComponent(location.href), "_self");
    }
    if (!localStorage.getItem("current_user")) {
        signVerification();
    }
    var model = JSON.parse(localStorage.getItem("current_user"))
    if (!model || model.UserRemark !== localStorage.getItem("openId")) {
        signVerification();
    }
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
                        return false;
                    }
                    signIn(inputValue, localStorage.getItem("openId"));
                });
            } else {
                localStorage.setItem("current_user", JSON.stringify(obj.model));
            }
        },
        error: function (d) {
            console.log(d);
        }
    });
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
                alert("该号码不存在,请重新输入");
            } else {
                var obj = JSON.parse(d);
                if (localStorage) {
                    localStorage.setItem("current_user", JSON.stringify(obj));
                    localStorage.setItem("opneId", obj.UserRemark);
                }
                console.log(obj);
                swal({
                    title: "登陆成功!",
                    text: "",
                    timer: 800,
                    showConfirmButton: false
                });
            }
        },
        error: function (d) {
            console.log(d);
        }
    });
}
//});

