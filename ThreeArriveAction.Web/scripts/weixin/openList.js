
function GetRequest() {
    var url = location.search; //获取url中"?"符后的字串
    var theRequest = new Object();
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
    }
    return theRequest;
}
var req = GetRequest();

var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        page: 1,
        size: 5,
        totle: 0,
        ddltown: -1,
        ddlvillage: -1,
        townlist: [],
        villagelist: [],
        allvillage: []
    },
    methods: {
        getpage: function (page) {
            var that = this;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_PublicMessagesManager.ashx",
                data: {
                    type: "openList",
                    openType: req["type"],
                    page: page,
                    size: that.size,
                    town: that.ddltown,
                    village: that.dllvillage
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.page = obj.page;
                    that.list = obj.list,
                    that.totle = obj.totle;
                }
            });
        }
    },
    computed: {

    },
    watch: {
        "page": function (newValue) {
            if (newValue > 1) {
                $("#lastpage").attr("disabled", false)
            } else {
                $("#lastpage").attr("disabled", true)
            }
        },
        "totle": function (newValue) {
            if (this.page < newValue / this.size) {
                $("#nextpage").attr("disabled", false)
            } else {
                $("#nextpage").attr("disabled", true)
            }
        },
        "ddltown": function (newValue) {
            if (newValue == -1) {
                return;
            }
            var that = this;
            var child = _.find(that.allvillage, function (v) {
                return v.parent.value == newValue
            });
            if (child) {
                that.villagelist = child.child;
            }
            that.ddlvillage = -1;
        },
        "ddlvillage": function (newValue) {
            if (newValue == -1) {
                return;
            }
            this.getpage(1);
        }
    },
    mounted: function () {
        var that = this;
        $.ajax({
            type: "post",
            url: "../Ajax/sys_VillagesManager.ashx?type=getAll",
            data: {
            },
            complete: function (d) {
                var obj = JSON.parse(d.responseText);
                that.allvillage = obj;
                obj.forEach((o) => {
                    that.townlist.push(o.parent);
                });
            }
        });
        this.getpage(1);
    }
});
