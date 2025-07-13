<script setup lang="ts">
import { getImageUrl } from '@/services/Api'
import { useSearchStore } from '@/stores/SearchStore'
import type { Book } from '@/Types/types'
import { useDateFormat, useEventListener } from '@vueuse/core'
import { storeToRefs } from 'pinia'
import { nextTick, ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const isOpen = ref(false)
const searchInputRef = ref<HTMLInputElement | null>(null)

const searchStore = useSearchStore()

const {
  searchQuery,
  searchResults,
  isSearching,
  isEmpty,
  searchError,
  hasQuery,
  hasResults,
  cacheSize,
} = storeToRefs(searchStore)

const openModal = async () => {
  isOpen.value = true
  await nextTick()
  searchInputRef.value?.focus()
}

const closeModal = () => {
  isOpen.value = false
  clearSearch()
}

function clearSearch() {
  searchStore.clearSearch()
  searchInputRef.value?.blur()
}

const handleKeydown = (e: KeyboardEvent) => {
  if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
    e.preventDefault()
    openModal()
  }

  if (e.key === 'Escape' && isOpen.value) {
    e.preventDefault()
    closeModal()
  }

  if (isOpen.value && hasResults.value) {
    // You can add arrow key navigation here if needed
  }
}

function goToBook(bookId: Book['bookId']) {
  router.push({ name: 'book-details', params: { id: bookId } })
  closeModal()
}

const formatDate = (dateString: string) => {
  return useDateFormat(new Date(dateString), 'MMM DD, YYYY').value
}

useEventListener('keydown', handleKeydown)
</script>

