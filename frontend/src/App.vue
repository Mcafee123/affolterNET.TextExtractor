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
main.responsive.max
  RouterView

</template>

<style lang="scss" scoped>
.main.responsive.max {
    min-height: 800px;
    background: lightgray;
    padding: 0;
}

.block {
    position: absolute;
    border: 1px solid gray;
}
</style>
