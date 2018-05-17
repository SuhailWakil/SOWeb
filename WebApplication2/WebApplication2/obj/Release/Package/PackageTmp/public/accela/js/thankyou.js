var MyPrintVue = new Vue({
    el: '#printable',
    data: {
        submitObject: submitObject
    },
    methods: {
        printMe: function () {
            window.print();
        }

    },
    filters: {

    }
});
