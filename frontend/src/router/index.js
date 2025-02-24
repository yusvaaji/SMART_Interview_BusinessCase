import { createRouter, createWebHashHistory } from 'vue-router'
import { useMainStore } from '@/stores/main.js'
import Home from '@/views/HomeView.vue'
import ManageProject from '@/views/ManageProject.vue'

const routes = [
  {
    meta: {
      title: 'Dashboard',
      requiresAuth: true
    },
    path: '/dashboard',
    name: 'dashboard',
    component: Home
  },
  {
    meta: {
      title: 'Manage Project',
      requiresAuth: true
    },
    path: '/manage-project/:id?',
    name: 'ManageProject',
    component: ManageProject
  },
  {
    meta: {
      title: 'Profile'
    },
    path: '/profile',
    name: 'profile',
    component: () => import('@/views/ProfileView.vue')
  },
  {
    meta: {
      title: 'Login'
    },
    path: '/',
    name: 'login',
    component: () => import('@/views/LoginView.vue')
  }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes,
  scrollBehavior(to, from, savedPosition) {
    return savedPosition || { top: 0 }
  }
})

router.beforeEach((to, from, next) => {
  const mainStore = useMainStore()
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!mainStore.token) {
      next({ name: 'login' })
    } else {
      next()
    }
  } else {
    next()
  }
})

export default router
