<script setup lang="ts"></script>

<template>
  <UCard
    variant="soft"
    class="bg-muted rounded-lg w-full max-w-6xl"
    :ui="{ body: 'p-6 ring-0', header: 'ring' }"
  >
    <template #header>
      <div class="flex justify-between items-center">
        <h3 class="font-semibold text-xl animate-pulse">Loading Book Details...</h3>

        <UIcon name="i-lucide-loader" class="size-12 text-primary animate-spin" />
      </div>
    </template>

    <div class="space-y-6">
      <div class="flex sm:flex-row flex-col gap-6">
        <!-- Image Skeleton -->
        <div class="flex-shrink-0 w-full sm:w-1/2 md:w-5/12 lg:w-1/3">
          <USkeleton class="bg-accented rounded-lg w-full h-96" />
        </div>

        <!-- Content Skeleton -->
        <div class="flex flex-col flex-1 justify-between space-y-6">
          <!-- Title and Author Skeleton -->
          <div>
            <USkeleton class="bg-accented mb-3 w-3/4 h-8" />
            <USkeleton class="bg-accented w-1/2 h-6" />
          </div>

          <!-- Details Grid Skeleton -->
          <div class="gap-6 grid grid-cols-1 sm:grid-cols-2">
            <div class="space-y-2">
              <USkeleton class="bg-accented w-20 h-4" />
              <USkeleton class="bg-accented w-24 h-5" />
            </div>
            <div class="space-y-2">
              <USkeleton class="bg-accented w-16 h-4" />
              <USkeleton class="bg-accented w-20 h-5" />
            </div>
            <div class="space-y-2">
              <USkeleton class="bg-accented w-12 h-4" />
              <USkeleton class="bg-accented w-16 h-5" />
            </div>
            <div class="space-y-2">
              <USkeleton class="bg-accented w-18 h-4" />
              <USkeleton class="bg-accented w-24 h-5" />
            </div>
          </div>
        </div>
      </div>

      <!-- Description Skeleton -->
      <div class="space-y-2">
        <USkeleton class="bg-accented w-24 h-4" />
        <USkeleton class="bg-accented w-full h-4" />
        <USkeleton class="bg-accented w-5/6 h-4" />
        <USkeleton class="bg-accented w-4/5 h-4" />
      </div>
    </div>
  </UCard>
</template>

<style scoped></style>
