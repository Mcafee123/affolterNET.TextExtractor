<template lang="pug">
h1 Extract Text from PDF: {{ msg }}
ul
    li(@click="goToPdf('Verfuegung_Nr_23-24_24846_3.json')") Verfügung Yanik Zürcher
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
li {
    list-style-type: none;
    cursor: pointer;
    color: blue;
    text-decoration: underline;
}
</style>
