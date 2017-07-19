var vm = new Vue({
    el: "#app",
    data: {
        ddlvillage: -1,
        page: 0,
        size: 5,
        totle: 0,
        list: []
    },
    methods: {
        getpage: function (page) {
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_OnButysManager.ashx?type=getOnButysList",
                data: {
                    page: page,
                    size: that.size,
                    ddlvillage: that.ddlvillage
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
        }
    },
    watch: {
        ddlvillage: function() {
            this.getpage(1);
        }
    },
    mounted: function () {
        this.getpage(1);
    }
});