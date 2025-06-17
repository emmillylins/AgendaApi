import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import AgendaView from '../views/AgendaView.vue'
import RegisterView from '../views/RegisterView.vue'

const routes = [
  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: { requiresAuth: false, hideWhenAuth: true }
  },
  {
    path: '/',
    redirect: (to) => {
      return localStorage.getItem('jwt_token') ? '/agendas' : '/login'
    }
  },
  {
    path: '/agendas',
    name: 'agendas',
    component: AgendaView,
    meta: { requiresAuth: true }
  },
  {
    path: '/register',
    name: 'register',
    component: RegisterView,
    meta: { requiresAuth: false, hideWhenAuth: true }
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  },
  {
  path: '/teste',
  name: 'Teste',
  component: { template: '<div>Teste de rota funciona!</div>' },
  
    meta: { requiresAuth: true }
}

]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL), 
  routes
})

router.beforeEach((to, from, next) => {
  const token =
    localStorage.getItem('jwt_token') || sessionStorage.getItem('jwt_token')
  const isAuthenticated = !!token

  if (to.meta.requiresAuth && !isAuthenticated) {
    return next({
      path: '/login',
      query: { redirect: to.fullPath }
    })
  }

  if (to.meta.hideWhenAuth && isAuthenticated) {
    return next('/agendas')
  }

  next()
})

export default router