<script setup lang="ts">
import { useWindowSize } from '@vueuse/core'
import ToggleTheme from './ToggleTheme.vue'
import { computed } from 'vue'
import { useAuthStore } from '@/stores/AuthStore'
import { storeToRefs } from 'pinia'
import { useRoute, type _RouteRecordBase } from 'vue-router'

const route = useRoute()

const navMenuItems = [
  { name: 'Home', path: '/' },
  { name: 'Books', path: '/books' },
]

const authStore = useAuthStore()
const { isLoggedIn } = storeToRefs(authStore)

const { width } = useWindowSize()

const mobile = computed(() => width.value <= 768)

const authLabel = computed(() => {
  return isLoggedIn.value ? 'Logout' : 'Login'
})

const authPath = computed(() => {
  return isLoggedIn.value ? '' : '/auth/login'
})

const isActiveRoute = (path: _RouteRecordBase['path']) => {
  // Exact match for home page
  if (path === '/' && route.path === '/') {
    return true
  }

  if (path !== '/' && route.path.startsWith(path)) {
    return true
  }

  return false
}
</script>

<template>
  <header
    class="top-0 z-50 sticky flex justify-between items-center shadow-md backdrop-blur-xl p-4 rounded-md w-full max-w-screen"
  >
    <section>
      <RouterLink to="/">
        <NavLogo class="w-40" />
      </RouterLink>
    </section>

    <section v-if="!mobile">
      <RouterLink
        v-for="item in navMenuItems"
        :key="item.name"
        :to="item.path"
        class="m-3"
        :class="{ 'text-primary font-bold': isActiveRoute(item.path) }"
        active-class="text-primary font-bold"
        exact-active-class="text-primary font-bold"
      >
        {{ item.name }}
      </RouterLink>
    </section>

    <section>
      <div class="flex justify-between items-center gap-4">
        <!-- <UIcon name="i-lucide-shopping-cart" class="size-6 cursor-pointer" /> -->

        <UButton
          :label="authLabel"
          color="primary"
          variant="solid"
          size="lg"
          class="items-center rounded-xl text-center cursor-pointer"
          :to="authPath"
          @click="isLoggedIn ? authStore.logout() : null"
        />
        <ToggleTheme />
      </div>
    </section>
  </header>
</template>

<style scoped></style>
