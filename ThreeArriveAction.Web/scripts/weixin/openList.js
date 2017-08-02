
var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 5,
        totle: 0,
        ddlvillage: -1,
        isNotMore: false
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
                    that.totle = obj.totle;
                    that.list = that.list.concat(obj.list);
                    if (obj.list.length < 1) {
                        that.isNotMore = true
                    } else {
                        that.isNotMore = false
                    }
                    if (window.scrolled) {
                        window.scrolled = false;
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
            this.isNotMore = false;
            this.list = [];
            this.getpage(1);
        }
    },
    mounted: function () {
        //this.getpage(1);
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

