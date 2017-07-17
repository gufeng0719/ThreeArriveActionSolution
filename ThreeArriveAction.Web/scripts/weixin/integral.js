
var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 10,
        totle: 0,
        ddlvillage: -1
    },
    methods: {
        getpage: function (page) {
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_UserIntegralsManager.ashx",
                data: {
                    type: "integralList",
                    page: page,
                    size: that.size,
                    ddlvillage: this.ddlvillage
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
        minute: function (id) {
            window.open("integralInfo.html?id=" + id, "_self");
        }
    },
    watch: {
        "ddlvillage": function () {
            this.getpage(1);
        }
    },
    mounted: function () {
        this.getpage(1);
    }
});