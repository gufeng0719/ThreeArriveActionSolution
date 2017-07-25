
var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 5,
        totle: 0,
        ddlvillage: -1
    },
    methods: {
        getpage: function (page) {
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_PublicMessagesManager.ashx",
                data: {
                    type: "openList",
                    openType: req["type"],
                    page: page,
                    size: that.size,
                    village: that.ddlvillage
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.page = obj.page;
                    that.list = obj.list,
                    that.totle = obj.totle;
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
        gotoInfo: function (id) {
            window.open("openInfo.html?id=" + id, "_self");
        }
    },
    watch: {
        "ddlvillage": function () {
            this.getpage(1);
        }
    },
    mounted: function () {
        this.getpage(1);
        if (req["type"] == "1") {
            $("#pagetitle").html("三到行动-党务公开");
        }
        if (req["type"] == "2") {
            $("#pagetitle").html("三到行动-村务公开");
        }
        if (req["type"] == "3") {
            $("#pagetitle").html("三到行动-财务公开");
        }
    }
});

