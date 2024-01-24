<template lang="pug">
.viewsettings
  h3 Anzeigeeinstellungen
  .contents
    .cb
      input(type="checkbox" v-model="showBlockBorders")
      span Blockgrenzen
    .cb
      input(type="checkbox" v-model="showLineBorders")
      span Zeilengrenzen
    .cb
      input(type="checkbox" v-model="showWordBorders")
      span Wortgrenzen
    .cb
      input(type="checkbox" v-model="showLetterBorders")
      span Buchstabengrenzen

</template>

<script lang="ts" setup>

import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { showBlockBorders, showLineBorders, showWordBorders, showLetterBorders } = useViewSettings()

</script>

<style lang="scss" scoped>
.viewsettings {
  h3 {
    color: white;
  }
  display: flex;
  flex-direction: column;
  .contents {
    background: white;
  }
}
</style>
