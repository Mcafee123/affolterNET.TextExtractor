<template lang="pug">
.box(v-if="customBox")
  .row 
    b BottomLeft:
  .row.wrap
    .field.border.small
      input(type="number" v-model="customBox.bottomLeftX")
    .field.border.small
      input(type="number" v-model="customBox.bottomLeftY")
  .row 
    b TopRight:
  .row.wrap
    .field.border.small
      input(type="number" v-model="customBox.topRightX")
    .field.border.small
      input(type="number" v-model="customBox.topRightY")
  .row.first
    button(@click="clearCustomBox()" alt="Box löschen" :disabled="!customBoxIsSet()")
      i clear
</template>

<script lang="ts" setup>

import { useViewSettings } from "@/composables/useViewSettings"

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const { customBox, customBoxIsSet, clearCustomBox } = useViewSettings()

</script>

<style lang="scss" scoped>

.row {
  .field {
    max-width: 100px;
  }
  button {
    max-width: 100px;
  }
  margin-block-start: 0;
  &.first {
    margin-block-start: 0.6rem;
  }
}

</style>
