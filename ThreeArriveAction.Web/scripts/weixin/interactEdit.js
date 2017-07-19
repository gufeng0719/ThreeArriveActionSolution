

var vm = new Vue({
    el: "#app",
    data: {
        localIds: [],
        content: "",
        needlocalIds: false
    },
    methods: {
        submitcontent: function () {
            var that = this;
            if (!that.content) {
                alert("请输入留言内容");
                return;
            }
            submit(function (paths) {
                $.ajax({
                    type: "post",
                    url: "../Ajax/sys_LeaveMessageManager.ashx",
                    data: {
                        type: "add",
                        openId: localStorage.getItem("openId"),
                        paths: paths,
                        content: that.content
                    },
                    complete: function (d) {
                        if (d.responseText > 0) {
                            alert("发布成功");
                            window.open("interact.html", "_self");
                        }
                    }
                });
            });

        }
    }
});