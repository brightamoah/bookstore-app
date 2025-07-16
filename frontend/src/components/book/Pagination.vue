<script setup lang="ts">
import { computed, ref } from 'vue'

defineProps<{
  currentPageFirst: number
  currentPageLast: number
  totalBooks: number
  pageSize: number
  pageSizeOptions: Array<{ label: string; value: number }>
  isLoading: boolean
}>()

const emit = defineEmits<{
  'update:pageSize': [value: number]
}>()
</script>

<template>
  <div class="bg-muted shadow p-4 rounded-lg">
    <div class="flex flex-wrap justify-between items-center gap-4">
      <div>
        <p class="text-sm">
          Showing {{ currentPageFirst }} - {{ currentPageLast }} of {{ totalBooks }} books
        </p>
      </div>
      <div class="flex items-center gap-2">
        <label class="text-sm">Books per page:</label>
        <USelect
          :model-value="pageSize"
          :items="pageSizeOptions"
          size="sm"
          @update:model-value="emit('update:pageSize', $event)"
          class="cursor-pointer"
        />
      </div>
    </div>
  </div>
</template>
