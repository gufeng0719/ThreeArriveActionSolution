var vm = new Vue({
    el: "#app",
    data: {
        list: [
        ],
        page: 1,
        size: 6,
        tbfoot: false,
        tbfootMsg: "",
        notMore: false
    },
    methods: {
        getpage: function (page) {
            if (this.notMore) {
                return;
            }
            $(window).scroll(function () { });
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_LeaveMessageManager.ashx",
                data: {
                    type: "getPage",
                    page: page,
                    size: that.size
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.list = obj.list;
                    that.totle = obj.totle;
                    that.page = obj.page;
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
        "list.length": function (newValue) {
            if (newValue <= 0) {
                this.tbfoot = true;
                this.tbfootMsg = "暂无数据";
            }
        }
    },
    mounted: function () {
        this.getpage(1);
    }
});

