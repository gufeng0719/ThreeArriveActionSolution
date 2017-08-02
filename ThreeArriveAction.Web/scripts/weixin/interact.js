var vm = new Vue({
    el: "#app",
    data: {
        list: [
        ],
        page: 1,
        size: 6,
        isNotMore: false
    },
    methods: {
        getpage: function (page) {
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
                    that.totle = obj.totle;
                    that.page = obj.page;

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
        select: function (id) {
            window.open("interactInfo.html?id=" + id, "_self");
        }
    },
    mounted: function () {
        this.getpage(1);
    }
});

