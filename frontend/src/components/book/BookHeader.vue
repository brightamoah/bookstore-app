<script setup lang="ts">
import type { Book } from '@/Types/types'
import { computed, ref } from 'vue'

defineProps<{
  isLoading: boolean
}>()

const emit = defineEmits<{
  submit: [book: Omit<Book, 'id'>]
  success: [message: string]
  error: [message: string]
  refreshBooks: []
}>()

const handleBookSubmitted = (book: Omit<Book, 'id'>) => {
  emit('submit', book)
}

const handleSuccess = (message: string) => {
  emit('success', message)
}

const handleError = (message: string) => {
  emit('error', message)
}

const isRefreshDisabled = ref(false)
const remainingTime = ref(0)
const refreshCoolDown = 120

const buttonLabel = computed(() => {
  return isRefreshDisabled.value ? `Wait ${remainingTime.value}s` : 'Refresh books'
})

let countdownInterval: ReturnType<typeof setInterval> | null = null

const handleRefreshClick = () => {
  if (isRefreshDisabled.value) return // Prevent multiple clicks

  isRefreshDisabled.value = true
  remainingTime.value = refreshCoolDown

  emit('refreshBooks')

  if (countdownInterval) {
    clearInterval(countdownInterval)
  }

  countdownInterval = setInterval(() => {
    remainingTime.value -= 1
    if (remainingTime.value <= 0) {
      clearInterval(countdownInterval!)
      countdownInterval = null
      isRefreshDisabled.value = false
    }
  }, 1000)
}
</script>

<template>
  <div class="shadow-sm border-b">
    <div class="mx-auto px-4 sm:px-6 lg:px-8 max-w-7xl">
      <div class="flex justify-between items-center py-6">
        <div>
          <h1 class="font-bold text-2xl">
            <UIcon name="tabler-books" class="size-10 text-primary" />
            <span>Book Store Dashboard</span>
          </h1>
          <p class="mt-1">Manage your book inventory</p>
        </div>

        <div class="flex items-center gap-4">
          <EditModal @error="handleError" @success="handleSuccess" @submit="handleBookSubmitted" />
          <UButton
            color="primary"
            variant="outline"
            size="xl"
            :label="buttonLabel"
            :disabled="isRefreshDisabled || isLoading"
            :loading="isLoading"
            loading-icon="i-lucide-loader"
            icon="i-heroicons-arrow-path"
            @click="handleRefreshClick"
            class="truncate cursor-pointer"
          />
        </div>
      </div>
    </div>
  </div>
</template>
