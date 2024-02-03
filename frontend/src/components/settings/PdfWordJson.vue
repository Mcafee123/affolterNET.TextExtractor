<template lang="pug">
.word(v-if="wordJson")
  .row.first
    BoundingBox(v-if="wordJson.boundingBox" :block="wordJson.boundingBox")
  .row.first
    .title 
      b Text:
    .text {{ wordJson.text }}
  .row
    .title
      b Font:
    .font {{  wordJson.fontName }}
  .row
    .title 
      b Orient.:
    .ori {{  wordJson.orientation }}
  .row 
    .title 
      b BaseLine:
    .bly {{ round(wordJson.baseLineY) }}
</template>

<script lang="ts" setup>

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import BoundingBox from '@/components/settings/BoundingBox.vue'
import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { wordJson, round } = useViewSettings()

</script>

<style lang="scss" scoped>

.title {
  min-width: 40px;
}

</style>