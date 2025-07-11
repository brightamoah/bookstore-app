<template>
  <div class="flex flex-col justify-center px-4 sm:px-6 lg:px-8 py-4 sm:py-8 min-h-[60dvh]">
    <div class="sm:mx-auto mt-4 sm:mt-8 w-full sm:w-[90%] md:w-[80%] lg:w-[70%] sm:max-w-md">
      <div class="bg-muted shadow-lg px-4 sm:px-8 md:px-10 py-6 sm:py-8 rounded-lg sm:rounded-lg">
        <div class="sm:mx-auto mb-4 sm:mb-6 sm:w-full sm:max-w-md">
          <h2 class="font-bold text-xl sm:text-2xl text-center">
            Welcome Back To
            <span class="text-primary">BookHaven!</span>
          </h2>
          <p class="text-sm sm:text-base text-center capitalize">
            Please login to access your books
          </p>
        </div>

        <ErrorAlert :errorMessage />

        <UForm :state="loginFormData" class="space-y-4 sm:space-y-6" @submit.prevent="handleLogin">
          <div>
            <UFormField
              required
              label="Email"
              name="email"
              size="xl"
              class="block mt-1 font-medium text-base"
            >
              <UInput
                id="email"
                v-model="loginFormData.email"
                name="email"
                type="email"
                color="neutral"
                highlight
                leading-icon="i-lucide-mail"
                autocomplete="email"
                size="xl"
                placeholder="Enter your email"
                required
                class="block rounded-full w-full"
              />
            </UFormField>
          </div>

          <div>
            <UFormField
              required
              name="password"
              label="Password"
              size="xl"
              class="block mt-1 font-medium text-base"
            >
              <UInput
                id="password"
                v-model="loginFormData.password"
                name="password"
                color="neutral"
                highlight
                size="xl"
                placeholder="Enter your password"
                leading-icon="i-lucide-lock"
                :type="showPassword ? 'text' : 'password'"
                autocomplete="current-password"
                required
                class="block rounded-full w-full"
                :ui="{ trailing: 'pe-1 ' }"
              >
                <template #trailing>
                  <UButton
                    color="neutral"
                    variant="link"
                    size="xl"
                    :icon="showPassword ? 'i-lucide-eye-off' : 'i-lucide-eye'"
                    :aria-label="showPassword ? 'Hide password' : 'Show password'"
                    :aria-pressed="showPassword"
                    aria-controls="password"
                    @click="showPassword = !showPassword"
                  />
                </template>
              </UInput>
            </UFormField>
          </div>

          <div class="flex justify-between items-center mt-6 mb-5">
            <div class="flex items-center shrink-0">
              <UCheckbox
                v-model="loginFormData.rememberMe"
                size="lg"
                sm:size="md"
                color="primary"
                label="Remember me"
              />
            </div>

            <div class="text-sm sm:text-sm">
              <a
                href="#"
                class="font-semibold text-primary hover:text-primary-dark whitespace-nowrap"
              >
                Forgot password?
              </a>
            </div>
          </div>

          <div class="flex justify-center items-center mt-4 sm:mt-6">
            <UButton
              type="submit"
              color="primary"
              variant="solid"
              label="Log In"
              size="xl"
              trailing-icon="i-lucide-log-in"
              loading-icon="i-lucide-loader"
              :loading="isLoading"
              :disabled="!isLoginFormValid"
              class="flex justify-center items-center shadow-sm px-4 py-2 sm:py-2.5 rounded-full focus:outline-none focus:ring-2 w-full sm:w-[85%] md:w-[70%] font-semibold text-white text-sm sm:text-base transition-all cursor-pointer"
            />
          </div>
        </UForm>

        <div class="mt-4">
          <div class="relative">
            <USeparator
              label="OR"
              size="xs"
              :ui="{
                border: 'border-(--ui-border-inverted)',
              }"
            />
          </div>
        </div>

        <p class="mt-4 sm:mt-6 text-sm sm:text-sm text-center">
          Don't have an account?
          <RouterLink
            :to="{ name: 'signup' }"
            class="ml-1 border-primary hover:border-primary-dark border-b font-semibold text-primary hover:text-primary-dark text-base"
          >
            Sign Up
          </RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/AuthStore'
import { storeToRefs } from 'pinia'
import { onMounted, onUnmounted } from 'vue'

const authStore = useAuthStore()
const { loginFormData, isLoading, showPassword, isLoginFormValid, errorMessage } =
  storeToRefs(authStore)

const { handleLogin, resetFormData } = authStore

// onMounted(() => {
//   console.log(errorMessage.value)
// })

onUnmounted(() => {
  resetFormData()
})
</script>
