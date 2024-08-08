<template lang="pug">
.test {{ showDate("2024-05-08T20:58:33") }}
.grid 
  .s12.m9.l6
    article(v-if="docs")
      nav.no-space(v-for="doc in docs" :key="doc.foldername")
        button.border.left-round.fill(@click="console.log('click')")
          i picture_as_pdf
        button.border.no-round.max(@click="console.log('click')") {{ doc.filename }}
        button.border.right-round(@click="console.log('delete')")
          i delete
</template>
<script lang="ts" setup>

import { onMounted, ref } from "vue"
import { loaderService } from "affolternet-vue3-library"
import type { ListDoc } from "@/types/listdoc"
import dayjs from "dayjs"

const docs = ref<ListDoc[]>([])

onMounted(async () => {
  loaderService.showLoader()
  try {
    const doclist = await( await fetch(`/api/listextracts`)).json();
    doclist.sort(function(a: ListDoc, b: ListDoc){
      return new Date(b.created).getTime() - new Date(a.created).getTime();
    })
    docs.value = doclist
  }
  finally {
    loaderService.hideLoader()
  } 
})

const showDate = (dateString: string) => {
  const date = dayjs(dateString);
  // Then specify how you want your dates to be formatted
  return date.format('DD. MMM YYYY HH:mm:ss');
}

</script>

<style lang="scss" scoped>
</style>
