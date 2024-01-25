<template lang="pug">
header.fill
  nav
    button.circle.transparent
      i menu
    h5(class="max center-align") Extract Pdf Text
    button(class="circle transparent")
      img.responsive(src="/favicon-32x32.png")
.grid
  .s12.m12.l8
    PdfUpload(class="center upload" v-if="!page" @uploadFile="uploadFile")
    .pdfview(v-if="page")
      .backcol
        button(class="circle transparent left" @click="goLeft")
          img.responsive(src="@/assets/arrow_left_icon.png")
      .pageview
        .center.current
          .field.border.round.small
            input(type="number" v-model="currentPage")
        PdfPage(:page="page")
      .nextcol
        button(class="circle transparent right" @click="goRight")
          img.responsive(src="@/assets/arrow_right_icon.png")
  .s12.m12.l4
    .settingscol(v-if="page")
      PdfUpload(@uploadFile="uploadFile")
      PdfViewSettings
      PdfBlockJson
      PdfLineJson
      PdfWordJson
      PdfLetterJson
    
</template>

<script lang="ts" setup>

import { ref, watch } from 'vue'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfPage from '@/components/PdfPage.vue'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfUpload from '@/components/settings/PdfUpload.vue'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfViewSettings from '@/components/settings/PdfViewSettings.vue'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfBlockJson from '@/components/settings/PdfBlockJson.vue'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfLineJson from '@/components/settings/PdfLineJson.vue'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfWordJson from '@/components/settings/PdfWordJson.vue'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfLetterJson from '@/components/settings/PdfLetterJson.vue'

import type { PdfDocument } from '@/types/pdfDocument'
import type { Page } from '@/types/page'

const pdfdata = ref<PdfDocument | null>(null)
const page = ref<Page | null>(null)
const currentPage = ref<number>(0)

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const uploadFile = async (pdf: File) => {
  const formData = new FormData()
  formData.append('file', pdf)
  const options = {
    method: 'POST',
    body: formData,
  }
  const response = await (await fetch('/api/upload', options)).json()
  pdfdata.value = response as PdfDocument
  currentPage.value = 1
}

watch(currentPage, (index) => {
  setPage(index - 1)
})

const setPage = (index: number) => {
  if (!pdfdata.value) {
    console.error('no pdf loaded')
    return
  }
  if (index < 0 || index > pdfdata.value.pages.length -1) {
    console.log('invalid index', index)
    return
  }
  page.value = pdfdata.value.pages[index]
  console.log('page:', index + 1)
}

const hasLeft = () => {
  return currentPage.value > 1
}

const hasRight = () => {
  if (!pdfdata.value) {
    console.error('no pdf loaded')
    return
  }
  return currentPage.value  < pdfdata.value.pages.length
}

const goLeft = () => {
  if (hasLeft()) {
    currentPage.value--
  }
}

const goRight = () => {
  if (hasRight()) {
    currentPage.value++
  }
}

</script>

<style lang="scss" scoped>

.upload {
  margin-top: 20%;
}

.pdfview {
  display: flex;

  .pageview {
    background: rgb(239, 235, 218);
    padding-top: 2px;
    .field {
      margin-block-end: 2px;
    }
    .current {
      max-width: 90px;
    }
  }
  .settingscol {
    background: #052;
    padding: 5px;
    max-width: 250px;
    overflow-y: scroll
  }

  .backcol, .nextcol {
    .right, .left {
      padding-top: 1px;
      padding-left: 5px;
      padding-right: 5px;
    }
    cursor: pointer;
    min-height: 100%;
    background: rgb(239, 235, 218);
    transition: background-color 0.2s ease-in-out;
    &:hover {
      background: rgb(240, 189, 135);
    }
  }
}
 
</style>
