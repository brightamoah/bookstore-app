<template>
  <section class="relative -mt-18 min-h-screen overflow-hidden">
    <!-- Background gradient blobs -->
    <div class="absolute inset-0 overflow-hidden">
      <div
        class="-top-[40%] -left-[40%] absolute bg-primary/10 blur-3xl rounded-full w-[80%] h-[80%]"
      ></div>
      <div
        class="right-[10%] -bottom-[30%] absolute bg-primary/5 dark:bg-primary/10 blur-3xl rounded-full w-[60%] h-[60%]"
      ></div>
    </div>

    <div
      class="z-10 relative flex items-center mx-auto px-4 sm:px-6 lg:px-8 py-8 lg:py-12 min-h-screen overflow-y-auto container"
    >
      <div class="flex lg:flex-row flex-col justify-between items-center gap-8 lg:gap-16 w-full">
        <!-- Text Content - Enhanced for visual appeal -->
        <div class="flex-1 lg:max-w-2xl lg:text-left text-center">
          <!-- Feature badge -->
          <div
            class="inline-flex items-center bg-primary/10 mb-4 lg:mb-6 px-4 py-2 rounded-full animate-fade-in"
          >
            <UIcon name="i-lucide-book-open" class="mr-2 w-4 h-4 text-primary" />
            <span class="font-medium text-primary text-sm">Your literary adventure awaits</span>
          </div>

          <!-- Enhanced heading with decorative elements -->
          <h1
            class="relative bg-clip-text bg-gradient-to-r from-primary via-primary-dark to-primary mb-4 lg:mb-6 pb-2 font-curve font-extrabold text-transparent text-3xl sm:text-4xl md:text-5xl lg:text-6xl leading-tight tracking-tight animate-fade-in"
          >
            Welcome to
            <span class="inline-block relative">
              <span class="font-raleway font-extrabold text-(--ui-text)">BookHaven</span>
              <svg
                class="-bottom-2 left-0 absolute opacity-70 w-full h-3 text-primary"
                viewBox="0 0 200 9"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M0,5 Q40,0 80,5 T160,5 T200,5"
                  stroke="currentColor"
                  stroke-width="4"
                  fill="none"
                />
              </svg>
            </span>
          </h1>

          <!-- Improved paragraph layout with visual enhancements -->
          <div class="lg:ml-10">
            <div class="relative mb-6 lg:mb-8 animate-fade-in-delayed-1">
              <div
                class="hidden lg:block top-0 -left-6 absolute bg-primary opacity-20 rounded w-1 h-full"
              ></div>
              <p
                class="max-w-xl lg:max-w-2xl font-curve text-gray-600 dark:text-gray-300 text-base sm:text-lg lg:text-xl leading-relaxed"
              >
                <span class="font-medium">Discover captivating stories</span>, expand your
                knowledge, and explore thousands of books across every genre imaginable.
              </p>
              <p
                class="mt-3 max-w-xl lg:max-w-2xl font-curve text-gray-600 dark:text-gray-300 text-base sm:text-lg lg:text-xl leading-relaxed"
              >
                From bestselling novels to academic texts, find your next
                <span class="font-cursive font-medium text-primary">great read</span> today.
              </p>
            </div>
          </div>

          <!-- Enhanced buttons with hover effects -->
          <div
            class="flex sm:flex-row flex-col justify-center lg:justify-start gap-3 sm:gap-4 mb-6 lg:mb-8 animate-fade-in-delayed-2"
          >
            <UButton
              to="/books"
              color="primary"
              variant="solid"
              size="xl"
              class="shadow-lg shadow-primary/20 hover:shadow-primary/30 hover:shadow-xl px-6 py-3 rounded-full w-full sm:w-auto font-medium transition-all hover:-translate-y-1"
              trailing-icon="i-lucide-arrow-right"
            >
              Browse All Books
            </UButton>

            <UButton
              to="/auth/login"
              color="white"
              variant="outline"
              size="xl"
              class="hover:bg-primary/5 dark:hover:bg-primary/20 px-6 py-3 border-2 border-primary dark:border-primary rounded-full w-full sm:w-auto font-medium text-primary dark:text-white transition-all hover:-translate-y-1"
              trailing-icon="i-lucide-user"
            >
              My Library
            </UButton>
          </div>

          <!-- Enhanced user showcase with rating display -->
          <div
            class="flex justify-center lg:justify-start items-center gap-4 animate-fade-in-delayed-3"
          >
            <div class="flex -space-x-3">
              <UAvatar
                src="https://github.com/benjamincanac.png"
                size="lg"
                alt="Reader avatar"
                class="ring-2 ring-white dark:ring-gray-800"
              />
              <UAvatar
                src="https://github.com/benjamincanac.png"
                size="lg"
                alt="Reader avatar"
                class="ring-2 ring-white dark:ring-gray-800"
              />
              <UAvatar
                src="https://github.com/benjamincanac.png"
                size="lg"
                alt="Reader avatar"
                class="ring-2 ring-white dark:ring-gray-800"
              />
            </div>
            <div class="flex flex-col">
              <span class="font-medium text-gray-800 dark:text-gray-200 text-sm"
                >Join 10,000+ book lovers</span
              >
              <div class="flex items-center text-yellow-500 text-xs">
                <!-- <HeroRating :rating="4.8" :star-size="4" /> -->
                <span class="ml-1 text-gray-600 dark:text-gray-400">4.8/5 customer rating</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Illustration with proper animation sequence -->
        <div
          class="relative lg:flex-grow-0 lg:flex-shrink-0 flex-1 w-full max-w-lg lg:max-w-2xl lg:basis-[45%]"
        >
          <div class="relative mx-auto illustration-container">
            <!-- Main illustration image -->
            <img
              src="https://res.cloudinary.com/eves-oasis/image/upload/bookstore_fhwcao.jpg"
              alt="Bookstore Illustration"
              class="shadow-2xl rounded-lg w-full h-auto max-h-[500px] lg:max-h-[600px] object-cover illustration-image"
            />

            <!-- Decorative floating elements -->
            <div
              class="top-1/4 -left-4 lg:-left-6 absolute bg-primary/30 blur-xl rounded-full w-8 lg:w-12 h-8 lg:h-12 animate-bounce-delayed delay-300"
            ></div>
            <div
              class="-right-2 lg:-right-4 bottom-1/3 absolute bg-primary/20 blur-lg rounded-full w-6 lg:w-8 h-6 lg:h-8 animate-bounce-slow delay-500"
            ></div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup></script>

