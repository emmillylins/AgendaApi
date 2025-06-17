import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:44380/api', // ajuste se necessário
  timeout: 10000, // 10s de timeout para evitar travamento
})

// Intercepta requisições e injeta token
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Intercepta respostas com erro (opcional, mas útil para debug)
axios.interceptors.response.use(response => response, error => {
  if (error.response.status === 401) {
    localStorage.removeItem('jwt_token')
    router.push('/login')
  }
  return Promise.reject(error)
})