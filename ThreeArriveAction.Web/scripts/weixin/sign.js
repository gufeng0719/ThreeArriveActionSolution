
var sh;
var etime;
var isShuJ = JSON.parse(localStorage.getItem("current_user")).OrganizationId === 3;
var image1 = "";
var image2 = "";
var image3 = "";
var image4 = "";

window.onload = function () {
    $.ajax({
        type: "post",
        url: "../Ajax/sys_SignManager.ashx?type=get",
        data: {
            openId: window.localStorage.getItem("openId")
        },
        success: function (d) {
            var obj = JSON.parse(d);
            if (obj.success === "True") {
                if (obj.obj == undefined || obj.obj == null) {
                    $("#txtsign").text("开启签到")
                    $("#endtimes").hide();
                    $("#btnsign").show();
                } else {
                    $("#txtsign").text("签到");
                    $("#endtimes").show();
                    $("#btnsign").show();
                    etime = new Date(obj.obj);
                    setTimeout(function () {
                        fresh();
                    }, 1000);
                }
            } else {
                if (isShuJ) {
                    $("#endtimes").show();
                    $('#signList').show();
                    var signInfo = getIsSubmit();
                    if (signInfo.code === 1) {
                        setTimeout(function () { getSignList() }, 100)
                        alert(signInfo.msg);
                        $('#imgs').show();
                    } else {
                        alert(signInfo.msg);
                        window.open("index.html", "_self");
                    }
                } else {
                    $('#signList').hide();
                    $("#endtimes").hide();
                    alert(obj.msg);
                    window.open("index.html", "_self");
                }
                $("#btnsign").hide();
            }
        }
    });
}

var date = new Date();
var year = date.getFullYear();
var month = date.getMonth() + 1;
var day = date.getDate();
var hours = date.getHours();
var min = date.getMinutes();

$("#times").html("今天是" + year + "年" + month + "月" + day + "日 <br />" + hours + "时" + min + "分");

function fresh() {
    var endtime = new Date(etime);
    endtime.setMinutes(endtime.getMinutes() + 5);
    var nowtime = new Date();
    var leftsecond = parseInt((endtime.getTime() - nowtime.getTime()) / 1000);
    var m = parseInt((leftsecond / 60) % 60);
    var s = parseInt(leftsecond % 60);
    $("#endtimes").html("距离签到结束还有<span>" + m + "" + "</span>分 <span>" + "" + s + "" + "</span>秒");
    if (leftsecond <= 0) {
        $("#endtimes").html("今天签到已经结束");
        if (isShuJ) {
            $('#imgs').show();
        } else {
            $('#signList').hide();
            $("#endtimes").hide();
            window.open("index.html", "_self");
        }
        $("#btnsign").hide();
    } else {
        setTimeout(function () {
            fresh();
        }, 1000);
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
                $("#btnsign").hide();
            }
            alert(obj.msg);
            if (obj.msg === "个人信息异常") {
                localStorage.clear();
                alert("已为您清除缓存,请重新打开微信尝试");
            }
            if (isShuJ) {
                $("#signList").show();
                setTimeout(function () { getSignList() }, 100)
            } else {
                window.open("index.html", "_self");
            }
        }
    });
    $("#endtimes").show();
    etime = new Date();
    fresh();
}

function getSignList() {
    $.ajax({
        type: "post",
        url: "../Ajax/sys_SignManager.ashx?type=getSignList",
        data: {
            villageId: JSON.parse(localStorage.getItem("current_user")).VillageId
        },
        complete: function (d) {
            var obj = JSON.parse(d.responseText);
            var str = '';
            obj.forEach((model) => {
                str += `<p>` +
                    model.time +
                    `<span style="margin-left: 15px; margin-right: 15px;color:darkblue">${model.name}</span>` +
                    `签到` +
                    `</p>`;
            });
            $("#signList>#list").html(str)
            setTimeout(function () { getSignList() }, 3000)
        }
    });
}

function pz(type) {
    window.wx.chooseImage({
        count: 1,
        sizeType: ['original', 'compressed'],
        sourceType: ['album', 'camera'],
        success: function (res) {
            if (type === 1)
                image1 = res.localIds.toString()
            if (type === 2)
                image2 = res.localIds.toString()
            if (type === 3)
                image3 = res.localIds.toString()
            if (type === 4)
                image4 = res.localIds.toString()
        }
    });
}

function submit() {
    if (!image1) {
        alert("请上传会议记录照片");
        return;
    }
    if (!image2) {
        alert("请上传学习记录照片");
        return;
    }
    if (!image3) {
        alert("请上传点名照片照片");
        return;
    }
    if (!image4) {
        alert("请上传工作布置记录照片");
        return;
    }
    var path1 = downFile(1);
    var path2 = downFile(2);
    var path3 = downFile(3);
    var path4 = downFile(4);

    $.ajax({
        type: "post",
        url: "../Ajax/sys_SignInfoManager.ashx",
        data: {
            type: "add",
            userId: JSON.parse(localStorage.getItem("current_user")).UserId,
            path1: path1,
            path2: path2,
            path3: path3,
            path4: path4,
            msg: $("#msg").val()
        },
        complete: function (d) {
            if (d.responseText > 0) {
                alert("提交成功~");
                window.open("index.html", "_self");
            }
        }
    });

}

function downFile(type) {
    var obj;
    var image = "";
    if (type === 1) {
        image = image1;
    }
    if (type === 2) {
        image = image2;
    }
    if (type === 3) {
        image = image3;
    }
    if (type === 4) {
        image = image4;
    }
    wx.uploadImage({
        localId: image,
        isShowProgressTips: 1,
        success: function (res) {
            $.ajax({
                url: "../Ajax/weixinInfo.ashx",
                type: "post",
                async: false,
                data: {
                    type: "downFile",
                    mediaId: res.serverId,
                    openId: localStorage.getItem("openId")
                },
                success: function (d) {
                    obj = d;
                }
            });
        }
    });
    return obj;
}

function getIsSubmit() {
    var obj;
    $.ajax({
        type: "post",
        url: "../Ajax/sys_SignInfoManager.ashx",
        async: false,
        data: {
            type: "getIsSubmit",
            userId: JSON.parse(localStorage.getItem("current_user")).UserId
        },
        complete: function (d) {
            obj = JSON.parse(d.responseText);
        }
    });
    return obj;
}

