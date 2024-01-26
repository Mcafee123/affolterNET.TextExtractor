<template lang="pug">
.wrapper(ref="wrapper")
  canvas(id="letterCanvas" ref="letterCanvas")
  canvas(id="boxesCanvas" ref="boxesCanvas")
  canvas(id="selectionCanvas" ref="selectionCanvas")
  canvas(id="hoveringCanvas" ref="hoveringCanvas" @mousemove="highlight" @click="select")
</template>

<script lang="ts" setup>

import { useViewSettings } from '@/composables/useViewSettings';
import type { Box } from '@/types/boundingBox';
import type { Letter } from '@/types/letter';
import type { Page } from '@/types/page';
import { onMounted, ref, type PropType, type Ref, onUnmounted, watch } from 'vue'

const wrapper: Ref<HTMLDivElement | undefined> = ref()
const letterCanvas: Ref<HTMLCanvasElement | undefined> = ref()
const boxesCanvas: Ref<HTMLCanvasElement | undefined> = ref()
const hoveringCanvas: Ref<HTMLCanvasElement | undefined> = ref()
const selectionCanvas: Ref<HTMLCanvasElement | undefined> = ref()
const letterCtx = ref<CanvasRenderingContext2D>()
const boxesCtx = ref<CanvasRenderingContext2D>()
const hoveringCtx = ref<CanvasRenderingContext2D>()
const selectionCtx = ref<CanvasRenderingContext2D>()

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
const { showBlockBorders, showLineBorders, showWordBorders, showLetterBorders, blockJson, lineJson, wordJson, letterJson, hexToRgb } = useViewSettings()
const prim = hexToRgb('--primary')
let primarycoloralpha = 'rgba(134, 217, 146, 0.5)'
if (prim) {
  primarycoloralpha = `rgba(${prim.r}, ${prim.g}, ${prim.b}, 0.5)`
}
const onprim = hexToRgb('--on-primary')
let onprimarycolor = 'rgba(134, 217, 146, 1)'
if (onprim) {
  onprimarycolor = `rgba(${onprim.r}, ${onprim.g}, ${onprim.b}, 1)`
}
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
  hoveringCtx.value = hoveringCanvas.value?.getContext('2d') as CanvasRenderingContext2D
  selectionCtx.value = selectionCanvas.value?.getContext('2d') as CanvasRenderingContext2D
  resize()
})

onUnmounted(() => {
  window.removeEventListener('resize', resize)
  letterCtx.value = undefined
  boxesCtx.value = undefined
  hoveringCtx.value = undefined
})

const resize = () => {
  if (!letterCanvas.value || !boxesCanvas.value || !hoveringCanvas.value || !selectionCanvas.value || !wrapper.value) {
    console.error('no canvas or no wrapper for resize')
    return
  }
  // on resize, canvas contents are cleared automatically, just calc the new size and redraw
  letterCanvas.value.width = wrapper.value.clientWidth
  letterCanvas.value.height = wrapper.value.clientWidth / pageWidth * pageHeight
  boxesCanvas.value.width = letterCanvas.value.width
  boxesCanvas.value.height = letterCanvas.value.height
  hoveringCanvas.value.width = letterCanvas.value.width
  hoveringCanvas.value.height = letterCanvas.value.height
  selectionCanvas.value.width = letterCanvas.value.width
  selectionCanvas.value.height = letterCanvas.value.height
  wrapper.value.style.height = `${letterCanvas.value.height}px`
  scale = letterCanvas.value.width / pageWidth
  renderPage()
}

const getY = (y: number) => pageHeight - y

const makeRect = (ctx: CanvasRenderingContext2D, boundingBox: Box, heightscale: number = 1.2) => {
  const x = boundingBox.bottomLeftX * scale
  const y = getY(boundingBox.topRightY) * scale
  const width = (boundingBox.topRightX - boundingBox.bottomLeftX) * scale
  const height = ((boundingBox.topRightY - boundingBox.bottomLeftY) * heightscale) * scale
  ctx.rect(x, y, width, height)
}

const makeLetterRect = (ctx: CanvasRenderingContext2D, letter: Letter) => {
  const x = letter.startBaseLine.X * scale
  const y = (getY(letter.startBaseLine.Y) - letter.fontSize) * scale
  const width = (letter.endBaseLine.X - letter.startBaseLine.X) * scale
  const height = letter.fontSize * scale
  ctx.beginPath()
  ctx.rect(x, y, width, height)
  return width
}

