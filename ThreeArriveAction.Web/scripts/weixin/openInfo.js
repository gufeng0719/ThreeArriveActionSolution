var vm = new Vue({
    el: "#app",
    data: {
        type: "",
        gimg: "",
        imgs: [],
        village: "",
        msg: ""
    },
    methods: {
        openImg: function (img) {
            window.open("image.html?id=" + req["id"] + "&current=" + img, "_self");
        }
    },
    mounted: function () {
        var that = this;
        $.ajax({
            type: "post",
            url: "../Ajax/sys_PublicMessagesManager.ashx",
            data: {
                type: "getModel",
                id: req["id"]
            },
            complete: function (d) {
                var obj = JSON.parse(d.responseText);
                that.type = obj.type;
                that.gimg = obj.gimg;
                that.imgs = obj.imgs;
                that.village = obj.village;
                that.msg = obj.msg;
            }
        });
    }
});