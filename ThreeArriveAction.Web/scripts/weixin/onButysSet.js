var vm = new Vue({
    el: "#app",
    data: {
        town: "",
        village: "",
        villageId: 0,
        userValue: -1,
        list: [],
        exist: false
    },
    methods: {
        submitcontent: function () {
            var that = this;
            if (that.userValue === -1) {
                alert("请先选择值班人员");
                return;
            }
            $.ajax({
                type: "post",
                url: "../Ajax/sys_OnButysManager.ashx?type=set",
                data: {
                    userid: that.userValue,
                    villageid: that.villageId
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    alert(obj.info);
                    if (obj.status === "y") {
                        window.open("onButys.html", "_self");
                    }
                }
            });
        }
    },
    mounted: function () {
        var that = this;
        $.ajax({
            type: "post",
            url: "../Ajax/sys_UsersManager.ashx?type=weixinGetButyUser",
            data: {
                openId: localStorage.getItem("openId")
            },
            complete: function (d) {
                var obj = JSON.parse(d.responseText);
                that.town = obj.town;
                that.village = obj.village;
                that.list = obj.list;
                that.villageId = obj.villageId;
                that.exist = obj.exist;
                if (that.exist) {
                    $("#btnSub").attr("disabled", true);
                } else {
                    $("#btnSub").attr("disabled", false);
                }
            }
        });
    }
});