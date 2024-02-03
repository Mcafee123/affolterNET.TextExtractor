<template lang="pug">
.box
  .boxSize
    .row
      .title
        b Size: 
      .x W: {{ round(block.topRightX - block.bottomLeftX) }}
      .y H: {{ round(block.topRightY - block.bottomLeftY) }}
  .bottomLeft
    .row
      .title
        b Bottom-Left: 
      .x X: {{ round(block.bottomLeftX) }}
      .y Y: {{ round(block.bottomLeftY) }}
  .topRight 
    .row 
      .title
        b Top-Right: 
      .x X: {{ round(block.topRightX) }}
      .y Y: {{ round(block.topRightY) }}
</template>

<script lang="ts" setup>
import { useViewSettings } from '@/composables/useViewSettings'
import type { blockType } from '@/types/block'
import type { PropType } from 'vue'


// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  block: {
    required: true,
    type: Object as PropType<blockType>
  }
})

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { round } = useViewSettings()

</script>

<style lang="scss" scoped>

.box {
  display: table;
}
.title {
  min-width: 75px;
}
.x {
  min-width: 70px;
}

</style>