const drawBox = (ctx: CanvasRenderingContext2D, boundingBox: Box, color: string, lineWidth: number, rectheightscale = 1) => {
  ctx.beginPath()
  ctx.lineWidth = lineWidth
  ctx.strokeStyle = color
  makeRect(ctx, boundingBox, rectheightscale)
  ctx.stroke()
}

const getFont = (fontName: string, fontSize: number) => {
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
      drawBox(boxesCtx.value, block.boundingBox, 'red', 1.5, 1.05)
    }
    for (var l = 0; l < block.lines.length; l++) {
      const line = block.lines[l]
      // box around each line
      if (showLineBorders.value) {
        drawBox(boxesCtx.value, line.boundingBox, 'blue', 1, 1.1)
      }
      for (var w = 0; w < line.words.length; w++) {
        const word = line.words[w]
        const fontName = word.fontName
        // box around each word
        if (showWordBorders.value) {
          drawBox(boxesCtx.value, word.boundingBox, 'green', 0.6)
        }
        for (var le = 0; le < word.letters.length; le++) {
          const letter = word.letters[le]
          boxesCtx.value.beginPath()
          boxesCtx.value.lineWidth = 0.3
          boxesCtx.value.strokeStyle = 'violet'
          const width = makeLetterRect(boxesCtx.value, letter)
          // box around each letter
          if (showLetterBorders.value) {
            boxesCtx.value.stroke()
          }
          // letter
          if (!boxesOnly) {
            letterCtx.value.font = getFont(fontName, letter.fontSize)
            letterCtx.value.fillText(letter.text, letter.startBaseLine.X * scale, getY(letter.startBaseLine.Y) * scale, width)
          }
        }
      }
    }
  }
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const highlight = ($event: MouseEvent) => {
  const cvs = hoveringCanvas.value
  const ctx = hoveringCtx.value
  if (!cvs || !ctx) {
    return
  }

  // important: correct mouse position:
  const rect = ctx.canvas.getBoundingClientRect()
  const x = $event.clientX - rect.left
  const y = $event.clientY - rect.top

  // loop through objects
  ctx.clearRect(0, 0, cvs.width, cvs.height)
  for (var b = 0; b < props.page.blocks.length; b++) {
    const block = props.page.blocks[b]
    for (var l = 0; l < block.lines.length; l++) {
      const line = block.lines[l]
      for (var w = 0; w < line.words.length; w++) {
        const word = line.words[w]
        ctx.beginPath()
        makeRect(ctx, word.boundingBox, 1.3)
        if (ctx.isPointInPath(x, y)) {
          cvs.style.cursor = 'pointer'
          ctx.fillStyle = primarycoloralpha
          ctx.fill()
          return
        } else {
          cvs.style.cursor = 'default'
        }
      }
    }
  }
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const select = ($event: MouseEvent) => {
  const cvs = selectionCanvas.value
  const ctx = selectionCtx.value
  if (!cvs || !ctx) {
    return
  }

  // important: correct mouse position:
  const rect = ctx.canvas.getBoundingClientRect()
  const x = $event.clientX - rect.left
  const y = $event.clientY - rect.top

  for (var b = 0; b < props.page.blocks.length; b++) {
    const block = props.page.blocks[b]
    for (var l = 0; l < block.lines.length; l++) {
      const line = block.lines[l]
      for (var w = 0; w < line.words.length; w++) {
        const word = line.words[w]
        for (var le = 0; le < word.letters.length; le++) {
          const letter = word.letters[le]
          ctx.beginPath()
          makeLetterRect(ctx, letter)
          if (ctx.isPointInPath(x, y)) {
            letterJson.value = { fontSize: letter.fontSize, text: letter.text, startBaseLine: letter.startBaseLine }
            wordJson.value = { boundingBox: word.boundingBox, text: word.text, fontName: word.fontName }
            lineJson.value = { boundingBox: line.boundingBox, text: line.words.map(w => w.text).join('') }
            blockJson.value = { boundingBox: block.boundingBox, text: block.lines.map(l => l.words.map(w => w.text).join('')).join('') }
            
            // make selection Rectangle
            ctx.clearRect(0, 0, cvs.width, cvs.height)
            ctx.beginPath()
            makeRect(ctx, word.boundingBox)
            ctx.fillStyle = primarycoloralpha
            ctx.strokeStyle = onprimarycolor
            ctx.fill()
            ctx.stroke()
            return
          }
        }
      }
    }
  }
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
  #letterCanvas {
    background-color: white;
  }
}
</style>
