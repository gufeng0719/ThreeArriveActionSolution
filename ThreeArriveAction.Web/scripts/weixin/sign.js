
window.onload = function () {
    var date = new Date();
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hours = date.getHours();
    var min = date.getMinutes();

    $("#times").html("今天是" + year + "年" + month + "月" + day + "日 <br />" + hours + "时" + min + "分");

    function _fresh() {
        var endtime = new Date(date);
        endtime.setMinutes(endtime.getMinutes() + 5);
        var nowtime = new Date();
        var leftsecond = parseInt((endtime.getTime() - nowtime.getTime()) / 1000);
        __d = parseInt(leftsecond / 3600 / 24);
        __h = parseInt((leftsecond / 3600) % 24);
        __m = parseInt((leftsecond / 60) % 60);
        __s = parseInt(leftsecond % 60);
        $("#endtimes").html("距离签到结束还有<span>" + __m + "" + "</span>分 <span>" + "" + __s + "" + "</span>秒");
        if (leftsecond <= 0) {
            $("#endtimes").html("今天签到已经结束");
            clearInterval(sh);
        }
    }
    var endStr;
    var sh;
    sh = setInterval(_fresh, 1000);
}

