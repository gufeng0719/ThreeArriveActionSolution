
var vm = new Vue({
    el: "#app",
    data: {
        family: -1,
        x: 33.5226,
        y: 119.156456,
        name: "",
        phone: "",
        address: "",
        number: 1,
        msg: ""
    },
    methods: {
        submitcontent: function () {
            if (!this.name) {
                alert("请输入户主姓名");
                return;
            }
            if (!this.phone) {
                alert("请输入联系方式");
                return;
            }
            if (!this.address) {
                alert("请输入家庭住址");
                return;
            }
            if (this.number === 0) {
                alert("家庭人口不可为0");
                return;
            }
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_SubscriberFamilyManager.ashx?type=addSubfamily",
                data: that.$data,
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    if (obj.line > 0) {
                        alert("修改成功");
                        window.open("index.html", "_self");
                    }
                }
            });
        },
        getSubfamilyModel: function (id) {
            if (id < 1) return;
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_SubscriberFamilyManager.ashx?type=getSubfamilyModel",
                data: {
                    id: id
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.x = obj.x;
                    that.y = obj.y;
                    that.name = obj.name;
                    that.phone = obj.phone;
                    that.address = obj.address;
                    that.number = obj.number;
                    that.msg = obj.msg;
                }
            });
        }
    },
    watch: {
        x: function (newValue) {
            $("#baiduframe").attr("src", "../../editor/plugins/baidumap/weixinIndex.html?yjindu=" + this.y + "&xweidu=" + newValue);
        },
        y: function (newValue) {
            $("#baiduframe").attr("src", "../../editor/plugins/baidumap/weixinIndex.html?yjindu=" + newValue + "&xweidu=" + this.x);
        },
        family: function (newValue) {
            this.getSubfamilyModel(newValue);
        }
    },
    mounted: function () {

    }
});

$(function () {
    $.ajax({
        type: "post",
        data: {
            openid: localStorage.getItem("openId"),
            type: "getJsapiTicket",
            url: location.href
        },
        url: "../Ajax/weixinInfo.ashx",
        success: function (d) {
            var obj = JSON.parse(d);
            console.log(obj);
            window.wx.config({
                debug: false,
                appId: obj.appId,
                timestamp: obj.timestamp,
                nonceStr: obj.nonceStr,
                signature: obj.signature,
                jsApiList: obj.jsApiList
            });
        },
        error: function (d) {
            console.log(d);
        }
    });

    window.wx.ready(function () {
        window.wx.getLocation({
            type: 'gcj02',
            success: function (res) {
                vm.x = res.latitude;
                vm.y = res.longitude;
            }
        });
    });

});
