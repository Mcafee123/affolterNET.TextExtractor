<template lang="pug">
article(class="center upload")
  PdfUpload(@uploadFile="uploadFile")
</template>

<script setup lang="ts">

import { ref } from "vue";
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfUpload from "@/components/settings/PdfUpload.vue"
import { loaderService } from "affolternet-vue3-library";
import { toastService } from "affolternet-vue3-library";

const pdfFile = ref<File | null>(null);

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const uploadFile = async (pdf: File) => {
  loaderService.showLoader();
  pdfFile.value = pdf;
  try {
    const formData = new FormData();
    formData.append("file", pdf);
    const options: RequestInit = {
      method: "POST",
      body: formData,
    };
    const response = await fetch("/api/upload", options);
    if (response.status !== 200) {
      toastService.showError(`parsing pdf failed, response.status = ${response.status}`);
      return;
    }
    // const json = await response.json();
    // pdfdata.value = json as Doc;
    // footnoteWordIds.value = getFootnoteWords();
    // currentPage.value = 1;
    // setPage(0);
  } catch (error) {
    console.error(error);
  } finally {
    loaderService.hideLoader();
  }
};
</script>

<style lang="scss" scoped>

</style>
