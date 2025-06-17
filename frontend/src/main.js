import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import axios from 'axios'
import '@fortawesome/fontawesome-free/css/all.css'

// Configuração global do axios
axios.defaults.baseURL = 'https://localhost:44380'

// Interceptor de requisição
axios.interceptors.request.use(config => {
  const token = localStorage.getItem('jwt_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}, error => {
  return Promise.reject(error)
})

// Interceptor de resposta (evita loops)
axios.interceptors.response.use(response => response, error => {
  if (error.response?.status === 401) {
    // Verifica se não está já na página de login
    if (router.currentRoute.value.path !== '/login') {
      localStorage.removeItem('jwt_token')
      router.push({
        path: '/login',
        query: { 
          redirect: router.currentRoute.value.fullPath,
          sessionExpired: true 
        }
      })
    }
  }
  return Promise.reject(error)
})

const app = createApp(App)
app.use(router)
app.mount('#app')