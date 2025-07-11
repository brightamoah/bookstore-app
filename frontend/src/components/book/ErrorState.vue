<script setup lang="ts">
defineProps<{
  errorMessage: string
}>()

defineEmits<{
  retry: []
}>()
</script>

<template>
  <div class="py-12 text-center">
    <UIcon name="i-lucide-alert-circle" class="mx-auto size-8 text-red-500" />
    <h3 class="mt-4 font-medium text-red-600 text-xl">Error Loading Books</h3>
    <p class="mt-2 text-gray-500">{{ errorMessage }}</p>
    <UButton color="primary" variant="outline" class="mt-4" @click="$emit('retry')">
      Try Again
    </UButton>
  </div>
</template>
