<template lang="pug">
h3 Letter
.contents(v-if="letterJson")
  b Font-Size: 
  span {{ letterJson.fontSize }}
  br
  b Text:
  span {{ letterJson.text }}
  br
  b Start-Base: 
  div X: {{ letterJson.startBaseLine.X }}, Y: {{ letterJson.startBaseLine.Y }}
</template>

<script lang="ts" setup>

import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { letterJson } = useViewSettings()

</script>

<style lang="scss" scoped>

.contents {
  background: white;
}

</style>
