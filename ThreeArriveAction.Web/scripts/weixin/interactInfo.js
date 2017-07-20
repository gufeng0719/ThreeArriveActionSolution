
var vm = new Vue({
    el: "#app",
    data: {
        list: [],
        praise: 0,
        yetPraise: false,
        time: "",
        content: "",
        reContent: "",
        userName: "",
        userId: 0,
        imgs: [],
        id: req["id"]
    },
    methods: {
        pushPraise: function () {
            var that = this;
            if (that.yetPraise) {
                return;
            }
            that.yetPraise = true;
            $.ajax({
                type: "post",
                url: "../Ajax/sys_LeaveMessageManager.ashx",
                data: {
                    type: "pushPraise",
                    id: that.id,
                    openId: localStorage.getItem("openId")
                },
                complete: function (d) {
                    var number = d.responseText;
                    that.praise = parseInt(number);
                    if (number > 0) {
                        that.yetPraise = true;
                    } else {
                        that.yetPraise = false;
                    }
                }
            });
        },
        comment: function (toPlayer) {
            var that = this;
            if (!that.reContent) {
                alert("请输入回复内容");
                return;
            }
            $.ajax({
                type: "post",
                url: "../Ajax/sys_ReplaysManager.ashx",
                data: {
                    type: "comment",
                    openId: localStorage.getItem("openId"),
                    toPlayer: toPlayer,
                    id: that.id,
                    content: that.reContent
                },
                complete: function (d) {
                    var obj = JSON.parse(d.responseText);
                    that.list.push(obj);
                    that.reContent = "";
                    swal.close();
                }
            });
        },
        commentToOther: function (toPlayer, toPlayerName) {
            var that = this;
            swal({
                title: " ",
                text: " ",
                type: "input",
                showCancelButton: true,
                closeOnConfirm: false,
                animation: "slide-from-bottom",
                confirmButtonText: "回复",
                cancelButtonText: "取消",
                inputPlaceholder: "回复：" + toPlayerName
            }, function (inputValue) {
                debugger;
                if (inputValue === false) return false;
                if (inputValue === "") {
                    swal.showInputError("请输入回复内容");
                    return false;
                }
                that.reContent = inputValue;
                that.comment(toPlayer);
            });
        }
    },
    mounted: function () {
        var that = this;
        $.ajax({
            type: "post",
            url: "../Ajax/sys_LeaveMessageManager.ashx",
            data: {
                type: "getModel",
                id: that.id,
                openId: localStorage.getItem("openId")
            },
            complete: function (d) {
                var obj = JSON.parse(d.responseText);
                that.praise = obj.praise;
                that.time = obj.time;
                that.content = obj.content;
                that.userName = obj.userName;
                that.imgs = obj.imgs;
                that.yetPraise = obj.yetPraise;
                that.userId = obj.userId;
            }
        });
        $.ajax({
            type: "post",
            url: "../Ajax/sys_ReplaysManager.ashx",
            data: {
                type: "getList",
                id: that.id,
                openId: localStorage.getItem("openId")
            },
            complete: function (d) {
                that.list = JSON.parse(d.responseText).list;
            }
        });
    }
});

