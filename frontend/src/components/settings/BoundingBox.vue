<template lang="pug">
b Box:
.bottomLeft Bottom-Left: {{ block.bottomLeftX }}, {{ block.bottomLeftY }}
.topRight Top-Right: {{ block.topRightX }}, {{ block.topRightY }}
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

</script>

<style lang="scss" scoped>

</style>
