<script setup lang="ts">
import { useBookStore } from '@/stores/BookStore'
import type { Book } from '@/Types/types'
import { showToast } from '@/utils/toastHandler'
import type { FormSubmitEvent } from '@nuxt/ui'
import { storeToRefs } from 'pinia'
import { computed, nextTick, onMounted, onUnmounted, watch } from 'vue'
import { ref } from 'vue'

const props = defineProps<{
  book?: Book | null
  open?: boolean
}>()

const emit = defineEmits<{
  (e: 'submit', book: Omit<Book, 'id'>): void
  // (e: 'success', message: string): void
  // (e: 'error', message: string): void
  (e: 'update:open', value: boolean): void
}>()

const formRef = ref<HTMLFormElement | null>(null)
const isOpen = ref<boolean>(props.open || false)
const originalBookData = ref<Book | null>(null)
const bookstore = useBookStore()
const { bookFormData, imagePreview, isLoading, validationErrors, isFormValid } =
  storeToRefs(bookstore)
const { clearBookFormdata, clearImage, handleFileChange, updateBook, validate, createBook } =
  bookstore

const categories = [
  'Fiction',
  'Non-Fiction',
  'Mystery',
  'Romance',
  'Science Fiction',
  'Fantasy',
  'Biography',
  'History',
  'Self-Help',
  'Business',
  'Poetry',
  'Drama',
  'Horror',
  'Thriller',
  'Dystopian',
]

const isEditMode = computed<boolean>(() => !!props.book)

const populateForm = (book: Book) => {
  originalBookData.value = { ...book }

  bookFormData.value.bookName = book.bookName
  bookFormData.value.category = book.category
  bookFormData.value.author = book.author
  bookFormData.value.price = book.price
  bookFormData.value.description = book.description
  bookFormData.value.imageUrl = book.imageUrl
  bookFormData.value.createdAt = book.createdAt
  bookFormData.value.updatedAt = book.updatedAt
  if ('bookId' in bookFormData.value) {
    ;(bookFormData.value as any).bookId = book.bookId
  }
}

const hasChanges = computed<boolean>(() => {
  if (!originalBookData.value || !isEditMode.value) return true

  const current = bookFormData.value
  const original = originalBookData.value

  return (
    current.bookName?.trim() !== original.bookName ||
    current.category !== original.category ||
    current.author?.trim() !== original.author ||
    Number(current.price) !== Number(original.price) ||
    current.description?.trim() !== original.description ||
    current.imageUrl !== original.imageUrl
  )
})

const openModal = () => {
  isOpen.value = true
  emit('update:open', true)

  if (props.book) {
    nextTick(() => {
      populateForm(props.book!)
    })
  } else {
    clearBookFormdata()
    clearImage()
  }
}

const closeModal = () => {
  isOpen.value = false
  emit('update:open', false)
  clearBookFormdata()
  clearImage()
}

const validateBookData = (data: any): boolean => {
  const requiredFields = ['bookName', 'category', 'author', 'price', 'description']

  for (const field of requiredFields) {
    if (!data[field] || (typeof data[field] === 'string' && data[field].trim() === '')) {
      console.error(`Validation failed: ${field} is required`)
      return false
    }
  }

  if (isNaN(Number(data.price)) || Number(data.price) <= 0) {
    console.error('Validation failed: price must be a positive number')
    return false
  }

  return true
}

