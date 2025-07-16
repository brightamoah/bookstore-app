<script setup lang="ts">
import { getImageUrl } from '@/services/Api'
import type { Book } from '@/Types/types'
import { showToast } from '@/utils/toastHandler'
import { ref } from 'vue'

const props = defineProps<{
  books: Book[]
  isLoading: boolean
  editBook: (book: Book) => void
  showBookDetail: (book: Book) => void
}>()

const emit = defineEmits<{
  deleteBook: [book: Book]
}>()

const bookToDelete = ref<Book | null>(null)
const showDeleteModal = ref<boolean>(false)

const bookToEdit = ref<Book | null>(null)
const showEditModal = ref<boolean>(false)

const handleDeleteClick = (book: Book) => {
  bookToDelete.value = book
  showDeleteModal.value = true
  console.log('Delete book:', book)
}

const handleEditClick = (book: Book) => {
  bookToEdit.value = book
  showEditModal.value = true
  console.log('Edit book', book)
}

const confirmDelete = () => {
  if (bookToDelete.value) {
    emit('deleteBook', bookToDelete.value)
    bookToDelete.value = null
    showDeleteModal.value = false
  }
}

const cancelDelete = () => {
  bookToDelete.value = null
  showDeleteModal.value = false
}

// const handleEditSuccess = (message: string) => {
//   console.log('Edit success:', message)
//   showEditModal.value = false
//   bookToEdit.value = null
// }

// const handleEditError = (message: string) => {
//   console.error('Edit error:', message)
// }
</script>

<template>
  <div class="gap-6 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4" v-auto-animate>
    <UCard
      v-for="book in books"
      :key="book.bookId"
      :ui="{
        body: 'p-4',
      }"
      class="group bg-muted hover:shadow-md transition-shadow duration-200"
    >
      <div class="space-y-4">
        <div class="relative">
          <img
            :src="getImageUrl(book.imageUrl)"
            :alt="book.bookName"
            class="rounded-lg w-full h-48 object-cover cursor-pointer"
            @click="showBookDetail(book)"
          />
          <div class="top-2 right-2 absolute">
            <UBadge color="success" variant="solid" size="sm"> In Stock </UBadge>
          </div>
        </div>

        <div class="space-y-2">
          <h3
            class="font-semibold text-lg line-clamp-2 cursor-pointer"
            @click="showBookDetail(book)"
          >
            {{ book.bookName }}
          </h3>
          <p class="text-gray-600 dark:text-gray-300 text-sm">by {{ book.author }}</p>
          <div class="flex justify-between items-center">
            <UBadge color="primary" variant="soft">
              {{ book.category }}
            </UBadge>
            <span class="font-bold text-gray-900 dark:text-white text-lg">
              GH₵{{ book.price.toFixed(2) }}
            </span>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-between items-center pt-3 border-muted border-t-2">
          <UButton
            @click="handleEditClick(book)"
            color="primary"
            variant="outline"
            size="sm"
            icon="i-heroicons-pencil-square"
            label="Edit"
            class="cursor-pointer"
          />

          <UButton
            color="error"
            variant="outline"
            size="sm"
            icon="i-heroicons-trash"
            label="Delete"
            class="cursor-pointer"
            @click="handleDeleteClick(book)"
          />
        </div>
      </div>
    </UCard>

    <div class="justify-center items-center mx-auto w-full">
      <EditModal
        v-if="showEditModal && bookToEdit"
        :book="bookToEdit"
        v-model:open="showEditModal"
      />

      <ConfirmationModal
        v-if="showDeleteModal"
        action-type="delete"
        v-model:open="showDeleteModal"
        :book="bookToDelete"
        :loading="isLoading"
        @confirm="confirmDelete"
        @cancel="cancelDelete"
      />
    </div>
  </div>
</template>
