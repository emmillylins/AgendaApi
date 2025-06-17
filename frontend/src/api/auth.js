import axios from "axios";

const API_URL = "https://localhost:44380/api/auth";

// Instância do axios configurada
const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    Accept: "application/json",
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

// Interceptor: adiciona token JWT em todas as requisições
apiClient.interceptors.request.use(
  (config) => {
    const token =
      sessionStorage.getItem("jwt_token") || localStorage.getItem("jwt_token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor: tratamento global de erros
apiClient.interceptors.response.use(
  (response) => {
    if (response.data?.success === false) {
      const error = new Error(response.data.errors?.[0] || "Operação falhou");
      error.response = response;
      return Promise.reject(error);
    }
    return response.data;
  },
  (error) => {
    if (error.response?.status === 401) {
      sessionStorage.removeItem("jwt_token");
      localStorage.removeItem("jwt_token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }

    if (error.response) {
      const message =
        error.response.data.errors?.[0] ||
        error.response.data.message ||
        "Erro na requisição";
      const apiError = new Error(message);
      apiError.response = error.response;
      return Promise.reject(apiError);
    } else if (error.request) {
      return Promise.reject(
        new Error("Sem resposta do servidor - verifique sua conexão")
      );
    } else {
      return Promise.reject(
        new Error("Erro ao configurar requisição: " + error.message)
      );
    }
  }
);

// Serviço de autenticação
export const authService = {
  /**
   * Login com salvamento de token e dados do usuário
   */
  async login(credentials) {
    try {
      const response = await apiClient.post("/login", credentials);

      const token = response?.data?.token || response?.token; // cobre os dois formatos
      if (token) {
        const storage = credentials.rememberMe ? localStorage : sessionStorage;
        storage.setItem("jwt_token", token);

        const userData = {
          id: response?.data?.userId || response?.userId,
          username: response?.data?.username || response?.username,
        };

        localStorage.setItem("user", JSON.stringify(userData));
      } else {
        throw new Error("Token não recebido do servidor.");
      }

      return response;
    } catch (error) {
      console.error("Erro no login:", error);
      throw error;
    }
  },

  /**
   * Logout e limpeza de sessão
   */
  async logout() {
    const token =
      sessionStorage.getItem("jwt_token") || localStorage.getItem("jwt_token");
    if (!token) return;
    sessionStorage.removeItem("jwt_token");
    localStorage.removeItem("jwt_token");
    localStorage.removeItem("user");
  },

  /**
   * Registro de usuário
   */
  async register(userData) {
    try {
      const response = await apiClient.post("/cadastro", userData);

      // Se o backend retornar success: false
      if (response.data && response.data.success === false) {
        return response.data; // Deve conter errors ou message
      }

      return response.data;
    } catch (error) {
      // Se for um erro de axios, já vem com response.data
      throw error;
    }
  },

  /**
   * Lista de usuários autenticados
   */
  async getUsers() {
    return apiClient.get("/usuarios");
  },

  /**
   * Verifica se o usuário está autenticado
   */
  isAuthenticated() {
    return !!(
      sessionStorage.getItem("jwt_token") || localStorage.getItem("jwt_token")
    );
  },

  /**
   * Recupera o token JWT atual
   */
  getToken() {
    return (
      sessionStorage.getItem("jwt_token") || localStorage.getItem("jwt_token")
    );
  },

  /**
   * Recupera dados do usuário armazenado
   */
  getUserData() {
    const user = localStorage.getItem("user");
    return user ? JSON.parse(user) : null;
  },
};
