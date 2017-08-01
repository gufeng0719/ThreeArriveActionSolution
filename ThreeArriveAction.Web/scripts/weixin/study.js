var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 10,
        totle: 0,
        type: -1,
        title: "",
        isNotMore: false
    },
    methods: {
        getpage: function (page) {
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_InteractionLearnsManager.ashx",
                data: {
                    type: "getPageList",
                    page: page,
                    size: that.size,
                    pageType: that.type,
                    title: that.title
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.list = that.list.concat(obj.list);
                    if (obj.list.length < 1) {
                        that.isNotMore = true
                    } else {
                        that.isNotMore = false
                    }
                    if (window.scrolled) {
                        window.scrolled = false;
                    }
                    that.page = obj.page;
                    that.totle = obj.totle;
                }
            });
        },
        toInfo: function (id) {
            window.open("studyInfo.html?id=" + id, "_self");
        }
    },
    watch: {
        type: function () {
            this.isNotMore = false;
            this.list = []
            this.getpage(1);
        }
    },
    mounted: function () {
        this.getpage(1);
    }
});
