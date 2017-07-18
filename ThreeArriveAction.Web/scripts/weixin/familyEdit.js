
Vue.component("ddlsee", {
    template:
        '<div><select v-model="type">                                                            ' +
        '   <option value="-1">-请选择-</option>                                             ' +
        '   <option v-for="model in typeList" :value="model.key">{{model.value}}</option>  ' +
        '</select>                                                                          ' +
        '<select v-model="myfamily">                                                          ' +
        '   <option value="-1">-请选择-</option>                                             ' +
        '   <option selected v-for="model in familyList" :value="model.key">{{model.value}}</option>' +
        '</select></div>',
    props: ["family"],
    data: function () {
        return {
            familyList: [],
            typeList: [
                {
                    key: 1,
                    value: "留守老人户"
                }, {
                    key: 2,
                    value: "重大病户"
                }, {
                    key: 3,
                    value: "五保户"
                }, {
                    key: 4,
                    value: "低保户"
                }, {
                    key: 5,
                    value: "离任村干部户"
                }, {
                    key: 6,
                    value: "老党员户"
                }, {
                    key: 7,
                    value: "信访户"
                }
            ],
            type: -1,
            myfamily: -1
        }
    },
    watch: {
        type: function (newValue) {
            var that = this;
            $.ajax({
                url: "../Ajax/sys_SubscriberFamilyManager.ashx?type=getsubfamily",
                type: "post",
                data: {
                    value: newValue,
                    openId: localStorage.getItem("openId")
                },
                success: function (d) {
                    var obj = JSON.parse(d);
                    that.familyList = obj;
                    that.myfamily = -1;
                }, error: function (d) {
                    console.log(d);
                }
            });
        },
        myfamily: function (newValue) {
            vm.family = newValue;
        }
    }
});

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
