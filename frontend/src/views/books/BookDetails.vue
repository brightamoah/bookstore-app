<script setup lang="ts">
import type { Book } from '@/Types/types'
import { onMounted, ref } from 'vue'
import { useDateFormat } from '@vueuse/core'
import { useRoute } from 'vue-router'
import { useBookStore } from '@/stores/BookStore'
import { getImageUrl } from '@/services/Api'

const route = useRoute()
const bookStore = useBookStore()

const bookId = Number(route.params.id)

const selectedBook = ref<Book | null>(null)
const isFetching = ref(false)
const fetchError = ref<string | null>(null)
const showEditModal = ref(false)

const fetchBook = async () => {
  isFetching.value = true
  fetchError.value = null

  try {
    selectedBook.value = await bookStore.getBookById(bookId)
  } catch (error) {
    console.error('Error fetching book:', error)
    fetchError.value = 'Failed to load book details. Please try again.'
  } finally {
    isFetching.value = false
  }
}

const handleEditClick = (book: Book) => {
  selectedBook.value = book
  showEditModal.value = true
  console.log('Edit book', book)
}

const retryFetch = () => {
  console.log('Retrying fetch for book ID:', bookId)
  fetchBook()
}

const handleBookUpdated = (message: string) => {
  console.log('Book updated:', message)
  fetchBook()
}

const handleUpdateError = (message: string) => {
  console.error('Update error:', message)
}

onMounted(() => {
  fetchBook()
  console.log('Book ID:', typeof bookId)
})
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
      <!-- Loading State -->
      <DetailsLoading v-if="isFetching" />

      <!-- Error State -->
      <UCard
        v-else-if="fetchError"
        variant="soft"
        class="bg-muted rounded-lg w-full max-w-6xl"
        :ui="{ body: 'p-6 ring-0', header: 'ring' }"
      >
        <template #header>
          <div class="flex justify-between items-center">
            <h3 class="font-semibold text-red-600 text-xl">Error Loading Book</h3>
          </div>
        </template>

        <div class="py-12 text-center">
          <UIcon
            name="i-heroicons-exclamation-triangle"
            class="mx-auto mb-4 w-16 h-16 text-red-500"
          />
          <p class="mb-4 text-muted">{{ fetchError }}</p>
          <UButton
            @click="retryFetch()"
            color="primary"
            variant="outline"
            icon="i-heroicons-arrow-path"
          >
            Try Again
          </UButton>
        </div>
      </UCard>

      <!-- Book Content -->
      <UCard
        v-else-if="selectedBook"
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
                :src="getImageUrl(selectedBook.imageUrl)"
                :alt="selectedBook.bookName"
                class="shadow-md rounded-lg w-full h-96 object-cover"
                loading="lazy"
                @error="($event.target as HTMLImageElement).src = '/placeholder-book.jpg'"
              />
            </div>
            <div class="flex flex-col flex-1 justify-between space-y-6">
              <div>
                <h2 class="mb-1 font-bold text-2xl">
                  {{ selectedBook.bookName }}
                </h2>
                <p class="text-lg italic">by {{ selectedBook.author }}</p>
              </div>

              <div class="gap-6 grid grid-cols-1 sm:grid-cols-2">
                <div>
                  <p class="mb-1 font-medium text-sm">Category</p>
                  <UBadge :label="selectedBook.category" color="primary" variant="soft" />
                </div>
                <div>
                  <p class="mb-1 font-medium text-sm">Price</p>
                  <p class="font-semibold text-green-600 text-lg">
                    GH₵{{ selectedBook.price.toFixed(2) }}
                  </p>
                </div>
                <div>
                  <p class="mb-1 font-medium text-sm">Stock</p>
                  <UBadge label="In Stock" color="success" variant="soft" />
                </div>
                <div>
                  <p class="mb-1 font-medium text-sm">Created</p>
                  <p class="">
                    {{ useDateFormat(selectedBook.createdAt, 'dddd Do MMMM, YYYY') }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div>
            <p class="mb-2 font-medium text-sm">Description</p>
            <p class="leading-relaxed">
              {{ selectedBook.description }}
            </p>
          </div>
        </div>

        <template #footer>
          <div class="flex justify-end gap-3">
            <UButton
              color="primary"
              variant="outline"
              size="lg"
              icon="i-heroicons-pencil-square"
              label="Edit"
              class="cursor-pointer"
              @click="handleEditClick(selectedBook)"
            />
          </div>
        </template>
      </UCard>

      <!-- No Book Found State -->
      <UCard
        v-else
        variant="soft"
        class="bg-muted rounded-lg w-full max-w-6xl"
        :ui="{ body: 'p-6 ring-0', header: 'ring' }"
      >
        <template #header>
          <div class="flex justify-between items-center">
            <h3 class="font-semibold text-xl">Book Not Found</h3>
          </div>
        </template>

        <div class="py-12 text-center">
          <UIcon name="i-heroicons-book-open" class="mx-auto mb-4 w-16 h-16 text-gray-400" />
          <p class="mb-4 text-gray-600 dark:text-gray-400">
            The book you're looking for doesn't exist or has been removed.
          </p>
          <UButton
            @click="$router.push({ name: 'books' })"
            color="primary"
            variant="outline"
            icon="i-heroicons-arrow-left"
          >
            Back to Books
          </UButton>
        </div>
      </UCard>
    </div>

    <EditModal
      v-if="showEditModal && selectedBook"
      v-model:open="showEditModal"
      :book="selectedBook"
      @success="handleBookUpdated"
      @error="handleUpdateError"
    />
  </div>
</template>

<style scoped></style>
