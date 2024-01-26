<template lang="pug">
article
  h5 Line
  .contents(v-if="lineJson")
    BoundingBox(v-if="lineJson.boundingBox" :block="lineJson.boundingBox")
    b Text:
    .text {{ lineJson.text }}
</template>

<script lang="ts" setup>

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import BoundingBox from '@/components/settings/BoundingBox.vue'
import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { lineJson } = useViewSettings()

</script>

<style lang="scss" scoped>

</style>