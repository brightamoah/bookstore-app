import type { ToastProps } from '@nuxt/ui'

export interface ToastOptions {
  title?: ToastProps['title']
  description?: ToastProps['description']
  color?: ToastProps['color']
  icon?: string
  duration?: number
  close?: boolean
}

export const showToast = (options: ToastOptions) => {
  const toast = useToast()
  const toastOptions: ToastProps = {
    title: options.title || 'Notification',
    description: options.description || '',
    color: options.color || 'info',
    icon: options.icon || 'i-heroicons-check-badge-16-solid',
    close: options.close !== undefined ? options.close : true,
    orientation: 'horizontal',
  }

  return toast.add(toastOptions)
}
