import { mount, VueWrapper } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import { createRouter, createWebHistory } from 'vue-router'
import { vi } from 'vitest'
import type { Component } from 'vue'

// Create a mock router for testing
export const createMockRouter = (routes: any[] = []) => {
  return createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', name: 'home', component: { template: '<div>Home</div>' } },
      { path: '/books', name: 'books', component: { template: '<div>Books</div>' } },
      { path: '/auth/login', name: 'login', component: { template: '<div>Login</div>' } },
      { path: '/auth/signup', name: 'signup', component: { template: '<div>Signup</div>' } },
      {
        path: '/books/:id',
        name: 'book-details',
        component: { template: '<div>Book Details</div>' },
      },
      ...routes,
    ],
  })
}

// Create a test wrapper with all necessary providers
export const createTestWrapper = (
  component: Component,
  options: {
    props?: Record<string, any>
    global?: {
      plugins?: any[]
      mocks?: Record<string, any>
      stubs?: Record<string, any>
      provide?: Record<string, any>
    }
    router?: any
    attachTo?: Element
  } = {},
) => {
  const pinia = createPinia()
  setActivePinia(pinia)

  const router = options.router || createMockRouter()

  const wrapper = mount(component, {
    ...options,
    global: {
      plugins: [pinia, router, ...(options.global?.plugins || [])],
      mocks: {
        $router: router,
        $route: router.currentRoute.value,
        ...(options.global?.mocks || {}),
      },
      stubs: {
        RouterLink: true,
        RouterView: true,
        UButton: true,
        UInput: true,
        UTextarea: true,
        UFormField: true,
        UForm: true,
        UModal: true,
        UCard: true,
        UIcon: true,
        UAlert: true,
        USelectMenu: true,
        UInputNumber: true,
        UBadge: true,
        UAvatar: true,
        USeparator: true,
        UApp: true,
        Navbar: true,
        Footer: true,
        ...(options.global?.stubs || {}),
      },
      provide: {
        ...(options.global?.provide || {}),
      },
    },
  })

  return wrapper
}

// Mock useToast composable
export const mockUseToast = vi.fn(() => ({
  add: vi.fn(),
  remove: vi.fn(),
  clear: vi.fn(),
}))

// Mock useRouter composable
export const mockUseRouter = vi.fn(() => ({
  push: vi.fn(),
  replace: vi.fn(),
  go: vi.fn(),
  back: vi.fn(),
  forward: vi.fn(),
  currentRoute: { value: { path: '/', query: {}, params: {} } },
}))

// Mock useRoute composable
export const mockUseRoute = vi.fn(() => ({
  path: '/',
  query: {},
  params: {},
  name: 'home',
  meta: {},
}))

// Helper to wait for component updates
export const waitForUpdate = async (wrapper: VueWrapper) => {
  await wrapper.vm.$nextTick()
  await new Promise((resolve) => setTimeout(resolve, 0))
}

// Helper to simulate user events
export const simulateUserEvent = {
  click: async (element: any) => {
    await element.trigger('click')
    await waitForUpdate(element.vm?.$parent || element)
  },
  input: async (element: any, value: string) => {
    await element.setValue(value)
    await element.trigger('input')
    await waitForUpdate(element.vm?.$parent || element)
  },
  submit: async (form: any) => {
    await form.trigger('submit')
    await waitForUpdate(form.vm?.$parent || form)
  },
}

// Mock FileReader for file upload tests
export const mockFileReader = () => {
  const mockFileReader = {
    result: 'data:image/jpeg;base64,mockbase64data',
    readAsDataURL: vi.fn(function (this: any) {
      setTimeout(() => {
        this.onload({ target: { result: this.result } })
      }, 0)
    }),
    onload: vi.fn(),
  }

  global.FileReader = vi.fn(() => mockFileReader) as any
  return mockFileReader
}

// Reset all mocks
export const resetAllMocks = () => {
  vi.clearAllMocks()
  mockUseToast.mockClear()
  mockUseRouter.mockClear()
  mockUseRoute.mockClear()
}
