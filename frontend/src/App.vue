<script setup lang="ts">

import { ref } from 'vue'
import { RouterLink, RouterView } from 'vue-router'
import pdfdata from './assets/Verfuegung_Nr_23-24_24846_3.json'
import type { Document } from './types/document'
import type { Block } from './types/block'
import type { Line } from './types/line'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const pdf = ref<Document>(pdfdata)

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const getBlockStyle = (block: Block) => {
    return `left: ${block.boundingBox.topRightY}px`
}
// eslint-disable-next-line @typescript-eslint/no-unused-vars
const getLineStyle = (line: Line) => {
    return `left: ${line.boundingBox.topRightY}px`
}

/*
            <!-- .line(v-for="line in block.lines" :style="getLineStyle(line)")
                .word(v-for="word in line.words") {{ word.text }}&nbsp; -->

                */
</script>

<template lang="pug">

.main
    .page(v-for="page in pdf.pages")
        .block(v-for="block in page.blocks" :style="getBlockStyle(block)")
            .line(v-for="line in block.lines" :style="getLineStyle(line)")
                .word(v-for="word in line.words") {{ word.text }}
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
