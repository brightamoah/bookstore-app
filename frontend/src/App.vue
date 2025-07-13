<script setup lang="ts">
import { onMounted, watch } from 'vue'
import { useAuthStore } from './stores/AuthStore'
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router'
import { useBookStore } from './stores/BookStore'

const router = useRouter()
const bookStore = useBookStore()
const authStore = useAuthStore()
const { isLoggedIn, user } = storeToRefs(authStore)

onMounted(async () => {
  if (!authStore.userChecked) await authStore.getCurrentUser()
})

watch([isLoggedIn, user], async ([newIsLoggedIn, newUser], [oldIsLoggedIn, oldUser]) => {
  if (!newIsLoggedIn && !newUser && (oldIsLoggedIn || oldUser)) {
    // User logged out, redirect to login
    console.log('User logged out, redirecting to login')
    router.push({
      name: 'login',
      query: {
        redirect: '/books',
      },
    })
  } else if ((newIsLoggedIn || newUser) && !(oldIsLoggedIn || oldUser)) {
    // User logged in, load books
    console.log('User logged in, loading books')
    await bookStore.refreshBooks()
  }
})
</script>

<template>
  <UApp>
    <div>
      <Navbar />
      <RouterView />
      <Footer />
    </div>
  </UApp>
</template>

<style scoped></style>
