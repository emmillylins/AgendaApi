import axios from "axios";

const apiClient = axios.create({
  baseURL: "https://localhost:44380/api",
  headers: {
    Accept: "application/json",
    "Content-Type": "application/json",
  },
//   withCredentials: true,
});

// Adiciona o token JWT em todas as requisições
apiClient.interceptors.request.use(
  (config) => {
    const token = sessionStorage.getItem("jwt_token") || localStorage.getItem("jwt_token");
    
  console.log("Token sendo enviado:", token); // Adicione este log
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Tratamento global de erros
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      sessionStorage.removeItem("jwt_token");
      localStorage.removeItem("jwt_token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }

    const message =
      error.response?.data?.errors?.[0] ||
      error.response?.data?.message ||
      "Erro desconhecido";

    const apiError = new Error(message);
    apiError.response = error.response;
    return Promise.reject(apiError);
  }
);

export default apiClient;
