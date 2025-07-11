<script setup lang="ts">
defineProps<{
  currentPage: number
  pageCount: number
  totalBooks: number
  canPrev: boolean
  canNext: boolean
  visiblePages: (number | string)[]
}>()

const emit = defineEmits<{
  first: []
  prev: []
  next: []
  last: []
  goToPage: [page: number]
}>()

const handlePageClick = (page: number | string) => {
  if (typeof page === 'number') {
    emit('goToPage', page)
  }
}
</script>

<template>
  <div class="mt-8">
    <div class="flex sm:flex-row flex-col justify-between items-center gap-4">
      <div class="text-sm">
        Page {{ currentPage }} of {{ pageCount }} ({{ totalBooks }} total books)
      </div>

      <div class="flex items-center gap-2">
        <!-- First Page -->
        <UButton
          icon="i-lucide-chevrons-left"
          size="sm"
          color="neutral"
          variant="outline"
          :disabled="!canPrev"
          @click="$emit('first')"
          aria-label="First page"
          class="cursor-pointer"
        />

        <UButton
          icon="i-lucide-chevron-left"
          size="sm"
          color="neutral"
          variant="outline"
          :disabled="!canPrev"
          @click="$emit('prev')"
          aria-label="Previous page"
          class="cursor-pointer"
        />

        <div class="flex items-center gap-1">
          <UButton
            v-for="page in visiblePages"
            :key="page"
            size="sm"
            :color="page === currentPage ? 'primary' : 'neutral'"
            :variant="page === currentPage ? 'solid' : 'outline'"
            @click="handlePageClick(page)"
            class="cursor-pointer"
          >
            {{ page }}
          </UButton>
        </div>

        <UButton
          icon="i-lucide-chevron-right"
          size="sm"
          color="neutral"
          variant="outline"
          :disabled="!canNext"
          @click="$emit('next')"
          aria-label="Next page"
          class="cursor-pointer"
        />

        <!-- Last Page -->
        <UButton
          icon="i-lucide-chevrons-right"
          size="sm"
          color="neutral"
          variant="outline"
          :disabled="!canNext"
          @click="$emit('last')"
          aria-label="Last page"
          class="cursor-pointer"
        />
      </div>
    </div>
  </div>
</template>
