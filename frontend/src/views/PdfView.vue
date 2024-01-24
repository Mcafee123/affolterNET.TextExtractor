<template lang="pug">
h5
    RouterLink(to="/") Übersicht
h3 {{ filename }}

.pdfview
  .pageview(v-if="pdfdata")
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

import { onMounted, ref } from 'vue'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import { useRoute, RouterLink } from 'vue-router'

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

import pdfDemoData from '@/assets/Verfuegung_Nr_23-24_24846_3.json'
import type { PdfDocument } from '@/types/pdfDocument'

const route = useRoute()
const filename = ref<string>('')

onMounted(async () => {
  filename.value = route.params.filename as string || ''
  console.log(filename.value)
  await getData()
})

const pdfdata = ref<PdfDocument | null>(null)

const getData = async () => {
//   const response = await fetch(`/assets/${filename.value}`)
//   const data = await response.json()
  pdfdata.value = pdfDemoData as PdfDocument
}

</script>

<style lang="scss" scoped>
.pdfview {
  display: flex;
  flex-direction: row;
  align-items: flex-start;
  .pageview {
    display: flex;
    flex-direction: column;
    align-items: center;
  }

  .settingscol {
    background: rgb(52, 52, 228);
    padding: 5px;
    max-width: 200px;
  }
}

</style>
