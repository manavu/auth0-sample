import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'

import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
// Install BootstrapVue
Vue.use(BootstrapVue)
// Optionally install the BootstrapVue icon components plugin
Vue.use(IconsPlugin)

import VueCookies from 'vue-cookies'
Vue.use(VueCookies)

import Axios from 'axios'
Vue.prototype.$axios = Axios

Vue.config.productionTip = false

// Import the Auth0 configuration
import { domain, clientId } from '../auth_config.json'
// Import the plugin here
import { Auth0Plugin } from './auth'

// Install the authentication plugin here
Vue.use(Auth0Plugin, {
  domain,
  clientId,
  audience: 'https://localhost:8080/api', // APIs の Identifier
  onRedirectCallback: (appState) => {
    router.push(
      appState && appState.targetUrl
        ? appState.targetUrl
        : window.location.pathname
    )
  },
})

// npm install babel-plugin-transform-inline-environment-variables --save-dev
// このプラグインを使うことで環境変数を使うことができるが、Vue cli で作ると標準ではいるっぽい
// 環境変数を読み取れる。
console.log(process.env.VUE_APP_API_URL)

new Vue({
  router,
  store,
  render: (h) => h(App),
}).$mount('#app')
