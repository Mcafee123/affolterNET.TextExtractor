<template lang="pug">
.grid 
  .s12.m9.l6
    article(v-if="docs")
      nav.no-space(v-for="doc in docs" :key="doc.foldername")
        button.border.left-round.fill(@click="openPdf(doc.foldername)")
          i picture_as_pdf
        button.border.no-round.max(@click="openPdf(doc.foldername)")
          span {{ doc.filename }}
          .badge.none added: {{ showDate(doc.created) }}
        button.border.right-round(@click="console.log('delete')")
          i delete
</template>
<script lang="ts" setup>

import { onMounted, ref } from "vue"
import { loaderService } from "affolternet-vue3-library"
import type { ListDoc } from "@/types/listdoc"
import dayjs from "dayjs"
import { useRouter } from "vue-router"

const docs = ref<ListDoc[]>([])
const router = useRouter()

onMounted(async () => {
  loaderService.showLoader()
  try {
    const doclist = await(await fetch(`/api/listextracts`)).json();
    doclist.sort(function(a: ListDoc, b: ListDoc){
      return new Date(b.created).getTime() - new Date(a.created).getTime();
    })
    docs.value = doclist
  }
  finally {
    loaderService.hideLoader()
  } 
})

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const showDate = (dateString: string) => {
  const date = dayjs(dateString);
  // Then specify how you want your dates to be formatted
  return date.format('DD. MMM YYYY HH:mm:ss');
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const openPdf = (doc: string) => {
  router.push({ name: 'pdf', params: { folder: doc, pageno: 1 } })
}

</script>

<style lang="scss" scoped>
</style>
