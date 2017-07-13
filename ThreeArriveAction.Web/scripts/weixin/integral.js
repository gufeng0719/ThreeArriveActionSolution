require(['/scripts/vue/vue.js', '/scripts/jquery/jquery-1.10.2.min.js'],
    function (Vue) {

        var vm = new Vue({
            el: "#app",
            data: {
                table: {
                    page: 0,
                    total: 0,
                    data: []
                }
            },
        });
});