const handleBookSubmit = async (event: FormSubmitEvent<typeof bookFormData>) => {
  isLoading.value = true
  try {
    let result
    let successMessage = ''

    if (isEditMode.value && props.book) {
      if (!hasChanges.value) {
        showToast({
          title: 'No Changes',
          description: 'No changes detected. Nothing to update.',
          color: 'warning',
          icon: 'i-lucide-alert-triangle',
        })
        isLoading.value = false
        return
      }

      const updateData = {
        bookId: props.book.bookId,
        bookName: bookFormData.value.bookName.trim(),
        category: bookFormData.value.category,
        author: bookFormData.value.author.trim(),
        price: Number(bookFormData.value.price),
        description: bookFormData.value.description.trim(),
        imageUrl: bookFormData.value.imageUrl || '',
        createdAt: props.book.createdAt,
        updatedAt: new Date().toISOString(),
      }
      if (!validateBookData(updateData)) {
        throw new Error('Validation failed: Please check all required fields')
      }
      result = await updateBook(props.book.bookId, updateData)

      successMessage = 'Book updated successfully!'
      showToast({
        title: 'Success',
        description: successMessage,
        color: 'success',
        icon: 'i-lucide-check-circle',
      })
    } else {
      result = await createBook(bookFormData.value as Omit<Book, 'id'>)
      successMessage = 'Book added successfully!'
      showToast({
        title: 'Success',
        description: successMessage,
        color: 'success',
        icon: 'i-lucide-check-circle',
      })
    }

    // emit('success', successMessage)
    clearBookFormdata()
    clearImage()
    setTimeout(() => {
      closeModal()
    }, 200)
  } catch (error: any) {
    const errorTitle = isEditMode.value ? 'Book Update Error' : 'Book Creation Error'
    console.error(`Error ${isEditMode.value ? 'updating' : 'creating'} book:`, error)

    const errorMessage =
      error.response?.data?.message ||
      error.message ||
      `Failed to ${isEditMode.value ? 'update' : 'create'} book`

    showToast({
      title: errorTitle,
      description: errorMessage,
      color: 'error',
      icon: 'i-lucide-alert-triangle',
    })
  } finally {
    isLoading.value = false
  }
}

const submitForm = async () => {
  if (!formRef.value) throw new Error('Form reference is not set')
  formRef.value.submit()
}

watch(
  () => props.open,
  (newValue) => {
    if (newValue !== undefined) {
      isOpen.value = newValue
    }
  },
)

watch(
  () => props.book,
  (newBook) => {
    if (newBook && isOpen.value) {
      nextTick(() => populateForm(newBook))
    }
  },
  { immediate: true, deep: true },
)

watch(isOpen, (newValue) => {
  emit('update:open', newValue)
  if (!newValue) {
    clearBookFormdata()
    clearImage()
  } else if (newValue && props.book) {
    nextTick(() => {
      populateForm(props.book!)
    })
  }
})

onUnmounted(() => {
  if (imagePreview.value) {
    URL.revokeObjectURL(imagePreview.value)
  }
})
</script>

