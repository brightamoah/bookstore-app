<script setup lang="ts">
import type { Book } from '@/Types/types'

defineProps<{
  books: Book[]
  editBook: (book: Book) => void
  deleteBook: (book: Book) => void
  showBookDetail: (book: Book) => void
}>()
</script>

<template>
  <div class="gap-6 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
    <UCard
      v-for="book in books"
      :key="book.id"
      :ui="{
        body: 'p-4',
      }"
      class="bg-muted hover:shadow-md transition-shadow duration-200 cursor-pointer"
      @click="showBookDetail(book)"
    >
      <div class="space-y-4">
        <div class="relative">
          <img
            :src="book.imageUrl"
            :alt="book.bookName"
            class="rounded-lg w-full h-48 object-cover"
          />
          <div class="top-2 right-2 absolute">
            <UBadge color="success" variant="solid" size="sm"> In Stock </UBadge>
          </div>
        </div>

        <div class="space-y-2">
          <h3 class="font-semibold text-gray-900 dark:text-white text-lg line-clamp-2">
            {{ book.bookName }}
          </h3>
          <p class="text-gray-600 dark:text-gray-300 text-sm">by {{ 'Henry Cavill' }}</p>
          <div class="flex justify-between items-center">
            <UBadge color="primary" variant="soft">
              {{ book.category }}
            </UBadge>
            <span class="font-bold text-gray-900 dark:text-white text-lg"> ${{ book.price }} </span>
          </div>
        </div>

        <!-- Action Buttons -->
        <div
          class="flex justify-between items-center pt-2 border-gray-200 dark:border-gray-700 border-t"
        >
          <UButton
            @click.stop="editBook(book)"
            color="primary"
            variant="outline"
            size="sm"
            icon="i-heroicons-pencil-square"
            class="cursor-pointer"
          />

          <div class="flex gap-2">
            <UButton
              @click.stop="deleteBook(book)"
              color="error"
              variant="outline"
              size="sm"
              icon="i-heroicons-trash"
              class="cursor-pointer"
            />
          </div>
        </div>
      </div>
    </UCard>
  </div>
</template>

<style scoped></style>
