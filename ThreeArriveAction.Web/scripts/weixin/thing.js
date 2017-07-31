var vm = new Vue({
    el: "#app",
    data: {
        localIds: [],
        family: -1,
        familyType: -1
    },
    mounted: function () {

        this.$children[0].typeList.push({ key: 8, value: "其他" })

    }
});

function submitcontent() {
    if (vm.familyType === 8) {
        if (!$("#name").val()) {
            alert("请输入姓名"); return;
        }
        if (!$("#phone").val()) {
            alert("请输入联系方式"); return;
        }
    } else {
        if (vm.family === -1) {
            alert("请选择拜访户"); return;
        }
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
                thinghaving: $("[name='thinghaving']:checked").val() === "1" ? "1" : "",
                imgurl: paths[0],
                slSubId: vm.family,
                name: $("#name").val(),
                phone: $("#phone").val()
            },
            url: "../Ajax/sys_ThingRecordManager.ashx?type=add",
            success: function (d) {
                var obj = JSON.parse(d);
                alert(obj.info);
                sendMsgToShuji();
            },
            error: function (d) {
                console.log(d);
            }
        });
    });
}

function sendMsgToShuji() {
    $.ajax({
        type: "post",
        url: "../Ajax/sys_ThingRecordManager.ashx?type=getMsgInfo",
        data: {
            openId: localStorage.getItem("openId")
        },
        complete: function (d) {
            var obj = JSON.parse(d.responseText);
            sendTemplateMsg(obj.toOpenId, obj, function (msg) {
                console.log(msg);
                window.open("index.html", "_self");
            });
        }
    });
}