<script setup lang="ts">
import type { Book } from '@/Types/types'
import { ref } from 'vue'
import { useDateFormat } from '@vueuse/core'

const {
  selectedBook = {
    id: 0,
    bookName: 'Sample Book Title',
    category: 'Fiction',
    price: 29.99,
    description:
      'A captivating story that takes readers on an unforgettable journey through mystery and adventure.',
    imageUrl: 'https://images.unsplash.com/photo-1544947950-fa07a98d237f?w=300&h=400',
  },
  editBook = (book: Book) => {
    console.log('Edit book:', book)
  },
  showBookDetailsProp = true,
} = defineProps<{
  selectedBook: Book
  showBookDetailsProp: boolean
  editBook: (book: Book) => void
}>()

const showBookDetails = ref(showBookDetailsProp)
</script>

<template>
  <div class="overflow-hidden">
    <UButton
      label="Back"
      variant="subtle"
      color="primary"
      icon="i-heroicons-arrow-left-20-solid"
      class="top-4 left-4 z-10 sticky mb-4 px-5 text-base text-center cursor-pointer"
      @click="$router.back()"
    />
    <div class="flex justify-center mx-auto p-4 w-full container">
      <UCard
        v-if="selectedBook"
        variant="soft"
        class="bg-muted rounded-lg w-full max-w-6xl"
        :ui="{ body: 'p-6 ring-0', header: 'ring', footer: 'border-t' }"
      >
        <template #header>
          <div class="flex justify-between items-center">
            <h3 class="font-semibold text-xl">Book Details</h3>
          </div>
        </template>

        <div class="space-y-6">
          <div class="flex sm:flex-row flex-col gap-6">
            <div class="flex-shrink-0 w-full sm:w-1/2 md:w-5/12 lg:w-1/3">
              <img
                :src="selectedBook.imageUrl"
                :alt="selectedBook.bookName"
                class="shadow-md rounded-lg w-full h-69 object-cover"
              />
            </div>
            <div class="flex flex-col flex-1 justify-between space-y-6">
              <!-- Book Title and Author -->
              <div>
                <h2 class="mb-1 font-bold text-2xl">
                  {{ selectedBook.bookName }}
                </h2>
                <p class="text-lg italic">by {{ 'Henry Cavill' }}</p>
              </div>

              <!-- Book Details Grid -->
              <div class="gap-6 grid grid-cols-1 sm:grid-cols-2">
                <div>
                  <p class="mb-1 font-medium text-sm">Category</p>
                  <p class="">{{ selectedBook.category }}</p>
                </div>
                <div>
                  <p class="mb-1 font-medium text-sm">Price</p>
                  <p class="font-semibold">${{ selectedBook.price }}</p>
                </div>
                <div>
                  <p class="mb-1 font-medium text-sm">Stock</p>
                  <p class="">50 units</p>
                </div>
                <div>
                  <p class="mb-1 font-medium text-sm">Published</p>
                  <p class="">
                    {{ useDateFormat(new Date(), 'DD-MM-YYYY') }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div>
            <p class="mb-2 font-medium text-gray-500 dark:text-gray-400 text-sm">Description</p>
            <p class="text-gray-700 dark:text-gray-300 leading-relaxed">
              {{ selectedBook.description }}
            </p>
          </div>
        </div>

        <template #footer>
          <div class="flex justify-end gap-3">
            <UButton
              @click="editBook(selectedBook)"
              color="primary"
              variant="outline"
              icon="i-heroicons-pencil-square"
            >
              Edit Book
            </UButton>
          </div>
        </template>
      </UCard>
    </div>
  </div>
</template>

<style scoped></style>
