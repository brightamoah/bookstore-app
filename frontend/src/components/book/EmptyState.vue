<script setup lang="ts">
const emit = defineEmits<{
  fetchBooks: []
}>()

const fetchAllBooks = () => {
  emit('fetchBooks')
}
</script>

<template>
  <div class="py-12 text-center">
    <div class="mx-auto w-12 h-12 text-gray-400">
      <UIcon name="tabler-books" class="mx-auto size-15 text-primary" />
    </div>
    <h3 class="mt-4 font-medium text-xl text-center">No books</h3>
    <p class="mt-1 text-sm">Get started by adding your first book to the inventory.</p>

    <UButton
      color="primary"
      variant="outline"
      size="xl"
      trailing-icon="i-lucide-cloud-download"
      class="mt-5"
      @click="fetchAllBooks"
    >
      Fetch Books
    </UButton>
  </div>
</template>
