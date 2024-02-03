<template lang="pug">
.line(v-if="lineJson")
  .row.first
    BoundingBox(v-if="lineJson.boundingBox" :block="lineJson.boundingBox")
  .row.first.wrap
    .title 
      b Text:
    .txt {{ lineJson.text }}
  .row.first 
    .title 
      b TopDist:
    .txt {{ round(lineJson.topDistance) }}
  .row 
    .title 
      b FontSizeAvg:
    .fsa {{ round(lineJson.fontSizeAvg) }}
  .row 
    .title 
      b BaseLine:
    .bly {{ round(lineJson.baseLineY) }}
  .row.first
    button(@click="drawBox()" alt="Box zeichnen")
      i draw
</template>

<script lang="ts" setup>

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import BoundingBox from '@/components/settings/BoundingBox.vue'
import { useViewSettings } from '@/composables/useViewSettings'
import type { Box } from '@/types/boundingBox';

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { lineJson, round, customBox } = useViewSettings()

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const drawBox = () => {
  const box = lineJson.value?.boundingBox as Box
  if (!box) {
    return
  }
  customBox.value.bottomLeftX = box.bottomLeftX
  customBox.value.bottomLeftY = box.bottomLeftY
  customBox.value.topRightX = box.topRightX
  customBox.value.topRightY = box.topRightY
  customBox.value.shadow = false
}

</script>

<style lang="scss" scoped>

.title {
  min-width: 90px;
}

</style>