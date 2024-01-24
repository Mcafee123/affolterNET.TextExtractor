<template lang="pug">
h1 Extract Text from PDF
ul
    li(@click="goToPdf('Verfuegung_Nr_23-24_24846_3.json')") Verfügung Yanik Zürcher
</template>

<script lang="ts" setup>

import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const goToPdf = (filename: string) => {
    const path = `/pdf/${encodeURI(filename)}`
    router.push(path)
}

</script>

<style lang="scss" scoped>
li {
    list-style-type: none;
    cursor: pointer;
    color: blue;
    text-decoration: underline;
}
</style>
