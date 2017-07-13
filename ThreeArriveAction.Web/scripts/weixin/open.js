function submitcontent() {
    submit(function (paths, x, y) {
        $.ajax({
            type: "post",
            url: "../Ajax/sys_PublicMessagesManager.ashx?type=add",
            data: {
                paths: ["1", "23", "345", "56756", "奥术大师", "阿萨德"],
                openId: localStorage.getItem("openId"),
                openType: $("[name='opentype']:checked").val(),
                msg: $("#msg").val()
            },
            complete: function (d) {

            }
        });
    });
};