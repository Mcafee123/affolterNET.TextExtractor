<template lang="pug">
header.fill
  nav
    RouterLink(to="/")
      i home
      div Home
    RouterLink(to="/documents")
      i picture_as_pdf 
      div PDF
    RouterLink(to="/upload")
      i cloud_upload
      div Upload
    h5(class="max center-align") ...
    button(class="circle transparent")
      img.responsive(src="/favicon-32x32.png")
an-loader(:height="3" :color="loadercolor")
main.responsive.max
  RouterView

an-toasts(
  :close-icon="{ class: 'extra', content: 'close' }"
  error-classes="an-toast-default extractor-toast extractor-error top" 
  :error-icon="{ class: 'extra', content: 'priority_high' }"
  warn-classes="an-toast-default extractor-toast extractor-warning top" 
  :warn-icon="{ class: 'extra', content: 'warning' }"
  info-classes="an-toast-default extractor-toast extractor-info top" 
  :info-icon="{ class: 'extra', content: 'info' }"
  done-classes="an-toast-default extractor-toast extractor-done bottom" 
  :done-icon="{ class: 'extra', content: 'done' }"
)
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'

import { useViewSettings } from '@/composables/useViewSettings'

const { getCssVar } = useViewSettings()

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const loadercolor = ref<string>('#4a8df8')

onMounted(async () => {
  document.body.classList.add('dark')
  await ui('theme', '/favicon-32x32.png')
  loadercolor.value = getCssVar('--primary')
})

</script>

<style lang="scss" scoped>

main.responsive.max {
    padding: 0;
}

.block {
    position: absolute;
    border: 1px solid gray;
}

</style>
