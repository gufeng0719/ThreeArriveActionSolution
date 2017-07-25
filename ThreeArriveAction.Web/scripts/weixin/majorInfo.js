var vm = new Vue({
    el: "#app",
    data: {
        userName: "",
        time: "",
        title: "",
        content: ""
    },
    methods: {
        readed: function () {
            $.ajax({
                type: "post",
                url: "../Ajax/sys_MajorInfoManager.ashx",
                data: {
                    type: "readed",
                    id: req["id"],
                    userId: JSON.parse(localStorage.getItem("current_user")).UserId
                },
                complete: function (d) {
                    if (d.responseText > 0) {
                        window.open("major.html", "_self");
                    }
                }
            });
        }
    },
    mounted: function () {
        var that = this;
        $.ajax({
            type: "post",
            url: "../Ajax/sys_MajorInfoManager.ashx",
            data: {
                type: "getModel",
                id: req["id"]
            },
            complete: function (d) {
                var obj = JSON.parse(d.responseText);
                that.userName = obj.userName;
                that.time = obj.time;
                that.title = obj.title;
                that.content = obj.content;
            }
        });
    }
});