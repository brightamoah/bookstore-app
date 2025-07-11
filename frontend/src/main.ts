import './assets/main.css'

import { createApp } from 'vue'
import { createPinia, type PiniaPlugin } from 'pinia'
import ui from '@nuxt/ui/vue-plugin'
import { autoAnimatePlugin } from '@formkit/auto-animate/vue'
import { createPersistedState } from 'pinia-plugin-persistedstate'
import App from './App.vue'
import router from './router'

const app = createApp(App)
const pinia = createPinia()

pinia.use(
  createPersistedState({
    storage: sessionStorage,
  }),
)
app.use(pinia)
app.use(router)
app.use(ui)
app.use(autoAnimatePlugin)

app.mount('#app')
