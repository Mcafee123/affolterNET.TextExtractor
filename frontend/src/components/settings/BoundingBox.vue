<template lang="pug">
b Box:
.bottomLeft
  .row
    .title Bottom-Left: 
    .x X: {{ round(block.bottomLeftX) }}
    .y Y: {{ round(block.bottomLeftY) }}
.topRight 
  .row 
    .title Top-Right: 
    .x X: {{ round(block.topRightX) }}
    .y Y: {{ round(block.topRightY) }}
</template>

<script lang="ts" setup>
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
const round = (inp: number) => Math.round(inp * 100) / 100

</script>

<style lang="scss" scoped>
.bottomLeft {
  display: table;
}

.topRight, .bottomLeft {
  .title {
    min-width: 70px;
  }
  .x {
    min-width: 40px;
  }
}

</style>
