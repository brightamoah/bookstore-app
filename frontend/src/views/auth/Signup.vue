<template>
  <div class="flex flex-col justify-center px-4 sm:px-6 lg:px-8 py-4 sm:py-8 min-h-[80dvh]">
    <div class="sm:mx-auto mt-4 sm:mt-8 w-full sm:w-[90%] md:w-[80%] lg:w-[70%] sm:max-w-md">
      <div class="bg-muted shadow-lg px-4 sm:px-8 md:px-10 py-6 sm:py-8 rounded-lg sm:rounded-lg">
        <div class="sm:mx-auto mb-4 sm:mb-6 sm:w-full sm:max-w-md">
          <h2 class="font-bold text-xl sm:text-2xl text-center">
            Welcome Back To
            <span class="text-primary">BookHaven!</span>
          </h2>
          <p class="text-sm sm:text-base text-center capitalize">Please sign up to continue</p>
        </div>
        <ErrorAlert :errorMessage class="mt-3" />

        <UForm
          :state="registerFormData"
          class="space-y-4 sm:space-y-6"
          @submit.prevent="handleSignup"
        >
          <div class="gap-4 grid grid-cols-1 sm:grid-cols-2">
            <div>
              <UFormField
                label="First Name"
                name="firstName"
                size="xl"
                required
                class="block mt-1 font-medium text-base"
              >
                <UInput
                  id="firstName"
                  v-model="registerFormData.firstName"
                  name="firstName"
                  type="text"
                  color="neutral"
                  highlight
                  leading-icon="i-lucide-user"
                  autocomplete="given-name"
                  size="xl"
                  placeholder="First name"
                  required
                  class="block rounded-full w-full"
                />
              </UFormField>
            </div>

            <div>
              <UFormField
                label="Last Name"
                name="lastName"
                required
                size="xl"
                class="block mt-1 font-medium text-base"
              >
                <UInput
                  id="lastName"
                  v-model="registerFormData.lastName"
                  name="lastName"
                  type="text"
                  color="neutral"
                  highlight
                  leading-icon="i-lucide-user"
                  autocomplete="family-name"
                  size="xl"
                  placeholder="Last name"
                  required
                  class="block rounded-full w-full"
                />
              </UFormField>
            </div>
          </div>

          <div>
            <UFormField
              label="Email"
              name="email"
              required
              size="xl"
              class="block mt-1 font-medium text-base"
            >
              <UInput
                id="email"
                v-model="registerFormData.email"
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

          <div class="gap-4 grid grid-cols-1 sm:grid-cols-2">
            <div>
              <UFormField
                name="password"
                label="Password"
                required
                size="xl"
                class="block mt-1 font-medium text-base"
              >
                <UInput
                  id="password"
                  v-model="registerFormData.password"
                  name="password"
                  color="neutral"
                  highlight
                  size="xl"
                  placeholder="Enter Password"
                  leading-icon="i-lucide-lock"
                  :type="showPassword ? 'text' : 'password'"
                  autocomplete="new-password"
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

            <div>
              <UFormField
                name="confirmPassword"
                label="Confirm Password"
                required
                size="xl"
                class="block mt-1 font-medium text-base"
              >
                <UInput
                  id="confirmPassword"
                  v-model="registerFormData.confirmPassword"
                  name="confirmPassword"
                  color="neutral"
                  highlight
                  size="xl"
                  placeholder="Confirm your password"
                  leading-icon="i-lucide-shield-check"
                  :type="showConfirmPassword ? 'text' : 'password'"
                  autocomplete="new-password"
                  required
                  class="block rounded-full w-full"
                  :ui="{ trailing: 'pe-1 ' }"
                >
                  <template #trailing>
                    <UButton
                      color="neutral"
                      variant="link"
                      size="xl"
                      :icon="showConfirmPassword ? 'i-lucide-eye-off' : 'i-lucide-eye'"
                      :aria-label="showConfirmPassword ? 'Hide password' : 'Show password'"
                      :aria-pressed="showConfirmPassword"
                      aria-controls="confirmPassword"
                      @click="showConfirmPassword = !showConfirmPassword"
                    />
                  </template>
                </UInput>
              </UFormField>
              <p v-if="passwordMismatch" class="mt-1 text-red-600 text-xs">
                Passwords do not match
              </p>
            </div>
          </div>

          <div class="flex justify-center items-center mt-4 sm:mt-6">
            <UButton
              type="submit"
              color="primary"
              variant="solid"
              label="Create Account"
              :loading="isLoading"
              size="xl"
              trailing-icon="i-lucide-user-plus"
              :disabled="!isSignupFormValid"
              class="flex justify-center items-center disabled:opacity-50 shadow-sm px-4 py-2 sm:py-2.5 rounded-full focus:outline-none focus:ring-2 w-full sm:w-[85%] md:w-[70%] font-semibold text-white text-sm sm:text-base transition-all disabled:cursor-not-allowed"
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
          Already have an account?
          <RouterLink
            :to="{ name: 'login' }"
            class="ml-1 border-primary hover:border-primary-dark border-b font-semibold text-primary hover:text-primary-dark text-base"
          >
            Log In
          </RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/AuthStore'
import { storeToRefs } from 'pinia'

const authStore = useAuthStore()

const {
  registerFormData,
  isLoading,
  passwordMismatch,
  showPassword,
  showConfirmPassword,
  isSignupFormValid,
  errorMessage,
} = storeToRefs(authStore)

const { handleSignup } = authStore
</script>
