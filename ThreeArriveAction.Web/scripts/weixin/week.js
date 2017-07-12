
function submitcontent() {
    submit(function (paths, xpoint, ypoint) {
        $.ajax({
            url: "../Ajax/sys_WeekArrivesManager.ashx?type=add",
            type: "post",
            data: {
                opneId: localStorage.getItem("openId"),
                slSubId: subfamily.val(),
                ThingMessage: $("#thing").html(),
                ThingResult: $("#result").html(),
                txtImgUrl: paths[0],
                xpoint: xpoint,
                ypoint: ypoint
            },
            success: function (d) {
                alert(JSON.parse(d).info);
            }, error: function (d) {
                console.log(d);
            }
        });
    });
};
