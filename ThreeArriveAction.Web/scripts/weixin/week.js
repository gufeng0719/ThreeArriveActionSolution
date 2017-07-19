var vm = new Vue({
    el: "#app",
    data: {
        localIds: [],
        family: -1
    }
});

function submitcontent() {
    if (vm.family === -1) {
        alert("请选择拜访户"); return;
    }
    if (!$("#thing").val()) {
        alert("请输入描述"); return;
    }
    if (!$("#thing").val()) {
        alert("请输入结果"); return;
    }
    submit(function (paths, xpoint, ypoint) {
        $.ajax({
            url: "../Ajax/sys_WeekArrivesManager.ashx?type=add",
            type: "post",
            data: {
                openId: localStorage.getItem("openId"),
                slSubId: vm.family,
                ThingMessage: $("#thing").val(),
                ThingResult: $("#result").val(),
                txtImgUrl: paths[0],
                xpoint: xpoint,
                ypoint: ypoint
            },
            success: function (d) {
                alert(JSON.parse(d).info);
                window.open("index.html", "_self");
            }, error: function (d) {
                console.log(d);
            }
        });
    });
};
