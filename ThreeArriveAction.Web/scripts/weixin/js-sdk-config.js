
require.config({
    baseUrl: "/scripts/",
    waitSeconds: 30,
    paths: {
        vue: 'vue/vue',
        jweixin: 'weixin/jweixin-1.2.0',
    },
});

require(["vue", "jweixin"],
    function (Vue, wx) {
        var timeOutEvent = 0;

        $.ajax({
            type: "post",
            data: {
                openid: localStorage.getItem("openId"),
                type: "getJsapiTicket",
                url: location.href
            },
            url: "../Ajax/weixinInfo.ashx",
            success: function (d) {
                var obj = JSON.parse(d);
                console.log(obj);
                wx.config({
                    debug: true,
                    appId: obj.appId,
                    timestamp: obj.timestamp,
                    nonceStr: obj.nonceStr,
                    signature: obj.signature,
                    jsApiList: obj.jsApiList,
                });
            },
            error: function (d) {
                console.log(d);
            }
        });

        Vue.component("image-template", {
            template:
                    `<div v-if="isMore">                                                                ` +
                    `    <a v-for="(localId,index) in localIds"                                         ` +
                    `        :data-id="localId"                                                         ` +
                    `        @touchstart="gtouchstart(localId.toString())"                              ` +
                    `        @touchmove="gtouchmove()"                                                  ` +
                    `        @touchend="gtouchend()">                                                   ` +
                    `        <img :src="localId" style="width: 32%;" />                                 ` +
                    `    </a>                                                                           ` +
                    `    <a @click="pz()" v-if="localIds.length < 9">                                   ` +
                    `        <img src="../images/templates/bottommenu/218.png" style="width: 32%;" />   ` +
                    `    </a>                                                                           ` +
                    `</div>                                                                             ` +
                    `<div v-else>                                                                       ` +
                    `   <a @click="pz()">` +
                    `       <img :src="srcUrl" style="width: 32%;" />` +
                    `   </a>` +
                    `</div>`,
            props: ["local-ids", "is-more"],
            data: function () {
                return {

                }
            },
            methods: {
                gtouchstart: function (localId) {
                    timeOutEvent = setTimeout("removeImage('" + localId + "',this)", 500);//这里设置定时器，定义长按500毫秒触发长按事件，时间可以自己改，个人感觉500毫秒非常合适
                    return false;
                },
                gtouchend: function () {
                    clearTimeout(timeOutEvent);
                    return false;
                },
                gtouchmove: function () {
                    clearTimeout(timeOutEvent);//清除定时器
                    timeOutEvent = 0;

                },
                pz: function () {
                    var that = this;
                    var count = that.isMore ? 9 - that.localIds.length : 1;
                    wx.chooseImage({
                        count: count,
                        sizeType: ['original', 'compressed'],
                        sourceType: ['album', 'camera'],
                        success: function (res) {
                            if (that.isMore) {
                                res.localIds.forEach((local) => {
                                    that.localIds.push(local);
                                })
                            } else {
                                that.localIds = res.localIds;
                            }

                        }
                    });
                }
            },
            computed: {
                srcUrl: function () {
                    if (!this.isMore) {
                        if (this.localIds.length <= 0) {
                            return "../images/templates/bottommenu/218.png"
                        } else {
                            return this.localIds[0]
                        }
                    }
                }
            },
        });

        getsubfamily = function () {
            var value = $("#subtype").val();
            $.ajax({
                url: "../Ajax/sys_SubscriberFamilyManager.ashx?type=getsubfamily",
                type: "post",
                data: {
                    value: value,
                    openId: localStorage.getItem("openId")
                },
                success: function (d) {
                    var obj = JSON.parse(d);
                    var html = "";
                    obj.forEach((o, i) => {
                        html += `<option ${i == 0 ? " selected" : ""} value="${o.key}">${o.value}</option>`;
                    });
                    $("#subfamily").html(html);
                }, error: function (d) {
                    console.log(d);
                }
            });
        }

        function removeImage(localId, vm) {
            timeOutEvent = 0;
            var ind = vm.localIds.indexOf(localId);
            if (ind > -1) {
                vm.localIds.splice(ind, 1);
            }
        }

        var vm = new Vue({
            el: "#app",
            data: {
                localIds: ["123"],
                glocalId: ["456"] // 整体图只能有一个
            },
        });

        submit = function (callback) {
            if (vm.localIds.length <= 0) {
                alert("请上传照片");
                return;
            }
            var paths = [];
            vm.localIds.forEach((localId) => {
                wx.uploadImage({
                    localId: localId.toString(),
                    isShowProgressTips: 1,
                    success: function (res) {
                        $.ajax({
                            url: "../Ajax/weixinInfo.ashx",
                            type: "post",
                            data: {
                                type: "downFile",
                                mediaId: res.serverId,
                                openId: localStorage.getItem("openId")
                            },
                            success: function (d) {
                                paths.push(d);
                                if (paths.length == vm.localIds.length) {
                                    if (vm.glocalId.length > 0) {
                                        wx.uploadImage({
                                            localId: vm.glocalId[0].toString(),
                                            isShowProgressTips: 1,
                                            success: function (gres) {
                                                $.ajax({
                                                    url: "../Ajax/weixinInfo.ashx",
                                                    type: "post",
                                                    data: {
                                                        type: "downFile",
                                                        mediaId: gres.serverId,
                                                        openId: localStorage.getItem("openId")
                                                    },
                                                    success: function (gd) {
                                                        paths.push(gd);
                                                    },
                                                    error: function (gd) {
                                                        console.log(gd);
                                                    }
                                                });
                                            }
                                        });
                                    }
                                    wx.getLocation({
                                        type: 'wgs84',
                                        success: function (res) {
                                            callback(paths, xpoint, ypoint);
                                        }
                                    });
                                }
                            }, error: function (d) {
                                console.log(d);
                            }
                        });
                    }
                });
            });
        };
    });

