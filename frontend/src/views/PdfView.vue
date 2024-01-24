<template lang="pug">
h5
    RouterLink(to="/") Übersicht

.upload
  h3 Upload PDF
  input(id="pdf_upload" type="file" name="pdf-upload" @change="uploadFile($event)" accept="application/pdf")

.pdfview(v-if="pdfdata")
  .pageview
    PdfPage(v-for="page in pdfdata.pages" :page="page")
  .settingscol
    PdfViewSettings
    PdfBlockJson
    PdfLineJson
    PdfWordJson
    PdfLetterJson
.bottomnavi
    
</template>

<script lang="ts" setup>

import { ref } from 'vue'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfPage from '@/components/PdfPage.vue'
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

const pdfdata = ref<PdfDocument | null>(null)

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const uploadFile = async ($event: Event) => {
  console.log('upload')
  const target = $event.target as HTMLInputElement
  if (!target || !target.files) {
    return
  }
  const pdf = target.files[0]
  if (!pdf.name.endsWith('.pdf')) {
    console.error(`"${pdf.name}" is not a pdf file`)
    return
  }
  const formData = new FormData()
  formData.append('file', pdf)
  const options = {
    method: 'POST',
    body: formData,
  }
  const response = await (await fetch('/api/upload', options)).json()
  pdfdata.value = response as PdfDocument
}

</script>

<style lang="scss" scoped>
.pdfview {
  display: flex;

  .settingscol {
    background: #052;
    padding: 5px;
    max-width: 250px;
    overflow-y: scroll
  }
}
 * {
  box-sizing: unset;
 }
</style>
