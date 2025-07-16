<script setup lang="ts">
import { ref, onUnmounted, reactive } from 'vue'

const images = ref<File[]>([])
const links = ref<string[]>([])
const dragActive = ref(false)
const imageUrls = reactive(new Map<number, string>())
const fileInputRef = ref<HTMLInputElement | null>(null)

// Create object URL for an image
function getImageUrl(img: File, idx: number): string {
  if (!imageUrls.has(idx)) {
    imageUrls.set(idx, URL.createObjectURL(img))
  }
  return imageUrls.get(idx) || ''
}

// Clean up object URLs to prevent memory leaks
function revokeImageUrls() {
  imageUrls.forEach((url) => URL.revokeObjectURL(url))
  imageUrls.clear()
}

function removeImage(idx: number) {
  if (imageUrls.has(idx)) {
    URL.revokeObjectURL(imageUrls.get(idx)!)
    imageUrls.delete(idx)
  }
  images.value.splice(idx, 1)
}

function removeLink(idx: number) {
  links.value.splice(idx, 1)
}

function handleDrop(e: DragEvent) {
  dragActive.value = false
  if (!e.dataTransfer) return

  // Handle images
  const files = Array.from(e.dataTransfer.files)
  files.forEach((file) => {
    if (file.type.startsWith('image/')) {
      images.value.push(file)
    }
  })

  // Handle links
  const url = e.dataTransfer.getData('text/uri-list') || e.dataTransfer.getData('text/plain')
  if (url && /^https?:\/\//.test(url)) {
    links.value.push(url)
  }
}

function handleDragOver(e: DragEvent) {
  e.preventDefault()
  dragActive.value = true
}

function handleDragLeave(e: DragEvent) {
  dragActive.value = false
}

// Handle file selection from input
function handleFileSelect(e: Event) {
  const target = e.target as HTMLInputElement
  if (target.files) {
    const files = Array.from(target.files)
    files.forEach((file) => {
      if (file.type.startsWith('image/')) {
        images.value.push(file)
      }
    })
    // Reset the input so the same file can be selected multiple times
    if (fileInputRef.value) {
      fileInputRef.value.value = ''
    }
  }
}

// Trigger file input click
function openFileDialog() {
  if (fileInputRef.value) {
    fileInputRef.value.click()
  }
}

// Clean up object URLs when component is unmounted
onUnmounted(() => {
  revokeImageUrls()
})
</script>

<template>
  <div class="flex flex-col justify-center items-center min-h-screen">
    <h1 class="mb-6 font-bold text-2xl">Drag and Drop Images or Links</h1>

    <div
      class="flex flex-col justify-center items-center border-4 border-dashed rounded-lg w-96 h-48 transition-colors duration-200"
      :class="dragActive ? 'border-blue-500 bg-blue-50' : 'border-gray-300 bg-white'"
      @drop.prevent="handleDrop"
      @dragover.prevent="handleDragOver"
      @dragleave="handleDragLeave"
      @click="openFileDialog"
    >
      <span class="text-gray-500 text-lg">Drop images or links here</span>
      <span class="mt-2 text-gray-400 text-sm">or click to select files</span>

      <!-- Hidden file input -->
      <input
        ref="fileInputRef"
        type="file"
        accept="image/*"
        multiple
        class="hidden"
        @change="handleFileSelect"
      />
    </div>

    <div class="mt-8 w-96">
      <div v-if="images.length" class="mb-6">
        <h2 class="mb-2 font-semibold">Images:</h2>
        <div class="flex flex-wrap gap-2">
          <div v-for="(img, idx) in images" :key="idx" class="group relative">
            <img
              :src="getImageUrl(img, idx)"
              class="border rounded w-20 h-20 object-cover"
              :alt="img.name"
            />
            <button
              @click="removeImage(idx)"
              class="-top-2 -right-2 absolute flex justify-center items-center bg-red-500 opacity-0 group-hover:opacity-100 rounded-full w-5 h-5 text-white transition-opacity"
              title="Remove image"
            >
              ×
            </button>
          </div>
        </div>
      </div>
      <div v-if="links.length">
        <h2 class="mb-2 font-semibold">Links:</h2>
        <ul class="pl-5 list-disc">
          <li v-for="(link, idx) in links" :key="idx" class="group relative">
            <a :href="link" target="_blank" class="text-blue-600 underline">{{ link }}</a>
            <button
              @click="removeLink(idx)"
              class="opacity-0 group-hover:opacity-100 ml-2 text-red-500 transition-opacity"
              title="Remove link"
            >
              ×
            </button>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<style scoped>
.hidden {
  display: none;
}
</style>