<template>
  <UModal
    v-model:open="isOpen"
    :ui="{ footer: 'justify-end', overlay: 'backdrop-blur-sm' }"
    class="z-90"
    :dismissible="false"
    @close="closeModal"
    @keydown.esc="closeModal"
  >
    <UButton
      size="lg"
      color="neutral"
      variant="subtle"
      icon="i-lucide-search"
      class="hover:bg-(--ui-bg) rounded-lg cursor-pointer"
      @click="openModal"
    >
      <template #trailing>
        <UKbd class="hidden lg:inline-flex opacity-60 group-hover:opacity-100 transition-opacity">
          <span class="text-xs">Ctrl</span> + <span class="text-xs">K</span>
        </UKbd>
      </template>
    </UButton>

    <template #header>
      <div class="flex flex-col space-y-4 w-full">
        <div class="flex justify-between items-center">
          <h3 class="font-semibold text-gray-900 dark:text-white text-lg">Search Books</h3>
          <div v-if="cacheSize && cacheSize > 0" class="text-gray-500 text-xs">
            {{ cacheSize }} cached searches
          </div>
        </div>

        <!-- Search Input -->
        <div class="relative">
          <UIcon
            name="i-lucide-search"
            size="20"
            class="top-1/2 left-3 z-10 absolute text-gray-400 -translate-y-1/2 transform"
          />
          <UInput
            ref="searchInput"
            v-model="searchQuery"
            placeholder="Search by title, author, category, or description..."
            size="xl"
            :ui="{
              base: 'w-full pl-10 pr-12',
            }"
            class="w-full"
          />
          <div class="top-1/2 right-3 z-10 absolute -translate-y-1/2 transform">
            <!-- Loading spinner -->
            <UIcon
              v-if="isSearching"
              name="i-lucide-loader"
              class="w-5 h-5 text-primary animate-spin"
            />
            <!-- Clear button -->
            <UButton
              v-else-if="hasQuery"
              @click="clearSearch"
              variant="ghost"
              size="xs"
              icon="i-lucide-x"
              class="rounded-full"
              :ui="{ base: 'h-6 w-6' }"
            />
          </div>
        </div>

        <!-- Search Stats -->
        <div v-if="hasResults && !isSearching" class="text-gray-600 dark:text-gray-400 text-sm">
          Found {{ searchResults.length }} result{{ searchResults.length !== 1 ? 's' : '' }}
          <span v-if="hasQuery">for "{{ searchQuery.trim() }}"</span>
        </div>
      </div>
    </template>

    <template #body>
      <div class="min-h-[200px] max-h-[400px]">
        <!-- Error State -->
        <div v-if="searchError" class="px-4 py-8 text-center">
          <UIcon name="i-lucide-alert-circle" class="mx-auto mb-3 w-12 h-12 text-red-500" />
          <p class="mb-2 font-medium text-red-600">{{ searchError }}</p>
          <UButton @click="clearSearch" variant="outline" size="sm" color="error">
            Clear Search
          </UButton>
        </div>

        <!-- Loading State -->
        <div v-else-if="isSearching" class="py-12 text-center">
          <UIcon name="i-lucide-loader" class="mx-auto mb-3 w-12 h-12 text-primary animate-spin" />
          <p class="text-gray-600 dark:text-gray-400">Searching books...</p>
        </div>

        <!-- Search Results -->
        <div v-else-if="hasResults" class="max-h-[400px] overflow-y-auto">
          <div class="divide-y divide-gray-100 dark:divide-gray-800">
            <div
              v-for="book in searchResults"
              :key="book.bookId || book.id"
              @click="goToBook(book.bookId)"
              class="group flex items-start gap-4 hover:bg-gray-50 dark:hover:bg-muted p-4 rounded-lg transition-colors cursor-pointer"
            >
              <!-- Book Cover -->
              <div class="flex-shrink-0">
                <img
                  :src="getImageUrl(book.imageUrl)"
                  :alt="book.bookName"
                  class="shadow-sm group-hover:shadow-md border rounded-md w-14 h-20 object-cover transition-shadow"
                  @error="($event.target as HTMLImageElement).src = '/placeholder-book.jpg'"
                />
              </div>

              <!-- Book Details -->
              <div class="flex-1 min-w-0">
                <h4
                  class="font-semibold text-gray-900 dark:text-white group-hover:text-primary line-clamp-2 transition-colors"
                >
                  {{ book.bookName }}
                </h4>
                <p class="mt-1 text-gray-600 dark:text-gray-400 text-sm">by {{ book.author }}</p>
                <p
                  v-if="book.description"
                  class="mt-2 text-gray-500 dark:text-gray-500 text-sm line-clamp-2"
                >
                  {{ book.description }}
                </p>
                <div class="flex justify-between items-center mt-3">
                  <div class="flex items-center gap-2">
                    <UBadge :label="book.category" size="xs" color="primary" variant="soft" />
                    <span class="text-gray-400 text-xs">
                      {{ formatDate(book.createdAt ?? '') }}
                    </span>
                  </div>
                  <span class="font-semibold text-green-600 text-sm">
                    GH₵{{ book.price.toFixed(2) }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-else-if="isEmpty" class="py-12 text-center">
          <UIcon name="i-lucide-search-x" class="mx-auto mb-3 w-12 h-12 text-gray-400" />
          <p class="font-medium text-gray-600 dark:text-gray-400">No books found</p>
          <p class="mt-1 text-gray-500 dark:text-gray-500 text-sm">
            Try different keywords or check your spelling
          </p>
        </div>

        <!-- Initial State -->
        <div v-else class="py-12 text-center">
          <UIcon name="i-lucide-search" class="mx-auto mb-3 w-12 h-12 text-gray-400" />
          <p class="font-medium text-gray-600 dark:text-gray-400">Start typing to search books</p>
          <p class="mt-1 text-gray-500 dark:text-gray-500 text-sm">
            Search across titles, authors, categories, and descriptions
          </p>
        </div>
      </div>
    </template>

    <template #footer>
      <div class="flex justify-between items-center">
        <div class="flex items-center gap-2 text-gray-500 dark:text-gray-500 text-xs">
          <UKbd>Esc</UKbd>
          <span>to close</span>
          <UKbd>⌘K</UKbd>
          <span>to search</span>
        </div>
        <UButton
          @click="closeModal"
          color="error"
          variant="soft"
          label="Close"
          size="lg"
          class="mx-2"
        />
      </div>
    </template>
  </UModal>
</template>

<style></style>

<style scoped></style>
