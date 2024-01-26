import 'beercss'
import 'material-dynamic-colors'
import 'affolternet-vue3-library/css'
import '@/assets/styles.scss'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

import { AnLoader, AnToast, AnToasts, loaderService } from 'affolternet-vue3-library'
import { toastService } from 'affolternet-vue3-library'

const app = createApp(App)

app.component('AnLoader', AnLoader)
app.component('AnToasts', AnToasts)
app.component('AnToast', AnToast)

app.provide('loaderService', loaderService)
app.provide('toastService', toastService)

app.use(createPinia())
app.use(router)

app.mount('#app')
