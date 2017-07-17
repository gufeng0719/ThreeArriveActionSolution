
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