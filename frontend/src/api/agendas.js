import axios from 'axios'

const API_URL = 'https://localhost:44380/api/agendas'

const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  },
  withCredentials: true,
  maxRedirects: 0
})

// Interceptor para adicionar token JWT às requisições
apiClient.interceptors.request.use(config => {
  const token = sessionStorage.getItem('jwt_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
    // console.log(token);
  }
  return config
}, error => {
  return Promise.reject(error)
})

// Interceptor para tratamento de respostas
apiClient.interceptors.response.use(
  response => response,
  error => {
    // Tratamento de erros 401 (Não autorizado)
    // if (error.response?.status === 401) {
    //   sessionStorage.removeItem('jwt_token')
      
    //   // Cria um elemento para exibir a mensagem
    //   const errorMessage = document.createElement('div')
    //   errorMessage.style.position = 'fixed'
    //   errorMessage.style.top = '20px'
    //   errorMessage.style.left = '50%'
    //   errorMessage.style.transform = 'translateX(-50%)'
    //   errorMessage.style.backgroundColor = '#ffebee'
    //   errorMessage.style.color = '#c62828'
    //   errorMessage.style.padding = '15px 20px'
    //   errorMessage.style.borderRadius = '4px'
    //   errorMessage.style.boxShadow = '0 2px 10px rgba(0,0,0,0.1)'
    //   errorMessage.style.zIndex = '10000'
    //   errorMessage.style.display = 'flex'
    //   errorMessage.style.alignItems = 'center'
    //   errorMessage.style.gap = '10px'
    //   errorMessage.innerHTML = `
    //     <i class="fas fa-exclamation-circle"></i>
    //     <span>Sua sessão expirou. Você será redirecionado para o login em 3 segundos...</span>
    //   `
      
    //   // Adiciona contador regressivo
    //   let seconds = 3
    //   const counter = document.createElement('span')
    //   counter.style.marginLeft = '5px'
    //   counter.textContent = `(${seconds})`
    //   errorMessage.appendChild(counter)
      
    //   document.body.appendChild(errorMessage)
      
    //   // Atualiza o contador a cada segundo
    //   const interval = setInterval(() => {
    //     seconds--
    //     counter.textContent = `(${seconds})`
        
    //     if (seconds <= 0) {
    //       clearInterval(interval)
    //       window.location.href = '/login'
    //     }
    //   }, 1000)
      
    //   // Remove a mensagem após o redirecionamento
    //   setTimeout(() => {
    //     if (errorMessage.parentNode) {
    //       document.body.removeChild(errorMessage)
    //     }
    //   }, 3000)
      
    //   return Promise.reject(new Error('Sessão expirada'))
    // }

    // Tratamento de outros erros
    if (error.response) {
      const backendError = error.response.data?.errors?.[0] || 
                         error.response.data?.message || 
                         error.response.data?.title ||
                         'Erro no servidor'
      
      const customError = new Error(backendError)
      customError.details = error.response.data
      customError.status = error.response.status
      
      // Tratamento especial para erros de validação
      if (error.response.status === 400 && error.response.data.errors) {
        customError.validationErrors = error.response.data.errors
      }
      
      return Promise.reject(customError)
    }
    
    return Promise.reject(new Error('Erro de conexão com o servidor'))
  }
)

// Funções básicas da API
export const getAgendasApi = async () => {
  const response = await apiClient.get('/')
  return response.data.data || response.data
}

export const createAgendaApi = async (agenda) => {
  const response = await apiClient.post('/', agenda)
  return response.data
}

export const updateAgendaApi = async (agenda) => {
  const response = await apiClient.put('/', agenda)
  return response.data
}

export const deleteAgendaApi = async (id) => {
  const response = await apiClient.delete(`/${id}`)
  return response.data
}