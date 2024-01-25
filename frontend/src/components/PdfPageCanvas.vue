<template lang="pug">
.wrapper(ref="wrapper")
  canvas(ref="letterCanvas" @mousemove="highlight($event)" @click="select($event)")
  canvas(ref="boxesCanvas")
</template>

<script lang="ts" setup>

import { useViewSettings } from '@/composables/useViewSettings';
import type { Box } from '@/types/boundingBox';
import type { Page } from '@/types/page';
import { onMounted, ref, type PropType, type Ref, onUnmounted, watch } from 'vue'

const wrapper: Ref<HTMLDivElement | undefined> = ref()
const letterCanvas: Ref<HTMLCanvasElement | undefined> = ref()
const boxesCanvas: Ref<HTMLCanvasElement | undefined> = ref()
const letterCtx = ref<CanvasRenderingContext2D>()
const boxesCtx = ref<CanvasRenderingContext2D>()

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  page: {
    required: true,
    type: Object as PropType<Page>
  }
})

const clearCanvas = (mode?: 'letter' | 'boxes') => {
  if (!letterCanvas.value || !boxesCanvas.value || !letterCtx.value || !boxesCtx.value) {
    console.error('no canvas or no contexts')
    return
  }
  if (mode === 'letter' || mode == undefined) {
    letterCtx.value.clearRect(0, 0, letterCanvas.value.width, letterCanvas.value.height)
  }
  if (mode === 'boxes' || mode == undefined) {
    boxesCtx.value.clearRect(0, 0, boxesCanvas.value.width, boxesCanvas.value.height)
  }
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { showBlockBorders, showLineBorders, showWordBorders, showLetterBorders } = useViewSettings()

let pageWidth = props.page.boundingBox.topRightX
let pageHeight = props.page.boundingBox.topRightY
let scale = 1

watch(() => props.page, () => {
  if (!letterCanvas.value) {
    console.error('no letterCanvas')
    return
  }
  // if page changes, clear the canvas elements and redraw
  pageWidth = props.page.boundingBox.topRightX
  pageHeight = props.page.boundingBox.topRightY
  scale = letterCanvas.value.width / pageWidth
  clearCanvas()
  resize()
})

watch(() => showBlockBorders.value, () => {
  clearCanvas('boxes')
  renderPage(true)
})

watch(() => showLineBorders.value, () => {
  clearCanvas('boxes')
  renderPage(true)
})

watch(() => showWordBorders.value, () => {
  clearCanvas('boxes')
  renderPage(true)
})

watch(() => showLetterBorders.value, () => {
  clearCanvas('boxes')
  renderPage(true)
})

onMounted(() => {
  window.addEventListener('resize', resize)
  letterCtx.value = letterCanvas.value?.getContext('2d') as CanvasRenderingContext2D
  boxesCtx.value = boxesCanvas.value?.getContext('2d') as CanvasRenderingContext2D
  resize()
})

onUnmounted(() => {
  window.removeEventListener('resize', resize)
  letterCtx.value = undefined
  boxesCtx.value = undefined
})

const resize = () => {
  if (!letterCanvas.value || !boxesCanvas.value || !wrapper.value) {
    console.error('no canvas or no wrapper for resize')
    return
  }
  // on resize, canvas contents are cleared automatically, just calc the new size and redraw
  letterCanvas.value.width = wrapper.value.clientWidth
  letterCanvas.value.height = wrapper.value.clientWidth / pageWidth * pageHeight
  boxesCanvas.value.width = letterCanvas.value.width
  boxesCanvas.value.height = letterCanvas.value.height
  wrapper.value.style.height = `${letterCanvas.value.height}px`
  scale = letterCanvas.value.width / pageWidth
  renderPage()
}

const getY = (y: number) => pageHeight - y

const drawBox = (ctx: CanvasRenderingContext2D, boundingBox: Box, scale: number, color: string, lineWidth: number) => {
  ctx.beginPath()
  ctx.lineWidth = lineWidth
  ctx.strokeStyle = color
  const x = boundingBox.bottomLeftX * scale
  const y = getY(boundingBox.topRightY) * scale
  const width = (boundingBox.topRightX - boundingBox.bottomLeftX) * scale
  const height = (boundingBox.topRightY - boundingBox.bottomLeftY) * scale
  ctx.rect(x, y, width, height)
  ctx.stroke()
}

const getFont = (fontName: string, fontSize: number, scale: number) => {
  const parts: string[] = []
  if (fontName.indexOf('italic') > -1) {
    parts.push('italic')
  }
  if (fontName.indexOf('bold') > -1) {
    parts.push('bold')
  }
  parts.push(`${fontSize * scale}px`)
  let font = 'Times New Roman, Times, serif'
  if (fontName.indexOf('Arial') > -1) {
    font = 'Arial, Helvetica, sans-serif'
  }
  parts.push(font)
  return parts.join(' ')
}

const renderPage = (boxesOnly: boolean = false) => {
  if (!letterCanvas.value || !boxesCanvas.value || !letterCtx.value || !boxesCtx.value) {
    return
  }
  for (var b = 0; b < props.page.blocks.length; b++) {
    const block = props.page.blocks[b]
    // box around each block
    if (showBlockBorders.value) {
      drawBox(boxesCtx.value, block.boundingBox, scale, 'red', 1.5)
    }
    for (var l = 0; l < block.lines.length; l++) {
      const line = block.lines[l]
      // box around each line
      if (showLineBorders.value) {
        drawBox(boxesCtx.value, line.boundingBox, scale, 'blue', 1)
      }
      for (var w = 0; w < line.words.length; w++) {
        const word = line.words[w]
        const fontName = word.fontName
        // box around each word
        if (showWordBorders.value) {
          drawBox(boxesCtx.value, word.boundingBox, scale, 'green', 0.6)
        }
        for (var le = 0; le < word.letters.length; le++) {
          const letter = word.letters[le]
          // box around each letter
          const x = letter.startBaseLine.X * scale
          const y = (getY(letter.startBaseLine.Y) - letter.fontSize) * scale
          const width = (letter.endBaseLine.X - letter.startBaseLine.X) * scale
          const height = letter.fontSize * scale
          if (showLetterBorders.value) {
            boxesCtx.value.beginPath()
            boxesCtx.value.lineWidth = 0.3
            boxesCtx.value.strokeStyle = 'violet'
            boxesCtx.value.rect(x, y, width, height)
            boxesCtx.value.stroke()
          }
          // letter
          if (!boxesOnly) {
            letterCtx.value.font = getFont(fontName, letter.fontSize, scale)
            letterCtx.value.fillText(letter.text, letter.startBaseLine.X * scale, getY(letter.startBaseLine.Y) * scale)
          }
        }
      }
    }
  }
}

const highlight = (evt: MouseEvent) => {

}

const select = (evt: MouseEvent) => {

}

</script>

<style lang="scss" scoped>

.wrapper {
  position: relative;
  width: 100%;
  height: 100%;
  canvas {
    position: absolute;
    top: 0;
    left: 0;
  }
}

</style>
