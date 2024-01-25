<template lang="pug">
h3 Word
.contents(v-if="wordJson")
  BoundingBox(v-if="wordJson.boundingBox" :block="wordJson.boundingBox")
  b Text:
  .text {{ wordJson.text }}
</template>

<script lang="ts" setup>

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import BoundingBox from '@/components/settings/BoundingBox.vue'
import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { wordJson } = useViewSettings()

</script>

<style lang="scss" scoped>

.contents {
  background: white;
}

</style>