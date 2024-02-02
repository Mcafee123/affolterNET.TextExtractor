<template lang="pug">
article
  button
    i attach_file
    span Upload PDF
    input(id="pdf_upload" type="file" name="pdf-upload" @change="uploadFile($event)" accept="application/pdf")
  p Je nach Grösse des PDF Dokuments kann das Verarbeiten einige Sekunden dauern.
  p Es wird (noch) kein OCR durchgeführt.
</template>

<script lang="ts" setup>

const emit = defineEmits(['upload-file'])

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const uploadFile = ($event: Event) => {
  const target = $event.target as HTMLInputElement
  if (!target || !target.files) {
    console.error(`no files`)
    return
  }
  const pdf = target.files[0]
  if (!pdf.name.endsWith('.pdf')) {
    console.error(`"${pdf.name}" is not a pdf file`)
    return
  }
  try {
    emit('upload-file', pdf)
  }
  finally {
    target.value = ''
  }
}

</script>

<style lang="scss" scoped>

</style>
