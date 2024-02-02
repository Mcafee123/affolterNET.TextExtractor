<template lang="pug">
.grid
  .s9.m9.l9
    PdfUpload(class="center upload" v-if="!page" @uploadFile="uploadFile")
    .pdfview(v-if="page")
      .backcol(v-if="pdfdata.pages.length > 1" :class="{ invisible: !hasLeft() }" @click="goLeft")
        button.circle.transparent.left
          img.responsive(src="@/assets/arrow_left_icon.png")
      .pageview.responsive
        .center.current(v-if="pdfdata.pages.length > 1")
          .field.border.round.small
            input(type="number" v-model="currentPage")
          .field.border.round.small
            input(type="number" v-model="pdfdata.pages.length" disabled)
        PdfPageCanvas(:page="page")
      .nextcol(v-if="pdfdata.pages.length > 1" :class="{ invisible: !hasRight() }" @click="goRight")
        button.circle.transparent.right
          img.responsive(src="@/assets/arrow_right_icon.png")
  .s3.m3.l3
    .settingscol(v-if="page")
      PdfPart(title="File")
        p {{ pdfdata.filename }}
        button(@click="refreshView()")
          i frame_reload
          span Reload PDF
      PdfPart(title="Upload")
        PdfUpload(@uploadFile="uploadFile")
      PdfPart(title="Anzeigen")
        PdfViewSettings
      PdfPart(title="Box zeichnen")
        PdfBox
      PdfPart(title="Block")
        PdfBlockJson
      PdfPart(title="Line")
        PdfLineJson
      PdfPart(title="Word")
        PdfWordJson
      PdfPart(title="Letter")
        PdfLetterJson
    
</template>

<script lang="ts" setup>
import { ref, watch } from "vue"

// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfPageCanvas from "@/components/PdfPageCanvas.vue"
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfUpload from "@/components/settings/PdfUpload.vue"
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfViewSettings from "@/components/settings/PdfViewSettings.vue"
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfBlockJson from "@/components/settings/PdfBlockJson.vue"
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfLineJson from "@/components/settings/PdfLineJson.vue"
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfWordJson from "@/components/settings/PdfWordJson.vue"
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfLetterJson from "@/components/settings/PdfLetterJson.vue"
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfPart from "@/components/settings/PdfPart.vue"
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfBox from "@/components/settings/PdfBox.vue"

import type { PdfDocument } from "@/types/pdfDocument"
import type { Page } from "@/types/page"

import { loaderService } from "affolternet-vue3-library"
import { toastService } from 'affolternet-vue3-library'

const pdfdata = ref<PdfDocument | null>(null);
const page = ref<Page | null>(null);
const currentPage = ref<number>(0);
const pdfFile = ref<File | null>(null)

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const refreshView = () => {
  if (!pdfFile.value) {
    console.error('no pdf loaded')
    return
  }
  uploadFile(pdfFile.value as File)
};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const uploadFile = async (pdf: File) => {
  loaderService.showLoader()
  pdfFile.value = pdf
  try {
    const formData = new FormData();
    formData.append("file", pdf);
    const options = {
      method: "POST",
      body: formData,
    };
    const response = await fetch("/api/upload", options)
    if (response.status !== 200) {
      toastService.showError(`parsing pdf failed, response.status = ${response.status}`)
      return
    }
    const json = await response.json()
    pdfdata.value = json as PdfDocument
    currentPage.value = 1
    setPage(0)
  } catch (error) {
    console.error(error)
  } finally {
    loaderService.hideLoader()
  }
}

watch(currentPage, (index) => {
  setPage(index - 1);
});

const setPage = (index: number) => {
  if (!pdfdata.value) {
    console.error("no pdf loaded")
    return;
  }
  if (index < 0 || index > pdfdata.value.pages.length - 1) {
    console.log("invalid index", index);
    return
  }
  page.value = pdfdata.value.pages[index]
};

const hasLeft = () => {
  return currentPage.value > 1
};

const hasRight = () => {
  if (!pdfdata.value) {
    console.error("no pdf loaded")
    return;
  }
  return currentPage.value < pdfdata.value.pages.length
};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const goLeft = () => {
  if (hasLeft()) {
    currentPage.value--
  }
};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const goRight = () => {
  if (hasRight()) {
    currentPage.value++
  }
};
</script>

<style lang="scss" scoped>
.grid {
  margin-top: 0;
  min-width: 700px;
}

.upload {
  margin-top: 5%;
}

.pdfview {
  display: flex;
  flex-wrap: nowrap;
  align-items: stretch;

  .pageview {
    padding-top: 2px;
    .field {
      margin-block-start: 0;
      margin-block-end: 2px;
    }
    .current {
      display: flex;
      max-width: 180px;
    }
  }
  .settingscol {
    background: #052;
    padding: 5px;
    overflow-y: scroll;
  }

  .backcol,
  .nextcol {
    cursor: pointer;
    min-height: 100%;
    background: var(--primary);
    opacity: 0.3;
    transition: opacity 0.2s ease-in-out;
    &:hover {
      opacity: 0.8;
    }
    padding-top: 50%;
  }
}
</style>
