<script setup lang="ts">
import { getImageUrl } from '@/services/Api'
import type { Book } from '@/Types/types'
import { ref } from 'vue'
import ConfirmationModal from '@/components/app/ConfirmationModal.vue'

const props = defineProps<{
  books: Book[]
  editBook: (book: Book) => void
  deleteBook: (book: Book) => void
  showBookDetail: (book: Book) => void
}>()

const emit = defineEmits<{
  deleteBook: [book: Book]
}>()

const showDeleteConfirmation = ref(false)
const bookToDelete = ref<Book | null>(null)

const handleDeleteClick = (book: Book) => {
  bookToDelete.value = book
  showDeleteConfirmation.value = true
}

const confirmDelete = () => {
  if (bookToDelete.value) {
    emit('deleteBook', bookToDelete.value)
    bookToDelete.value = null
    showDeleteConfirmation.value = false
  }
}
</script>

<template>
  <div class="gap-6 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4" v-auto-animate>
    <UCard
      v-for="book in books"
      :key="book.id"
      :ui="{
        body: 'p-4',
      }"
      class="group bg-muted hover:shadow-md transition-shadow duration-200 cursor-pointer"
      @click="showBookDetail(book)"
    >
      <div class="space-y-4">
        <div class="relative">
          <img
            :src="getImageUrl(book.imageUrl)"
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
          <p class="text-gray-600 dark:text-gray-300 text-sm">by {{ book.author }}</p>
          <div class="flex justify-between items-center">
            <UBadge color="primary" variant="soft">
              {{ book.category }}
            </UBadge>
            <span class="font-bold text-gray-900 dark:text-white text-lg"> ${{ book.price }} </span>
          </div>
        </div>

        <!-- Action Buttons -->
        <div
          class="flex justify-between items-center opacity-0 group-hover:opacity-100 pt-2 border-gray-200 dark:border-gray-700 border-t transition-opacity duration-200"
        >
          <UButton
            @click.stop="editBook(book)"
            color="primary"
            variant="outline"
            size="sm"
            icon="i-heroicons-pencil-square"
            label="Edit"
            class="cursor-pointer"
          />

          <UButton
            @click.stop="handleDeleteClick(book)"
            color="error"
            variant="outline"
            size="sm"
            icon="i-heroicons-trash"
            label="Delete"
            class="cursor-pointer"
          />
        </div>
      </div>
    </UCard>

    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      v-model="showDeleteConfirmation"
      action-type="remove"
      :book="bookToDelete as Book"
      @confirm="confirmDelete"
    />
  </div>
</template>
