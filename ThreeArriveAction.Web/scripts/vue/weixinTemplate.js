
Vue.component("ddlsee", {
    template:
        '<div>' +
        '   <select v-model="type">                                                                    ' +
        '      <option value="-1">-请选择-</option>                                                     ' +
        '      <option v-for="model in typeList" :value="model.key">{{model.value}}</option>           ' +
        '   </select>                                                                                  ' +
        '   <select v-model="myfamily" v-show="type!=8">                                               ' +
        '      <option selected v-for="model in familyList" :value="model.key">{{model.value}}</option>' +
        '   </select>' +
        '</div>',
    props: ["family"],
    data: function () {
        return {
            familyList: [],
            typeList: [
                {
                    key: 1,
                    value: "留守老人户"
                }, {
                    key: 2,
                    value: "重大病户"
                }, {
                    key: 3,
                    value: "五保户"
                }, {
                    key: 4,
                    value: "低保户"
                }, {
                    key: 5,
                    value: "离任村干部户"
                }, {
                    key: 6,
                    value: "老党员户"
                }, {
                    key: 7,
                    value: "信访户"
                }
            ],
            type: -1,
            myfamily: -1
        }
    },
    watch: {
        type: function (newValue) {
            var that = this;
            $.ajax({
                url: "../Ajax/sys_SubscriberFamilyManager.ashx?type=getsubfamily",
                type: "post",
                data: {
                    value: newValue,
                    openId: localStorage.getItem("openId")
                },
                success: function (d) {
                    var obj = JSON.parse(d);
                    that.familyList = obj;
                    if (that.familyList.length > 0) {
                        that.myfamily = that.familyList[0].key;
                    }
                }, error: function (d) {
                    console.log(d);
                }
            });
            if (that.$parent.familyType) {
                that.$parent.familyType = newValue;
            }
        },
        myfamily: function (newValue) {
            vm.family = newValue;
        }
    }
});

Vue.component("ddlvillage", {
    template:
        '<div>                                                                                  ' +
            '    <select v-model="ddltown"                                                          ' +
            '       style="height: 30px; border: 1px solid #eeeeee;padding-left: 5px; width: 40%;"> ' +
            '        <option value="-1">-请选择-</option>                                            ' +
            '        <option v-for="t in townlist" v-bind:value="t.value">{{t.text}}</option>       ' +
            '    </select>                                                                          ' +
            '    <select v-model="myddlvillage"                                                       ' +
            '       style="height: 30px; border: 1px solid #eeeeee;padding-left: 5px; width: 40%;"> ' +
            '        <option value="-1">-请选择-</option>                                            ' +
            '        <option v-for="v in villagelist" v-bind:value="v.value">{{v.text}}</option>    ' +
            '    </select>                                                                          ' +
            '</div>',
    props: ["ddlvillage"],
    data: function () {
        return {
            ddltown: -1,
            townlist: [],
            villagelist: [],
            allvillage: [],
            myddlvillage: this.ddlvillage
        }
    },
    watch: {
        "ddltown": function (newValue) {
            if (newValue === -1) {
                return;
            }
            var that = this;
            var child = _.find(that.allvillage, function (v) {
                return v.parent.value === newValue;
            });
            if (child) {
                that.villagelist = child.child;
            }
            that.myddlvillage = -1;
        },
        "myddlvillage": function (newValue) {
            vm.ddlvillage = newValue;
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
    }
});

Vue.component("image-template", {
    template:
            '<div v-if="isMore">                                                               ' +
            '    <div v-for="(localId,index) in localIds" style="display:inline"               ' +
            '       @dblclick="removeImg(localId)">                                            ' +
            '       <a :data-id="localId">                                                     ' +
            '           <img :src="localId" style="width: 28%;" />                             ' +
            '       </a>                                                                       ' +
            '    </div>                                                                        ' +
            '    <a @click="pz()" v-if="localIds.length < 9">                                  ' +
            '        <img src="../images/templates/bottommenu/218.png" style="width: 28%;" />  ' +
            '    </a>                                                                          ' +
            '</div>                                                                            ' +
            '<div v-else>                                                                      ' +
            '   <a @click="pz()">                                                              ' +
            '       <img :src="srcUrl" style="width: 28%;" />                                  ' +
            '   </a>                                                                           ' +
            '</div>                                                                            ',
    props: ["local-ids", "is-more"],
    data: function () {
        return {
        }
    },
    methods: {
        pz: function () {
            var that = this;
            var count = that.isMore ? 9 - vm.localIds.length : 1;
            window.wx.chooseImage({
                count: count,
                sizeType: ['original', 'compressed'],
                sourceType: ['album', 'camera'],
                success: function (res) {
                    if (that.isMore) {
                        res.localIds.forEach((local) => {
                            vm.localIds.push(local);
                        });
                    } else {
                        if (typeof (vm.needglocalId) != "undefined" && vm.needglocalId === true) {
                            vm.glocalId = res.localIds;
                        } else {
                            vm.localIds = res.localIds;
                        }
                    }

                }
            });
        },
        removeImg: function (id) {
            vm.localIds = window._.remove(vm.localIds, function (localId) {
                return localId !== id;
            });
        }
    },
    computed: {
        srcUrl: function () {
            if (!this.isMore) {
                if (this.localIds.length <= 0) {
                    return "../images/templates/bottommenu/218.png";
                } else {
                    return this.localIds[0];
                }
            }
            return "../images/templates/bottommenu/218.png";
        }
    }
});