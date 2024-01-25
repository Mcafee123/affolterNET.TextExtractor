<template lang="pug">
h3 Block
.contents(v-if="blockJson")
  BoundingBox(v-if="blockJson.boundingBox" :block="blockJson.boundingBox")
  b Text:
  .text {{ blockJson.text }}
</template>

<script lang="ts" setup>

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import BoundingBox from '@/components/settings/BoundingBox.vue'
import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { blockJson } = useViewSettings()

</script>

<style lang="scss" scoped>

.contents {
  background: white;
}
</style>