
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
                    value: "重大病灾户"
                }, {
                    key: 3,
                    value: "五保户"
                }, {
                    key: 4,
                    value: "建党立卡低收入户"
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
            myfamily: -1,
            allSubfamily: []
        }
    },
    methods: {
        getAllSubfamily: function () {
            var that = this;
            $.ajax({
                url: "../Ajax/sys_SubscriberFamilyManager.ashx?type=getsubfamily",
                type: "post",
                data: {
                    openId: localStorage.getItem("openId"),
                    isThing: location.href.indexOf("thing") > -1
                },
                success: function (d) {
                    that.allSubfamily = JSON.parse(d);
                    that.type = that.allSubfamily[0].type;
                }, error: function (d) {
                    console.log(d);
                }
            });

        },
        getlist: function (type) {
            var that = this;
            var list = _.where(that.allSubfamily, { type: type });
            that.familyList = list;
            if (that.familyList && that.familyList.length > 0) {
                that.myfamily = that.familyList[0].key;
            }
        }
    },
    watch: {
        type: function (newValue) {
            var that = this;
            that.getlist(newValue);
            if (that.$parent.familyType) {
                that.$parent.familyType = newValue;
            }
        },
        myfamily: function (newValue) {
            vm.family = newValue;
        },
        allSubfamily: function (newValue) {

        }
    },
    mounted: function () {
        this.getAllSubfamily();
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
            var that = this;
            if (newValue < 1) {
                that.villagelist = [];
                that.myddlvillage = -1;
                return;
            }
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
                openId: localStorage.getItem("openId")
            },
            complete: function (d) {
                var obj = JSON.parse(d.responseText);
                that.allvillage = obj.list;
                obj.list.forEach((o) => {
                    that.townlist.push(o.parent);
                });
                if (obj.current > 0) {
                    var item;
                    obj.list.forEach((n) => {
                        var temp = _.find(n.child, function (c) {
                            return c.value === obj.current
                        });
                        if (temp) {
                            item = n;
                        }
                    });

                    if (item) {
                        that.ddltown = item.parent.value;
                        setTimeout(function () {
                            that.myddlvillage = obj.current;
                        }, 300)
                    }
                }
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
            '           <img :src="localId" style="width: 15%;" />                             ' +
            '       </a>                                                                       ' +
            '    </div>                                                                        ' +
            '    <a @click="pz()" v-if="localIds.length < 9">                                  ' +
            '        <img src="../images/templates/bottommenu/218.png" style="width: 15%;" />  ' +
            '    </a>                                                                          ' +
            '</div>                                                                            ' +
            '<div v-else>                                                                      ' +
            '   <a @click="pz()">                                                              ' +
            '       <img :src="srcUrl" style="width: 15%;" />                                  ' +
            '   </a>                                                                           ' +
            '</div>',
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

Vue.component('tw-item', {
    template:
        '<td colspan="2">' +
        '   <tr class="row">' +
        '       <td>图文标题 :&nbsp;&nbsp;&nbsp;</td>                                                                ' +
        '       <td>                                                                                                ' +
        '           <input type="text" v-model="tw.title" />                                                        ' +
        '           <el-tooltip class="btn-add delete" v-if="twlength > 1"                                          ' +
        '                       effect="dark" content="删除这组图文" placement="right">                               ' +
        '               <el-button @click="deleteTw(ind)">-</el-button>                                             ' +
        '           </el-tooltip>                                                                                   ' +
        '       </td>                                                                                               ' +
        '   </tr>                                                                                                   ' +
        '   <tr class="row">                                                                                        ' +
        '       <td>选择封面 :&nbsp;&nbsp;&nbsp;</td>                                                                ' +
        '       <td>                                                                                                ' +
        '           <form method="post" enctype="multipart/form-data">                                              ' +
        '               <input type="file" v-bind:id="\'file_\' + ind" accept="image/gif, image/jpeg"               ' +
        '                   @change="uploadFile(\'file_\' + ind)" v-loading.fullscreen.lock="fullscreenLoading"/>   ' +
        '               <br />                                                                                      ' +
        '               <span class="hint">支持PNG\JPEG\JPG\GIF格式 不能超过2M</span>                                  ' +
        '               <input type="button" v-bind:value="\'上传\'" class="btn"                                     ' +
        '                  style="display:none"                                                                     ' +
        '                  @click="uploadFile(\'file_\' + ind)" v-loading.fullscreen.lock="fullscreenLoading" />    ' +
        '           </form>                                                                                         ' +
        '       </td>                                                                                               ' +
        '   </tr>                                                                                                   ' +
        '   <tr class="row">                                                                                        ' +
        '       <td>图文内容 :&nbsp;&nbsp;&nbsp;</td>                                                                ' +
        '       <td>                                                                                                ' +
        '           <textarea style="height: 60px;width: 247px;" v-model="tw.msg"></textarea>                       ' +
        '       </td>                                                                                               ' +
        '   </tr>                                                                                                   ' +
        '   <tr class="row" v-if="ind != twlength-1">                                                               ' +
        '       <td colspan="2">                                                                                    ' +
        '           <div style="border-top: 1px solid black;width: 105%">                                           ' +
        '                                                                                                           ' +
        '           </div>                                                                                          ' +
        '       </td>                                                                                               ' +
        '   </tr>' +
        '</td>',
    props: ["tw", "ind", "twlength"],
    data: function () {
        return {
            fullscreenLoading: false
        }
    },
    methods: {
        deleteTw: function (index) {
            var that = vm;
            var temp = [];
            for (var i = 0; i < that.twList.length; i++) {
                if (i !== index) {
                    temp.push({
                        title: that.twList[i].title,
                        msg: that.twList[i].msg
                    });
                }
            }
            that.twList = temp;
        },
        uploadFile: function (name) {
            var that = this;
            if (!$('#' + name).val()) {
                alert("请先选择需要上传的文件");
                return;
            }
            if (that.tw.yetpath === $('#' + name).val() && that.tw.path) {
                alert("请勿重复上传");
                return;
            }
            that.fullscreenLoading = true;
            var formData = new FormData();
            formData.append('file', $('#' + name)[0].files[0]);
            $.ajax({
                url: '../Ajax/upload_ajax.ashx?action=UpLoadMsgFile',
                type: 'POST',
                cache: false,
                data: formData,
                timeout: 90000,
                processData: false,
                contentType: false
            }).done(function (res) {
                if (res === "网络异常,请重新尝试上传") {
                    alery(res);
                    return;
                }
                that.tw.path = res;
                that.tw.yetpath = $('#' + name).val()
                that.fullscreenLoading = false;
                alert("上传成功")
            }).fail(function (res) {
                that.fullscreenLoading = false;
                alert(res);
            });
        }
    },
    computed: {
    }
})