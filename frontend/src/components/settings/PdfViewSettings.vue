<template lang="pug">
.col
  label.checkbox.icon
    input(type="checkbox" v-model="showBlockBorders")
    span
      i close 
      i done
    span Blockgrenzen
  label.checkbox.icon
    input(type="checkbox" v-model="showLineBorders")
    span
      i close 
      i done
    span Zeilengrenzen
  label.checkbox.icon
    input(type="checkbox" v-model="showWordBorders")
    span
      i close 
      i done  
    span Wortgrenzen
  label.checkbox.icon
    input(type="checkbox" v-model="showLetterBorders")
    span
      i close 
      i done
    span Buchstabengrenzen
  label.checkbox.icon
    input(type="checkbox" v-model="showFootnotes")
    span
      i close 
      i done
    span Fussnoten
  label.checkbox.icon
    input(type="checkbox" v-model="showPageNumbers")
    span
      i close 
      i done
    span Seitenzahlen
</template>

<script lang="ts" setup>

import { useViewSettings } from '@/composables/useViewSettings'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { showBlockBorders, showLineBorders, showWordBorders, showLetterBorders, showFootnotes, showPageNumbers } = useViewSettings()

</script>

<style lang="scss" scoped>
.col {
  display: flex;
  flex-direction: column;
}
</style>
