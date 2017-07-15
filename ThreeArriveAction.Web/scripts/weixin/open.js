function submitcontent() {
    if (!$("[name='opentype']:checked").val()) {
        alert("请选择公开类型");
        return;
    }   
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