<template lang="pug">
.block(:style="getStyle()" :class="showBlockBorders ? 'blockborder' : ''")
PdfLine(v-for="line in block.lines" :line="line" :pageheight="pageheight" @selected="select")

</template>

<script lang="ts" setup>

import { watch, type PropType } from 'vue'
import type { Block } from '@/types/block'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfLine from '@/components/PdfLine.vue'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  block: {
    required: true,
    type: Object as PropType<Block>
  },
  pageheight: {
    required: true,
    type: Number
  }
})

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { showBlockBorders, blockJson } = useViewSettings()

watch(showBlockBorders, (value) => {
  console.log('changed', value)
})
const boundingBox = props.block.boundingBox;
const width = boundingBox.topRightX - boundingBox.bottomLeftX
const height = boundingBox.topRightY - boundingBox.bottomLeftY
const top = props.pageheight - boundingBox.topRightY
const left = boundingBox.bottomLeftX
const parts: string[] = []
parts.push(`width: ${width}px`)
parts.push(`height: ${height}px`)
parts.push(`top: ${top}px`)
parts.push(`left: ${left}px`)

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const getStyle = () => {
  return parts.join('; ')
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const select = (evt: MouseEvent) => {
  blockJson.value = { boundingBox: props.block.boundingBox, text: props.block.lines.map(l => l.words.map(w => w.text).join('')).join('') }
}

</script>

<style lang="scss" scoped>

.block {
  position: absolute;
}

.blockborder {
  border: 1px solid red;
}

</style>