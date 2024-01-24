<script setup lang="ts">

import { ref } from 'vue'
import { RouterLink, RouterView } from 'vue-router'
import pdfdata from './assets/Verfuegung_Nr_23-24_24846_3.json'
import type { PdfDocument } from './types/pdfDocument'
import type { Block } from './types/block'
import type { Line } from './types/line'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const pdf = ref<PdfDocument>(pdfdata as PdfDocument)


// eslint-disable-next-line @typescript-eslint/no-unused-vars
const getLineStyle = (line: Line) => {
    return `left: ${line.boundingBox.topRightY}px`
}

</script>

<template lang="pug">

RouterView

</template>

<style lang="scss" scoped>
.main {
    min-height: 800px;
    background: lightgray;
}

.block {
    position: absolute;
    border: 1px solid gray;
}
</style>
./types/pdfDocument