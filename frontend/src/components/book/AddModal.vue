<script setup lang="ts">
import { useBookStore } from '@/stores/BookStore'
import type { Book } from '@/Types/types'
import type { FormSubmitEvent } from '@nuxt/ui'
import { storeToRefs } from 'pinia'
import { onUnmounted, watch } from 'vue'
import { ref } from 'vue'

// const props = defineProps<{
//   book: Book
// }>()

const emit = defineEmits<{
  (e: 'submit', book: Omit<Book, 'id'>): void
  (e: 'success', message: string): void
  (e: 'error', message: string): void
}>()

const formRef = ref<HTMLFormElement | null>(null)
const isOpen = ref<boolean>(false)
const bookstore = useBookStore()
const { bookFormData, imagePreview, isLoading, validationErrors, isFormValid } =
  storeToRefs(bookstore)
const { clearBookFormdata, clearImage, handleFileChange, createBook, validate } = bookstore

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

// const book = ref<Book>({ ...props.book })

const handleBookSubmit = async (event: FormSubmitEvent<typeof bookFormData>) => {
  isLoading.value = true
  try {
    const result = await createBook(bookFormData.value as Omit<Book, 'id'>)
    // Emit success event
    emit('success', 'Book added successfully!')

    clearBookFormdata()
    clearImage()
    isOpen.value = false
  } catch (error: any) {
    console.error('Error creating book:', error)

    const errorMessage = error.response?.data?.message || error.message || 'Failed to create book'
    emit('error', errorMessage)
  } finally {
    isLoading.value = false
  }
}

const submitForm = async () => {
  if (!formRef.value) throw new Error('Form reference is not set')
  formRef.value.submit()
}

watch(
  () => isOpen.value,
  (newValue: boolean) => {
    isOpen.value = newValue
    console.log('Modal open state changed:', newValue)

    if (!newValue) {
      clearBookFormdata()
      clearImage()
    }
  },
)

onUnmounted(() => {
  if (imagePreview.value) {
    URL.revokeObjectURL(imagePreview.value)
  }
})
</script>

<template>
  <div>
    <UModal
      title="Add New Book"
      description="Complete The Form To Add A New Book"
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
        color="primary"
        variant="solid"
        size="lg"
        icon="i-heroicons-plus"
        class="items-center w-full truncate"
      >
        Add Book
      </UButton>

      <template #body>
        <div class="mx-auto w-full">
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

          <div v-if="bookFormData.imageUrl" class="text-gray-600 text-sm">
            Image URL: {{ bookFormData.imageUrl }}
          </div>
        </div>
      </template>

      <template #footer="{ close }">
        <UButton
          label="Cancel"
          color="error"
          variant="outline"
          @click="close"
          class="mr-2 cursor-pointer"
        />
        <UButton
          label="Submit"
          loading-icon="i-lucide-loader"
          :loading="isLoading"
          color="primary"
          class="cursor-pointer"
          :disabled="!isFormValid"
          @click="submitForm"
        />
      </template>
    </UModal>
  </div>
</template>

<style scoped></style>
