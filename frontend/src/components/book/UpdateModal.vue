<template>
  <div>
    <!-- Trigger Button -->
    <button
      @click="isOpen = true"
      class="bg-blue-600 hover:bg-blue-700 px-4 py-2 rounded-lg font-medium text-white transition-colors"
    >
      Add Book
    </button>

    <!-- Modal Overlay -->
    <div
      v-if="isOpen"
      class="z-50 fixed inset-0 flex justify-center items-center bg-black bg-opacity-50 p-4"
      @click="closeModal"
    >
      <!-- Modal Content -->
      <div class="bg-white rounded-lg w-full max-w-md max-h-[90vh] overflow-y-auto" @click.stop>
        <!-- Header -->
        <div class="flex justify-between items-center p-6 border-b">
          <h2 class="font-semibold text-gray-800 text-xl">Add New Book</h2>
          <button @click="closeModal" class="text-gray-400 hover:text-gray-600 text-2xl">×</button>
        </div>

        <!-- Form -->
        <form @submit.prevent="submitForm" class="space-y-4 p-6">
          <!-- Book Name -->
          <div>
            <label class="block mb-1 font-medium text-gray-700 text-sm"> Book Name * </label>
            <input
              v-model="formData.bookName"
              type="text"
              class="px-3 py-2 border border-gray-300 focus:border-transparent rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 w-full"
              :class="{ 'border-red-500': errors.bookName }"
              placeholder="Enter book name"
            />
            <span v-if="errors.bookName" class="text-red-500 text-sm">{{ errors.bookName }}</span>
          </div>

          <!-- Category -->
          <div>
            <label class="block mb-1 font-medium text-gray-700 text-sm"> Category * </label>
            <select
              v-model="formData.category"
              class="px-3 py-2 border border-gray-300 focus:border-transparent rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 w-full"
              :class="{ 'border-red-500': errors.category }"
            >
              <option value="">Select a category</option>
              <option value="fiction">Fiction</option>
              <option value="non-fiction">Non-Fiction</option>
              <option value="mystery">Mystery</option>
              <option value="romance">Romance</option>
              <option value="science-fiction">Science Fiction</option>
              <option value="fantasy">Fantasy</option>
              <option value="biography">Biography</option>
              <option value="history">History</option>
              <option value="self-help">Self Help</option>
              <option value="education">Education</option>
            </select>
            <span v-if="errors.category" class="text-red-500 text-sm">{{ errors.category }}</span>
          </div>

          <!-- Price -->
          <div>
            <label class="block mb-1 font-medium text-gray-700 text-sm"> Price * </label>
            <div class="relative">
              <span class="top-2 left-3 absolute text-gray-500">$</span>
              <input
                v-model="formData.price"
                type="number"
                step="0.01"
                min="0"
                class="py-2 pr-3 pl-8 border border-gray-300 focus:border-transparent rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 w-full"
                :class="{ 'border-red-500': errors.price }"
                placeholder="0.00"
              />
            </div>
            <span v-if="errors.price" class="text-red-500 text-sm">{{ errors.price }}</span>
          </div>

          <!-- Description -->
          <div>
            <label class="block mb-1 font-medium text-gray-700 text-sm"> Description </label>
            <textarea
              v-model="formData.description"
              rows="3"
              class="px-3 py-2 border border-gray-300 focus:border-transparent rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 w-full resize-none"
              placeholder="Enter book description"
            ></textarea>
          </div>

          <!-- Image Selector -->
          <div>
            <label class="block mb-1 font-medium text-gray-700 text-sm"> Book Cover Image </label>
            <div class="space-y-2">
              <input
                ref="fileInput"
                type="file"
                accept="image/*"
                @change="handleImageSelect"
                class="hidden"
              />
              <button
                type="button"
                @click="$refs.fileInput.click()"
                class="px-3 py-2 border-2 border-gray-300 hover:border-gray-400 focus:border-transparent border-dashed rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 w-full transition-colors"
              >
                <div class="flex justify-center items-center space-x-2">
                  <svg
                    class="w-5 h-5 text-gray-400"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M12 6v6m0 0v6m0-6h6m-6 0H6"
                    ></path>
                  </svg>
                  <span class="text-gray-600">Choose Image</span>
                </div>
              </button>

              <!-- Image Preview -->
              <div v-if="imagePreview" class="relative">
                <img
                  :src="imagePreview"
                  alt="Preview"
                  class="rounded-md w-full h-32 object-cover"
                />
                <button
                  type="button"
                  @click="removeImage"
                  class="top-1 right-1 absolute flex justify-center items-center bg-red-500 hover:bg-red-600 rounded-full w-6 h-6 text-white text-sm"
                >
                  ×
                </button>
              </div>

              <div v-if="formData.imageUrl" class="text-gray-600 text-sm">
                Image URL: {{ formData.imageUrl }}
              </div>
            </div>
          </div>

          <!-- Form Actions -->
          <div class="flex space-x-3 pt-4">
            <button
              type="button"
              @click="closeModal"
              class="flex-1 hover:bg-gray-50 px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-gray-500 text-gray-700 transition-colors"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="isSubmitting"
              class="flex-1 bg-blue-600 hover:bg-blue-700 disabled:opacity-50 px-4 py-2 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 text-white transition-colors disabled:cursor-not-allowed"
            >
              {{ isSubmitting ? 'Adding...' : 'Add Book' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'

interface BookFormData {
  bookName: string
  category: string
  price: number | null
  description: string
  imageUrl: string
}

interface FormErrors {
  bookName?: string
  category?: string
  price?: string
}

// Reactive state
const isOpen = ref(false)
const isSubmitting = ref(false)
const imagePreview = ref<string>('')
const fileInput = ref<HTMLInputElement>()

// Form data
const formData = reactive<BookFormData>({
  bookName: '',
  category: '',
  price: null,
  description: '',
  imageUrl: '',
})

// Form errors
const errors = reactive<FormErrors>({})

// Convert file to base64 URL (simulating image upload)
const convertFileToUrl = (file: File): Promise<string> => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    reader.onload = () => {
      // In a real app, you would upload to a server and get back a URL
      // For demo purposes, we'll use the base64 data URL
      const result = reader.result as string
      resolve(result)
    }
    reader.onerror = reject
    reader.readAsDataURL(file)
  })
}

