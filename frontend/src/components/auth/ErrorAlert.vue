<script lang="ts" setup>
const { errorMessage = 'Success' } = defineProps<{
  errorMessage: string | null
}>()
</script>

<template>
  <div v-auto-animate class="flex justify-center items-center">
    <UAlert
      v-if="errorMessage && errorMessage.trim() !== ''"
      variant="subtle"
      color="error"
      :description="errorMessage"
      class="flex justify-center mb-2 rounded-3xl font-medium text-sm text-center"
    />
  </div>
</template>
