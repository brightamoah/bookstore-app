<script setup lang="ts">
import type { Book } from '@/Types/types'

const emit = defineEmits<{
  submit: [book: Omit<Book, 'id'>]
  success: [message: string]
  error: [message: string]
}>()

const handleBookSubmitted = (book: Omit<Book, 'id'>) => {
  emit('submit', book)
}

const handleSuccess = (message: string) => {
  emit('success', message)
}

const handleError = (message: string) => {
  emit('error', message)
}
</script>

<template>
  <div class="shadow-sm border-b">
    <div class="mx-auto px-4 sm:px-6 lg:px-8 max-w-7xl">
      <div class="flex justify-between items-center py-6">
        <div>
          <h1 class="font-bold text-2xl">
            <UIcon name="tabler-books" class="size-10 text-primary" />
            <span>Book Store Dashboard</span>
          </h1>
          <p class="mt-1">Manage your book inventory</p>
        </div>
        <AddModal @submit="handleBookSubmitted" @success="handleSuccess" @error="handleError" />
      </div>
    </div>
  </div>
</template>
