<template lang="pug">
article(class="center upload")
  PdfUpload(@uploadFile="uploadFile")
</template>

<script setup lang="ts">
// eslint-disable-next-line @typescript-eslint/no-unused-vars
import PdfUpload from "@/components/settings/PdfUpload.vue"

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const uploadFile = () => {
  console.log("uploadFile")
}

</script>

<style lang="scss" scoped>

</style>