<!-- <style>
/* Animation keyframes */
@keyframes float {
  0% {
    transform: translateY(0px);
  }
  50% {
    transform: translateY(-20px);
  }
  100% {
    transform: translateY(0px);
  }
}

@keyframes spin-slow {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

@keyframes fade-in {
  0% {
    opacity: 0;
    transform: translateY(20px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes bounce-slow {
  0%,
  100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-20px);
  }
}

@keyframes slide-in-right {
  0% {
    opacity: 0;
    transform: translateX(100px);
  }
  100% {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes slide-in-left {
  0% {
    opacity: 0;
    transform: translateX(-100px);
  }
  100% {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Animation classes */
.animate-float {
  animation: float 6s ease-in-out infinite;
}

.animate-spin-slow {
  animation: spin-slow 10s linear infinite;
}

.animate-spin-delayed {
  animation: spin-slow 8s linear infinite reverse;
}

.animate-bounce-slow {
  animation: bounce-slow 5s ease-in-out infinite;
}

.animate-bounce-delayed {
  animation: bounce-slow 7s ease-in-out infinite 1s;
}

.animate-fade-in {
  animation: fade-in 0.8s ease-out forwards;
}

.animate-fade-in-delayed-1 {
  opacity: 0;
  animation: fade-in 0.8s ease-out forwards 0.2s;
}

.animate-fade-in-delayed-2 {
  opacity: 0;
  animation: fade-in 0.8s ease-out forwards 0.4s;
}

.animate-fade-in-delayed-3 {
  opacity: 0;
  animation: fade-in 0.8s ease-out forwards 0.6s;
}

.animate-fade-in-delayed-4 {
  opacity: 0;
  animation: fade-in 0.8s ease-out forwards 0.8s;
}

.animate-slide-in-right {
  animation: slide-in-right 1s cubic-bezier(0.25, 0.46, 0.45, 0.94) forwards;
}

.animate-slide-in-left {
  animation: slide-in-left 1s cubic-bezier(0.25, 0.46, 0.45, 0.94) forwards;
}

.delay-300 {
  animation-delay: 300ms;
}

.delay-500 {
  animation-delay: 500ms;
}

/* New animation sequence for illustration */
.illustration-container {
  animation: slide-in-right 1s cubic-bezier(0.25, 0.46, 0.45, 0.94) forwards;
}

.illustration-image {
  animation: float 6s ease-in-out infinite;
  animation-delay: 1s; /* Start floating after slide-in completes */
}

/* Add animation-fill-mode to properly handle animation states */
.illustration-container,
.illustration-image,
.animate-fade-in,
.animate-fade-in-delayed-1,
.animate-fade-in-delayed-2,
.animate-fade-in-delayed-3,
.animate-fade-in-delayed-4 {
  animation-fill-mode: both;
}
</style> -->
