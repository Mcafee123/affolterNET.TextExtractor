<template lang="pug">
h4 Extract Text from PDF
h6 {{ msg }}

</template>

<script lang="ts" setup>

import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

import { loaderService } from 'affolternet-vue3-library'

const router = useRouter()

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const goToPdf = (filename: string) => {
  const path = `/pdf/${encodeURI(filename)}`
  router.push(path)
}

const msg = ref<string>('')
onMounted(async () => {
  loaderService.showLoader()
  try {
  const { text } = await( await fetch(`/api/info`)).json();
  msg.value = text
  }
  finally {
    loaderService.hideLoader()
  } 
})

</script>

<style lang="scss" scoped>

.container {
  padding: 20px;
}

</style>
