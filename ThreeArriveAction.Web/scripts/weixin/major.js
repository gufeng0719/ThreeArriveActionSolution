var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 5,
        totle: 0,
        isNotMore: false
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
        toInfo: function (id) {
            window.open("majorInfo.html?id=" + id, "_self");
        }
    },
    mounted: function () {
        this.getpage(1);
    }
});
