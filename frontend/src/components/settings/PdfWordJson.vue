<template lang="pug">
article
  h5 Word
  .contents(v-if="wordJson")
    BoundingBox(v-if="wordJson.boundingBox" :block="wordJson.boundingBox")
    b Text:
    .text {{ wordJson.text }}
    b Font:
    .font {{  wordJson.fontName }}
</template>

<script lang="ts" setup>

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import BoundingBox from '@/components/settings/BoundingBox.vue'
import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { wordJson } = useViewSettings()

</script>

<style lang="scss" scoped>

</style>