function submitcontent() {
    submit(function (paths, x, y) {
        $.ajax({
            type: "post",
            url: "../Ajax/sys_PublicMessagesManager.ashx?type=add",
            data: {
                paths: paths,
                openId: localStorage.getItem("openId"),
                openType: $("[name='opentype']:checked").val(),
                msg: $("#msg").val()
            },
            complete: function (d) {

            }
        });
    });
};