<template lang="pug">
.grid
  .s12.m9.l6
    article(v-if="docs.length > 0")
      nav.no-space(v-for="doc in docs" :key="doc.foldername")
        button.border.left-round.fill(@click="openDoc(doc.foldername)")
          i picture_as_pdf
        button.border.no-round.max(@click="openDoc(doc.foldername)")
          span {{ doc.filename }}
          .badge.none added: {{ showDate(doc.created) }}
        button.border.right-round(@click="deleteDoc(doc.foldername)")
          i delete
    article(v-if="!hasDocs")
      p no documents found
</template>
<script lang="ts" setup>
import { onMounted, ref } from "vue";
import { loaderService } from "affolternet-vue3-library";
import type { ListDoc } from "@/types/listdoc";
import dayjs from "dayjs";
import { useRouter } from "vue-router";
import { api } from "@/services/api";

const hasDocs = ref<boolean>(true)
const docs = ref<ListDoc[]>([]);
const router = useRouter();

onMounted(async () => {
  loaderService.showLoader();
  try {
    const doclist = await api.get<ListDoc[]>('/api/listDocuments');
    doclist.sort(function (a: ListDoc, b: ListDoc) {
      return new Date(b.created).getTime() - new Date(a.created).getTime();
    });
    docs.value = doclist;
    if (docs.value.length === 0) {
      hasDocs.value = false;
    }
  } finally {
    loaderService.hideLoader();
  }
});

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const showDate = (dateString: string) => {
  const date = dayjs(dateString);
  // Then specify how you want your dates to be formatted
  return date.format("DD. MMM YYYY HH:mm:ss");
};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const openDoc = (doc: string) => {
  router.push({ name: "pdf", params: { folder: doc, pageno: 1 } });
};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const deleteDoc = async (folder: string) => {
  loaderService.showLoader();
  try {
    await api.delete(`/api/deleteDocument/${folder}`);
    docs.value = docs.value.filter((d) => d.foldername !== folder);
  } finally {
    loaderService.hideLoader();
  }
};
</script>

<style lang="scss" scoped></style>
