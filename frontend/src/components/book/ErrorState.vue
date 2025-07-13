<script setup lang="ts">
defineProps<{
  errorMessage: string
}>()

defineEmits<{
  retry: []
  clearError: []
}>()
</script>

<template>
  <div class="py-12 text-center">
    <UIcon name="i-lucide-alert-circle" class="mx-auto size-8 text-red-500" />
    <h3 class="mt-4 font-medium text-red-600 text-xl">Error Loading Books</h3>
    <p class="mt-2 text-gray-500">{{ errorMessage }}</p>
    <div class="flex justify-center items-center space-x-4 mt-4">
      <UButton
        color="neutral"
        variant="outline"
        size="xl"
        icon="i-heroicons-x-mark"
        @click="$emit('clearError')"
      >
        Dismiss Error
      </UButton>

      <UButton
        color="primary"
        variant="outline"
        icon="i-heroicons-arrow-path"
        size="xl"
        @click="$emit('retry')"
      >
        Try Again
      </UButton>
    </div>
  </div>
</template>
