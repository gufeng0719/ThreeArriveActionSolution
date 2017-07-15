
var sh;
var etime;
window.onload = function () {
    $.ajax({
        type: "post",
        url: "../Ajax/sys_SignManager.ashx?type=get",
        data: {
            openId: localStorage.getItem("openId")
        },
        success: function (d) {
            var obj = JSON.parse(d);
            if (obj.success === "True") {
                if (obj.obj == undefined || obj.obj == null) {
                    $("#txtsign").text("开启签到");
                    $("#endtimes").hide();
                    $("#btnsign").show();
                } else {
                    $("#txtsign").text("签到");
                    $("#endtimes").show();
                    $("#btnsign").show();
                    etime = new Date(obj.obj);
                    setTimeout(function () {
                        _fresh();
                    }, 1000);
                }
            } else {
                $("#endtimes").hide();
                $("#btnsign").hide();
                alert(obj.msg);
                window.open("index.html", "_self");
            }
        }
    });

    var date = new Date();
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hours = date.getHours();
    var min = date.getMinutes();

    $("#times").html("今天是" + year + "年" + month + "月" + day + "日 <br />" + hours + "时" + min + "分");

    function _fresh() {
        var endtime = new Date(etime);
        endtime.setMinutes(endtime.getMinutes() + 5);
        var nowtime = new Date();
        var leftsecond = parseInt((endtime.getTime() - nowtime.getTime()) / 1000);
        __m = parseInt((leftsecond / 60) % 60);
        __s = parseInt(leftsecond % 60);
        $("#endtimes").html("距离签到结束还有<span>" + __m + "" + "</span>分 <span>" + "" + __s + "" + "</span>秒");
        if (leftsecond <= 0) {
            $("#endtimes").html("今天签到已经结束");
            window.open("index.html", "_self");
        } else {
            setTimeout(function () {
                _fresh();
            }, 1000);
        }
    }
}

function addsign() {
    $.ajax({
        type: "post",
        url: "../Ajax/sys_SignManager.ashx?type=add",
        data: {
            openId: localStorage.getItem("openId")
        },
        success: function (d) {
            var obj = JSON.parse(d);
            if (obj.success === "True") {
                $("#endtimes").hide();
                $("#btnsign").hide();
            }
            alert(obj.msg);
            window.open("index.html", "_self");
        }
    });
}

