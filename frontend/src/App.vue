<script setup lang="ts">

</script>

<template lang="pug">

nav.left
  RouterLink(to="/")
    i home
    div Home
  RouterLink(to="/pdf")
    i picture_as_pdf 
    div PDF
  a
    i search
    div Search
  a
    i share
    div share
main.responsive
  RouterView

</template>

<style lang="scss" scoped>
.main {
    min-height: 800px;
    background: lightgray;
}

.block {
    position: absolute;
    border: 1px solid gray;
}
</style>
