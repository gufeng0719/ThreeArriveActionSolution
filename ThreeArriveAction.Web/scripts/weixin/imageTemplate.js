Vue.component("image-template", {
    template:
            `<div v-if="isMore">                                                               ` +
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
            timeOutEvent: 0
        }
    },
    methods: {
        gtouchstart: function (localId) {
            this.timeOutEvent = setTimeout("removeImage('" + localId + "')", 500);//这里设置定时器，定义长按500毫秒触发长按事件，时间可以自己改，个人感觉500毫秒非常合适
            return false;
        },
        gtouchend: function () {
            clearTimeout(this.timeOutEvent);
            return false;
        },
        gtouchmove: function () {
            clearTimeout(this.timeOutEvent);//清除定时器
            this.timeOutEvent = 0;

        },
        pz: function () {
            var that = vm;
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


function removeImage(localId) {
    vm.timeOutEvent = 0;
    var ind = vm.localIds.indexOf(localId);
    if (ind > -1) {
        vm.localIds.splice(ind, 1);
    }
}





wx.getLocation({
    type: 'wgs84', // 默认为wgs84的gps坐标，如果要返回直接给openLocation用的火星坐标，可传入'gcj02'
    success: function (res) {
        var latitude = res.latitude; // 纬度，浮点数，范围为90 ~ -90
        var longitude = res.longitude; // 经度，浮点数，范围为180 ~ -180。
        var speed = res.speed; // 速度，以米/每秒计
        var accuracy = res.accuracy; // 位置精度
    }
});

