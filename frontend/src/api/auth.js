import apiClient from "./authClient";

export const authService = {
  async login(credentials) {
    const response = await apiClient.post("/auth/login", credentials);
    const token = response.data.data?.token;
    if (!token) throw new Error("Token não recebido do servidor.");

    const storage = credentials.rememberMe ? localStorage : sessionStorage;
    storage.setItem("jwt_token", token);

    const userData = {
      id: response.data.data.userId,
      username: response.data.data.username,
    };
    localStorage.setItem("user", JSON.stringify(userData));

    return response.data;
  },

  async register(userData) {
    const response = await apiClient.post("/auth/cadastro", userData);
    return response.data.data;
  },

  logout() {
    sessionStorage.removeItem("jwt_token");
    localStorage.removeItem("jwt_token");
    localStorage.removeItem("user");
  },

  isAuthenticated() {
    return !!(
      sessionStorage.getItem("jwt_token") || localStorage.getItem("jwt_token")
    );
  },

  getToken() {
    return (
      sessionStorage.getItem("jwt_token") || localStorage.getItem("jwt_token")
    );
  },

  getUserData() {
    const user = localStorage.getItem("user");
    return user ? JSON.parse(user) : null;
  },
};
