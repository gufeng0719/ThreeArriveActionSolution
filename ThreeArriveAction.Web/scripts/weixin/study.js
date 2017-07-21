var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 10,
        totle: 0,
        type: -1,
        title: ""
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
            window.open("studyInfo.html?id=" + id, "_self");
        }
    },
    watch: {
        type: function () {
            this.getpage(1);
        }
    },
    mounted: function () {
        this.getpage(1);
    }
});

function submitcontent() {
    $.ajax({
        type: "post",
        url: "",
        data: {

        },
        complete: function (d) {
            //d.responseText
        }
    });
}