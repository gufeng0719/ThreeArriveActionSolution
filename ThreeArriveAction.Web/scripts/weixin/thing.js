var vm = new Vue({
    el: "#app",
    data: {
        localIds: [],
        family: -1
    }
});

function submitcontent() {
    if (vm.family === -1) {
        alert("请选择拜访户");return;
    }
    if (!$("#title").val()) {
        alert("请输入事件"); return;
    }
    if (!$("#thing").val()) {
        alert("请输入原因"); return;
    }
    if (!$("[name='thinghaving']").val()) {
        alert("请选择是否解决"); return;
    }
    if (!$("#Textarea1").val()) {
        alert("请输入解决放方式"); return;
    }
    submit(function (paths) {
        $.ajax({
            type: "post",
            data: {
                openId: localStorage.getItem("openId"),
                thingname: $("#title").val(),
                thingreason: $("#thing").val(),
                thingsolution: $("#Textarea1").val(),
                thinghaving: $("[name='thinghaving']:checked").val() == "1" ? "1" : "",
                imgurl: paths[0],
                slSubId: vm.family
            },
            url: "../Ajax/sys_ThingRecordManager.ashx?type=add",
            success: function (d) {
                var obj = JSON.parse(d);
                alert(obj.info);
                window.open("index.html", "_self");
            },
            error: function (d) {
                console.log(d);
            }
        });
    });
}