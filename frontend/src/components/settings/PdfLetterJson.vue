<template lang="pug">
.letter(v-if="letterJson")
  .row.first
    .title 
      b Font-Size: 
    .font {{ letterJson.fontSize }}
  .row
    .title 
      b Text:
    .txt {{ letterJson.text }}
  .row
    .title 
      b Orientation:
    .ori {{  letterJson.orientation }}
  .row
    .title 
      b Start-Base: 
    .xy X: {{ round(letterJson.startBaseLine.X) }}, Y: {{ round(letterJson.startBaseLine.Y) }}

</template>

<script lang="ts" setup>

import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { letterJson, round } = useViewSettings()

</script>

<style lang="scss" scoped>

.letter {
  display: table;
}

.title {
  min-width: 70px;
}

</style>