<template>
  <div>
    <UModal
      :title="isEditMode ? 'Edit Book' : 'Add New Book'"
      :description="
        isEditMode ? 'Update the book information' : 'Complete The Form To Add A New Book'
      "
      :dismissible="false"
      class="top-50 cursor-pointer"
      v-model:open="isOpen"
      :ui="{
        footer: 'justify-end',
        body: 'ring',

        overlay: 'z-[50] bg-black/50 backdrop-blur-sm',
        content: 'z-[60] max-w-3xl w-full rounded-lg',
      }"
    >
      <UButton
        v-if="!isEditMode"
        color="primary"
        variant="solid"
        size="lg"
        icon="i-heroicons-plus"
        class="items-center w-full truncate cursor-pointer"
        @click="openModal"
      >
        Add Book
      </UButton>

      <template #body>
        <div class="mx-auto w-full">
          <div
            v-if="isEditMode && props.book"
            class="bg-blue-50 dark:bg-blue-900/20 mb-4 p-3 border border-blue-200 dark:border-blue-800 rounded-lg"
          >
            <p class="font-medium text-blue-800 dark:text-blue-200 text-sm">
              Editing: {{ props.book.bookName }} by {{ props.book.author }}
            </p>
          </div>

          <UForm
            ref="formRef"
            :state="bookFormData"
            :validate="validate"
            :validate-on="['blur', 'change', 'change', 'input']"
            @submit.prevent="handleBookSubmit"
          >
            <div class="gap-4 grid grid-cols-1 sm:grid-cols-2">
              <UFormField label="Book Name" name="bookName" required>
                <UInput
                  size="xl"
                  placeholder="Enter The Book Name"
                  class="w-full"
                  v-model="bookFormData.bookName"
                />
              </UFormField>

              <UFormField label="Category" name="category" required>
                <USelectMenu
                  placeholder="Select the book category"
                  size="xl"
                  :items="categories"
                  class="w-full"
                  v-model="bookFormData.category"
                  arrow
                  :ui="{
                    content: 'z-[90]',
                  }"
                  :content="{
                    align: 'center',
                    side: 'bottom',
                    sideOffset: 8,
                    position: 'popper',
                  }"
                />
              </UFormField>
            </div>

            <div class="gap-4 grid grid-cols-1 sm:grid-cols-2 mt-6">
              <UFormField label="Author" name="author" required>
                <UInput
                  size="xl"
                  placeholder="Enter the author's name"
                  class="w-full"
                  v-model="bookFormData.author"
                />
              </UFormField>

              <UFormField label="Price" name="price" required>
                <UInputNumber
                  v-model="bookFormData.price"
                  placeholder="Enter the price of the book"
                  size="xl"
                  :min="0"
                  :step="1"
                  :format-options="{
                    style: 'currency',
                    currency: 'GHC',
                    currencyDisplay: 'code',
                    currencySign: 'accounting',
                  }"
                  class="w-full"
                />
              </UFormField>
            </div>

            <UFormField label="Description" name="description" required class="mt-4">
              <UTextarea
                size="xl"
                placeholder="Enter a brief description of the book"
                class="w-full"
                v-model="bookFormData.description"
              />
            </UFormField>

            <div class="mt-6 w-full">
              <UFormField label="Book Image" name="imageUrl" required class="mt-4">
                <UInput
                  type="file"
                  size="xl"
                  placeholder="Select a book image"
                  class="w-full"
                  accept="image/*"
                  leading-icon="i-lucide-image"
                  :ui="{
                    base: 'flex items-center justify-center py-4 border-2 border-dashed border-accented outline-none cursor-pointer rounded-lg',
                    leadingIcon: 'size-7',
                  }"
                  @change="handleFileChange"
                />
              </UFormField>
            </div>
          </UForm>

          <div v-if="imagePreview" class="relative mt-4">
            <img :src="imagePreview" alt="Preview" class="rounded-md w-full object-cover" />
            <UButton
              type="button"
              icon="i-lucide-x"
              size="xs"
              class="top-1 right-1 absolute flex justify-center items-center bg-red-500 hover:bg-red-600 rounded-full text-white text-sm"
              @click="clearImage"
            />
          </div>

          <div v-else-if="isEditMode && bookFormData.imageUrl" class="mt-4">
            <p class="mb-2 text-gray-600 dark:text-gray-400 text-sm">Current Image:</p>
            <div class="relative">
              <img
                :src="bookFormData.imageUrl"
                alt="Current book image"
                class="border border-gray-200 dark:border-gray-700 rounded-md w-full h-48 object-cover"
                @error="($event.target as HTMLImageElement).src = '/placeholder-book.jpg'"
              />
              <div class="top-2 left-2 absolute">
                <UBadge label="Current" color="info" variant="solid" size="xs" />
              </div>
            </div>
            <p class="mt-1 text-gray-500 text-xs">Upload a new image to replace the current one</p>
          </div>

          <div v-if="bookFormData.imageUrl && !imagePreview" class="mt-2 text-gray-600 text-sm">
            Image URL: {{ bookFormData.imageUrl }}
          </div>
        </div>
      </template>

      <template #footer>
        <UButton
          label="Cancel"
          color="error"
          variant="outline"
          @click="closeModal"
          class="mr-2 cursor-pointer"
        />
        <UButton
          :label="isEditMode ? 'Update Book' : 'Add Book'"
          loading-icon="i-lucide-loader"
          :loading="isLoading"
          :color="isEditMode ? 'warning' : 'primary'"
          class="cursor-pointer"
          :disabled="!isFormValid"
          @click="submitForm"
        />
      </template>
    </UModal>
  </div>
</template>

<style scoped></style>
