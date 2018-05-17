//plugin
Vue.use(VueI18n)

// set lang
Vue.config.lang = language

// set locales
Object.keys(locales).forEach(function (lang) {
    Vue.locale(lang, locales[lang])
})

var MyPrintVue = new Vue({
    el: '#printable',
    data: {
        selectedResult: selectedResult,
        details: details
    },
    methods: {
        printMe: function () {
            window.print();
        }

    },
    filters: {
        moment: function (date) {
            return moment(date).isValid() ? moment(date).format('MMMM Do YYYY, h:mm a') : '';

        },
        momentMod: function (date) {
            return moment(date).isValid() ? moment(date).format('MMMM Do YYYY') : '';

        }
    }
});

MyPrintVue.printMe();