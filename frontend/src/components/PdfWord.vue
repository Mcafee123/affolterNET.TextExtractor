<template lang="pug">

.word(:style="getStyle()" :class="showWordBorders ? 'wordborder' : ''")
PdfLetter(v-for="letter in word.letters" :letter="letter" :fontName="font" :isItalic="italic" :isBold="bold" :pageheight="pageheight" @selected="select")

</template>

<script lang="ts" setup>

import { ref, type PropType } from 'vue'
import type { Word } from '@/types/word'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfLetter from '@/components/PdfLetter.vue'
import { useViewSettings } from '@/composables/useViewSettings'

const emit = defineEmits(['selected'])

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  word: {
    required: true,
    type: Object as PropType<Word>
  },
  pageheight: {
    required: true,
    type: Number
  }
})

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { showWordBorders, wordJson } = useViewSettings()

const boundingBox = props.word.boundingBox;
const width = boundingBox.topRightX - boundingBox.bottomLeftX
const height = boundingBox.topRightY - boundingBox.bottomLeftY
const top = props.pageheight - boundingBox.topRightY
const left = boundingBox.bottomLeftX
const parts: string[] = []
parts.push(`width: ${width}px`)
parts.push(`height: ${height}px`)
parts.push(`top: ${top}px`)
parts.push(`left: ${left}px`)

const font = ref<string>('Times New Roman, Times, serif')
if (props.word.fontName.indexOf('Arial') > -1) {
  font.value = 'Arial, Helvetica, sans-serif'
}
let bold = ref<boolean>(false)
if (props.word.fontName.indexOf('bold') > -1) {
  bold.value = true
}
let italic = ref<boolean>(false)
if (props.word.fontName.indexOf('italic') > -1) {
  italic.value = true
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const getStyle = () => {
  return parts.join('; ')
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const select = (evt: MouseEvent) => {
  wordJson.value = { boundingBox: props.word.boundingBox, text: props.word.text, fontName: props.word.fontName }
  emit('selected')
}

</script>

<style lang="scss" scoped>

.word {
  position: absolute;
  box-sizing: unset;
}

.wordborder {
  border: 0.5px solid green;
}

</style>