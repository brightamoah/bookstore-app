<script setup lang="ts">
import { useColorMode, useWindowSize } from '@vueuse/core'

const mode = useColorMode()
</script>

<template>
  <div>
    <UButton
      color="neutral"
      variant="ghost"
      class="cursor-pointer"
      :icon="mode === 'dark' ? 'i-lucide-moon' : 'i-lucide-sun'"
      @click="mode = mode === 'dark' ? 'light' : 'dark'"
    />
  </div>
</template>

<style scoped></style>
