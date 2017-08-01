var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 5,
        totle: 0
    },
    methods: {
        getpage: function (page) {
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_MajorInfoManager.ashx",
                data: {
                    type: "getPageList",
                    page: page,
                    size: that.size,
                    userId: JSON.parse(localStorage.getItem("current_user")).UserId
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.list = obj.list;
                    that.page = obj.page;
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
        toInfo: function (id) {
            window.open("majorInfo.html?id=" + id, "_self");
        }
    },
    mounted: function () {
        this.getpage(1);
    }
});