<script setup lang="ts">
import Navbar from './components/app/Navbar.vue'
</script>

<template>
  <UApp>
    <div>
      <Navbar />
      <RouterView />
      <!-- <Footer /> -->
    </div>
  </UApp>
</template>

<style scoped></style>
