<script setup lang="ts">
import { VisuallyHidden } from 'reka-ui'
import { computed, ref, watch } from 'vue'
import type { Book } from '@/Types/types'
import { getImageUrl } from '@/services/Api'

const props = defineProps<{
  actionType: 'delete' | 'clear' | 'remove'
  book?: Book | null
  open?: boolean
  loading?: boolean
}>()

const emit = defineEmits<{
  confirm: []
  cancel: []
  'update:open': [value: boolean]
}>()

const isModalOpen = ref<boolean>(props.open || false)

watch(
  () => props.open,
  (newValue) => {
    isModalOpen.value = newValue || false
  },
)

watch(isModalOpen, (newValue) => {
  emit('update:open', newValue)
})

const title = computed(() => {
  switch (props.actionType) {
    case 'delete':
      return 'Delete Book'
    case 'remove':
      return 'Remove Book'
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
      return `Are you sure you want to remove <strong>"${props.book?.bookName}"</strong> from your cart?`
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
  isModalOpen.value = false
}

const handleCancel = () => {
  emit('cancel')
  isModalOpen.value = false
}
</script>

<template>
  <UModal
    v-model:open="isModalOpen"
    :dismissible="false"
    :ui="{
      footer: 'justify-end',
      body: 'ring',
      overlay: 'bg-black/50 backdrop-blur-sm',
      content: 'rounded-lg',
    }"
    aria-labelledby="confirmation-modal-title"
    aria-describedby="confirmation-modal-description"
  >
    <template #title>
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
        <div class="text-sm" v-html="message"></div>
      </div>
    </template>

    <template #footer>
      <div class="flex justify-end gap-3">
        <UButton
          label="Cancel"
          color="neutral"
          variant="outline"
          @click="handleCancel"
          class="mr-2 cursor-pointer"
        />
        <UButton
          :label="confirmButtonText"
          loading-icon="i-lucide-loader"
          :loading="loading"
          color="error"
          variant="solid"
          :icon="actionType === 'delete' ? 'i-heroicons-trash' : 'i-heroicons-x-mark'"
          @click="handleConfirm"
          class="cursor-pointer"
        />
      </div>
    </template>
  </UModal>
</template>
