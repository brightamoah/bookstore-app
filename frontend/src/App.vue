<script setup lang="ts">
import { defineAsyncComponent, onMounted, ref, watch } from 'vue'
import { useAuthStore } from './stores/AuthStore'
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router'
import { useBookStore } from './stores/BookStore'

const isLoaded = ref(false)

const router = useRouter()
const bookStore = useBookStore()
const authStore = useAuthStore()
const { isLoggedIn, user } = storeToRefs(authStore)

onMounted(async () => {
  if (!authStore.userChecked) await authStore.getCurrentUser()
  isLoaded.value = true
})

watch([isLoggedIn, user], async ([newIsLoggedIn, newUser], [oldIsLoggedIn, oldUser]) => {
  if (!newIsLoggedIn && !newUser && (oldIsLoggedIn || oldUser)) {
    console.log('User logged out, redirecting to login')
    router.push({
      name: 'login',
      query: {
        redirect: '/books',
      },
    })
  }
})

const FooterComponentLazy = defineAsyncComponent({
  loader: () => import('@/components/app/Footer.vue'),
})
</script>

<template>
  <UApp :toaster="{ position: 'top-right', duration: 8000 }">
    <div>
      <Navbar />
      <RouterView v-if="isLoaded" />
      <FooterComponentLazy v-if="isLoaded" />
    </div>
  </UApp>
</template>

<style scoped></style>
