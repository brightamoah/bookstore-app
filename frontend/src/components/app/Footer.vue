<script setup lang="ts">
import { useDateFormat, useNow } from '@vueuse/core'

const scrollToTop = () => {
  console.log('Scrolling to top...')
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

const currentYear = useDateFormat(useNow(), 'YYYY')
</script>

<template>
  <footer class="flex flex-col justify-center items-center space-y-4 p-10 w-full">
    <UButton
      label="Back to Top"
      variant="subtle"
      color="primary"
      class="p-3 rounded-full text-base text-center cursor-pointer"
      @click="scrollToTop"
    />

    <USeparator
      size="xs"
      :ui="{
        border: 'border-(--ui-border-inverted)',
      }"
      class="w-full md:w-[80%]"
    />

    <div class="flex sm:flex-row flex-col justify-between w-full md:w-[80%] md:max-w-6xl">
      <p class="text-center">&copy; {{ currentYear }} Bright Amoah. All rights reserved.</p>

      <div class="flex justify-center sm:justify-start gap-4 mt-4 sm:mt-0">
        <UIcon name="i-lucide-instagram" class="size-6 text-primary" />
        <UIcon name="i-lucide-facebook" class="size-6 text-primary" />
        <UIcon name="i-lucide-twitter" class="size-6 text-primary" />
      </div>
    </div>
  </footer>
</template>

<style scoped></style>
