

var vm = new Vue({
    el: "#app",
    data: {
        ddlvillage: -1,
        page: 1,
        size: 5,
        totle: 0,
        list: [],
        isNotMore: false
    },
    methods: {
        getpage: function (page) {
            //var that = this;
            //$.ajax({
            //    type: "post",
            //    url: "../Ajax/sys_OnButysManager.ashx?type=getOnButysList",
            //    data: {
            //        page: page,
            //        size: that.size,
            //        ddlvillage: that.ddlvillage
            //    },
            //    complete: function (d) {
            //        var obj = JSON.parse(d.responseText);
            //        that.page = obj.page;
            //        that.totle = obj.totle;
            //        that.list = that.list.concat(obj.list);
            //        if (obj.list.length < 1) {
            //            that.isNotMore = true
            //        } else {
            //            that.isNotMore = false
            //        }
            //        if (window.scrolled) {
            //            window.scrolled = false;
            //        }
            //    }
            //});
        }
    },
    watch: {
        ddlvillage: function (newValue) {
            if (newValue < 1) return
            this.isNotMore = false;
            this.list = [];
            this.getpage(1);
        }
    },
    mounted: function () {

    }
});


