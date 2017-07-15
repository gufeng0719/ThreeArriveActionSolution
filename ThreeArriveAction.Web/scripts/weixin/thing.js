function submitcontent() {
    submit(function (paths) {
        $.ajax({
            type: "post",
            data: {
                openId: localStorage.getItem("openId"),
                thingname: $("#title").val(),
                thingreason: $("#thing").val(),
                thingsolution: $("#Textarea1").val(),
                thinghaving: $("[name='thinghaving']:checked").val() == "1" ? "1" : "" ,
                imgurl: paths[0],
                slSubId: $("#subfamily").val(),
            },
            url: "../Ajax/sys_ThingRecordManager.ashx?type=add",
            success: function (d) {
                var obj = JSON.parse(d);
                alert(obj.info);
            },
            error: function (d) {
                console.log(d);
            }
        });
    });
}