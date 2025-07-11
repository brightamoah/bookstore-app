<script setup lang="ts">
import { computed } from 'vue'
import type { Book } from '@/Types/types'

const props = defineProps<{
  modelValue: boolean
  actionType: 'delete' | 'clear' | 'remove'
  book?: Book
  item?: { name: string; size: string }
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  confirm: []
}>()

const title = computed(() => {
  switch (props.actionType) {
    case 'delete':
      return 'Delete Book'
    case 'remove':
      return 'Remove Item'
    case 'clear':
      return 'Clear Cart'
    default:
      return 'Confirm Action'
  }
})

const message = computed(() => {
  switch (props.actionType) {
    case 'delete':
      return `Are you sure you want to delete <strong>"${props.book?.bookName}"</strong> by ${props.book?.author}? This action cannot be undone.`
    case 'remove':
      return `Are you sure you want to remove <strong>${props.item?.name} (${props.item?.size})</strong> from your cart?`
    case 'clear':
      return 'Are you sure you want to clear all items from your cart?'
    default:
      return 'Are you sure you want to proceed?'
  }
})

const confirmButtonText = computed(() => {
  switch (props.actionType) {
    case 'delete':
      return 'Delete Book'
    case 'remove':
      return 'Remove'
    case 'clear':
      return 'Clear'
    default:
      return 'Confirm'
  }
})

const handleConfirm = () => {
  emit('confirm')
  emit('update:modelValue', false)
}

const handleCancel = () => {
  emit('update:modelValue', false)
}
</script>

<template>
  <UModal :model-value="modelValue" @update:model-value="$emit('update:modelValue', $event)">
    <template #header>
      <div class="flex items-center gap-3">
        <div
          class="flex justify-center items-center bg-red-100 dark:bg-red-900/20 rounded-full w-10 h-10"
        >
          <UIcon
            name="i-heroicons-exclamation-triangle"
            class="w-6 h-6 text-red-600 dark:text-red-400"
          />
        </div>
        <h3 class="font-semibold text-gray-900 dark:text-white text-lg">
          {{ title }}
        </h3>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <!-- Book Preview (for delete action) -->
        <div
          v-if="actionType === 'delete' && book"
          class="flex items-start gap-3 bg-gray-50 dark:bg-gray-800 p-3 rounded-lg"
        >
          <img
            :src="book.imageUrl"
            :alt="book.bookName"
            class="border rounded w-12 h-16 object-cover"
            @error="($event.target as HTMLImageElement).src = '/placeholder-book.jpg'"
          />
          <div class="flex-1 min-w-0">
            <h4 class="font-medium text-gray-900 dark:text-white truncate">
              {{ book.bookName }}
            </h4>
            <p class="text-gray-600 dark:text-gray-400 text-sm">by {{ book.author }}</p>
            <div class="flex items-center gap-2 mt-1">
              <UBadge :label="book.category" size="xs" color="primary" variant="soft" />
              <span class="font-semibold text-green-600 text-sm"> ${{ book.price }} </span>
            </div>
          </div>
        </div>

        <!-- Warning Message -->
        <div class="text-gray-600 dark:text-gray-300 text-sm" v-html="message"></div>
      </div>
    </template>

    <template #footer>
      <div class="flex justify-end gap-3">
        <UButton label="Cancel" color="error" variant="outline" @click="handleCancel" />
        <UButton
          :label="confirmButtonText"
          color="error"
          variant="solid"
          @click="handleConfirm"
          :icon="actionType === 'delete' ? 'i-heroicons-trash' : 'i-heroicons-x-mark'"
        />
      </div>
    </template>
  </UModal>
</template>
