<template lang="pug">
article
  h6(@click="toggle()") 
    i(v-if="showDetails") expand_less
    i(v-if="!showDetails") expand_more
    span {{ title }}
  Transition(name="collapse")
    .contents(v-show="showDetails")
      slot
</template>

<script lang="ts" setup>

import { ref, watch} from 'vue'
import { useStorage } from '@/composables/useStorage'

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const props = defineProps({
  title: {
    type: String,
    required: true
  }
})

const storageKey: string = `show${props.title.replace(/ /g, '+')}Details`
const { getFromStorage, setToStorage } = useStorage()
const showDetails = ref<boolean>(getFromStorage(storageKey, 'true') === 'true')
watch (showDetails, (newVal) => {
  setToStorage(storageKey, newVal.toString())
})

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const toggle = () => {
  showDetails.value = !showDetails.value
}

</script>

<style lang="scss" scoped>

h5 {
  cursor: pointer;
}

.collapse-enter-active,
.collapse-leave-active {
  transition: opacity 0.2s ease;
}

.collapse-enter-from,
.collapse-leave-to {
  opacity: 0;
}

</style>
