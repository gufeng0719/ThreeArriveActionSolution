
var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 10,
        totle: 0,
        ddlvillage: -1,
        isNotMore: false
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
        minute: function (id) {
            window.open("integralInfo.html?id=" + id, "_self");
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
    }
});