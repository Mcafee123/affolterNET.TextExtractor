<template lang="pug">
header.fill
  nav
    RouterLink(to="/")
      i home
      div Home
    RouterLink(to="/pdf")
      i picture_as_pdf 
      div PDF
    h5(class="max center-align") ...
    button(class="circle transparent")
      img.responsive(src="/favicon-32x32.png")
an-loader(:height="3" :color="loadercolor")
main.responsive.max
  RouterView

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
