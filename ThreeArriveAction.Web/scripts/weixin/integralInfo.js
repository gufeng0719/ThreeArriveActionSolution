
function GetRequest() {
    var url = location.search; //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") !== -1) {
        var str = url.substr(1);
        var strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
req = GetRequest();

var vm = new Vue({
    el: "#app",
    data: {
        name: "",
        village: "",
        score: 0,
        list: [],
        page: 1,
        size: 10,
        totle: 0,
        userId: 0
    },
    methods: {
        getpage: function (page) {
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_IntegralInfoManager.ashx",
                data: {
                    type: "integralInfoList",
                    page: page,
                    size: that.size,
                    userId: that.userId
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.page = obj.page;
                    that.totle = obj.totle;
                    that.list = obj.list;
                    if (that.page > 1) {
                        $("#lastpage").attr("disabled", false);
                    } else {
                        $("#lastpage").attr("disabled", true);
                    }
                    if (that.page < (that.totle / that.size)) {
                        $("#nextpage").attr("disabled", false);
                    } else {
                        $("#nextpage").attr("disabled", true);
                    }
                }
            });
        },
        getmodel: function () {
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_UserIntegralsManager.ashx",
                data: {
                    type: "getModel",
                    id: req["id"]
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.name = obj.name;
                    that.village = obj.village;
                    that.score = obj.score;
                    that.userId = obj.userId;
                    that.getpage(1);
                }
            });
        }
    },
    mounted: function () {
        this.getmodel();
    }

});