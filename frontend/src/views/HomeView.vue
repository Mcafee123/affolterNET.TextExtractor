<template lang="pug">
.container
  h1 Extract Text from PDF: {{ msg }}
  h3 Go to: 
      RouterLink(to="/pdf") Pdf-Text-Extractor
</template>

<script lang="ts" setup>

import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const goToPdf = (filename: string) => {
  const path = `/pdf/${encodeURI(filename)}`
  router.push(path)
}

const msg = ref<string>('')
onMounted(async () => {
  const { text } = await( await fetch(`/api/info`)).json();
  msg.value = text
})

</script>

<style lang="scss" scoped>

.container {
  padding: 20px;
}

</style>
