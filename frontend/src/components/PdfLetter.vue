<template lang="pug">

.letter(:style="getStyle()" :class="showLetterBorders ? 'letterborder' : ''" @mouseenter="enter($event)" @mouseleave="leave($event)" @click="select($event)") {{ letter.text }}

</template>

<script lang="ts" setup>

import type { PropType } from 'vue'
import type { Letter } from '@/types/letter'
import { useViewSettings } from '@/composables/useViewSettings'

const emit = defineEmits(['selected'])

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  letter: {
    required: true,
    type: Object as PropType<Letter>
  },
  fontName: {
    required: true,
    type: String
  },
  isBold: {
    required: true,
    type: Boolean
  },
  isItalic: {
    required: true,
    type: Boolean
  },
  pageheight: {
    required: true,
    type: Number
  }
})

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { showLetterBorders, letterJson } = useViewSettings()

const startBaseLine = props.letter.startBaseLine;
const endBaseLine = props.letter.endBaseLine;
const width = endBaseLine.X - startBaseLine.X
const height = props.letter.glyphRectangle.topRightY - props.letter.glyphRectangle.bottomLeftY
const top = (props.pageheight - startBaseLine.Y) - props.letter.fontSize
const left = startBaseLine.X
const parts: string[] = []
parts.push(`width: ${width}px`)
parts.push(`height: ${height}px`)
parts.push(`top: ${top}px`)
parts.push(`left: ${left}px`)
parts.push(`font-family: ${props.fontName}`)
parts.push(`font-weight: ${props.isBold ? 'bold' : 'normal'}`)
parts.push(`font-size: ${props.letter.fontSize}px`)

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const getStyle = () => {
  return parts.join('; ')
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const enter = (evt: MouseEvent) => {
  if (evt.target instanceof HTMLElement) {
    const scalefactor = 3
    evt.target.style.setProperty('background-color', 'yellow')
    evt.target.style.setProperty('height', `${height * scalefactor}px`)
    evt.target.style.setProperty('width', `${width * scalefactor}px`)
    evt.target.style.setProperty('font-size', `${props.letter.fontSize * scalefactor}px`)
    evt.target.style.setProperty('top', `${top - ((scalefactor / 2) * height)}px`)
    evt.target.style.setProperty('left', `${left - ((scalefactor / 4) * width)}px`)
  }
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const leave = (evt: MouseEvent) => {
  if (evt.target instanceof HTMLElement) {
    evt.target.style.setProperty('background-color', 'transparent')
    evt.target.style.setProperty('height', `${height}px`)
    evt.target.style.setProperty('width', `${width}px`)
    evt.target.style.setProperty('font-size', `${props.letter.fontSize}px`)
    evt.target.style.setProperty('top', `${top}px`)
    evt.target.style.setProperty('left', `${left}px`)
  }
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const select = (evt: MouseEvent) => {
  leave(evt)
  letterJson.value = { fontSize: props.letter.fontSize, text: props.letter.text, startBaseLine: props.letter.startBaseLine }
  emit('selected')
}

</script>

<style lang="scss" scoped>

.letter {
  position: absolute;
  cursor: pointer;
  transition: all 0.1s ease-in-out;
  box-sizing: unset;
}

.letterborder {
  border: 0.5px solid violet;
}

</style>