// Handle image selection
const handleImageSelect = async (event: Event) => {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]

  if (file) {
    try {
      // Show preview
      imagePreview.value = URL.createObjectURL(file)

      // Convert to URL (in real app, upload to server)
      const imageUrl = await convertFileToUrl(file)
      formData.imageUrl = imageUrl
    } catch (error) {
      console.error('Error processing image:', error)
      alert('Error processing image. Please try again.')
    }
  }
}

// Remove selected image
const removeImage = () => {
  imagePreview.value = ''
  formData.imageUrl = ''
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

// Validate form
const validateForm = (): boolean => {
  // Clear previous errors
  Object.keys(errors).forEach((key) => {
    delete errors[key as keyof FormErrors]
  })

  let isValid = true

  // Validate book name
  if (!formData.bookName.trim()) {
    errors.bookName = 'Book name is required'
    isValid = false
  }

  // Validate category
  if (!formData.category) {
    errors.category = 'Category is required'
    isValid = false
  }

  // Validate price
  if (formData.price === null || formData.price < 0) {
    errors.price = 'Valid price is required'
    isValid = false
  }

  return isValid
}

// Submit form
const submitForm = async () => {
  if (!validateForm()) return

  isSubmitting.value = true

  try {
    // Simulate API call
    await new Promise((resolve) => setTimeout(resolve, 1000))

    // Log the form data (in real app, send to API)
    console.log('Book added:', {
      bookName: formData.bookName,
      category: formData.category,
      price: formData.price,
      description: formData.description,
      imageUrl: formData.imageUrl,
    })

    // Show success message
    alert('Book added successfully!')

    // Reset form and close modal
    resetForm()
    closeModal()
  } catch (error) {
    console.error('Error adding book:', error)
    alert('Error adding book. Please try again.')
  } finally {
    isSubmitting.value = false
  }
}

// Reset form
const resetForm = () => {
  formData.bookName = ''
  formData.category = ''
  formData.price = null
  formData.description = ''
  formData.imageUrl = ''
  imagePreview.value = ''
  Object.keys(errors).forEach((key) => {
    delete errors[key as keyof FormErrors]
  })
  if (fileInput.value) {
    fileInput.value.value = ''
  }
}

// Close modal
const closeModal = () => {
  isOpen.value = false
  resetForm()
}
</script>
