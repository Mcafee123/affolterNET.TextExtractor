<template lang="pug">
.page(:style="getStyle()")
    PdfBlock(v-for="block in page.blocks" :block="block" :pageheight="height")

</template>

<script lang="ts" setup>

import type { PropType } from 'vue'
import type { Page } from '@/types/page'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfBlock from '@/components/PdfBlock.vue'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  page: {
    required: true,
    type: Object as PropType<Page>
  }
})

const width = props.page.boundingBox.topRightX
const height = props.page.boundingBox.topRightY

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const getStyle = () => {
    return `width: ${width}px; height: ${height}px`
}

</script>

<style lang="scss" scoped>
.page {
    position: relative;
    background: lightgray;
}
</style>