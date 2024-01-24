<template lang="pug">

.line(:style="getStyle()" :class="showLineBorders ? 'lineborder' : ''")
PdfWord(v-for="word in line.words" :word="word" :pageheight="pageheight" @selected="select")

</template>

<script lang="ts" setup>

import type { PropType } from 'vue'
import type { Line } from '@/types/line'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfWord from '@/components/PdfWord.vue'
import { useViewSettings } from '@/composables/useViewSettings'

const emit = defineEmits(['selected'])

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  line: {
    required: true,
    type: Object as PropType<Line>
  },
  pageheight: {
    required: true,
    type: Number
  }
})

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { showLineBorders, lineJson } = useViewSettings()

const boundingBox = props.line.boundingBox;
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
  lineJson.value = { boundingBox: props.line.boundingBox, text: props.line.words.map(w => w.text).join('') }
  emit('selected')
}

</script>

<style lang="scss" scoped>

.line {
  position: absolute;
  box-sizing: unset;
}

.lineborder {
  border: 1px solid blue;
}

</style>