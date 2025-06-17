<template>
  <div class="login-view">
    <div class="login-container">
      <div class="login-header">
        <h1 class="login-title">Bem-vindo de volta</h1>
        <p class="login-subtitle">Faça login para acessar sua conta</p>
      </div>

      <LoginForm @login-success="handleLoginSuccess" />

      <div class="login-footer">
        <p class="register-link">
          Não tem uma conta?
          <router-link to="/register">Crie uma agora</router-link>
        </p>
      </div>
    </div>
  </div>
</template>

<script>
import LoginForm from "../components/LoginForm.vue";

export default {
  components: {
    LoginForm,
  },
  data() {
    return {
      errorMessage: "",
      isSessionExpired: false,
    };
  },
  created() {
    this.checkAuthenticationStatus();
    this.checkSessionExpiration();
  },
  methods: {
    checkAuthenticationStatus() {
      // Se já estiver autenticado, redireciona
      if (localStorage.getItem("jwt_token")) {
        this.redirectAfterLogin();
      }
    },

    checkSessionExpiration() {
      // Verifica se foi redirecionado por sessão expirada
      this.isSessionExpired = this.$route.query.sessionExpired === "true";
    },

    async handleLoginSuccess() {
      console.log("Login bem-sucedido. Redirecionando agora...");
      try {
        await this.redirectAfterLogin();
      } catch (error) {
        console.error("Redirecionamento falhou:", error);
        this.errorMessage =
          "Ocorreu um erro durante o redirecionamento. Por favor, tente novamente.";
      }
    },

    async redirectAfterLogin() {
      const redirectPath = this.$route.query.redirect || "/agendas";
      console.log("Redirecionando para:", redirectPath);
      await this.$router.push(redirectPath);
      console.log("Após redirecionamento");

      // Recarrega a página somente se for necessário
      if (this.$route.fullPath === redirectPath) {
        window.location.reload();
      }
    },
  },
};
</script>

<style scoped>
.login-view {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f5f7fa;
  padding: 20px;
}

.login-container {
  width: 100%;
  max-width: 400px;
  padding: 2rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.login-title {
  color: #2c3e50;
  margin-bottom: 1.5rem;
}

.register-link {
  margin-top: 1.5rem;
  color: #7f8c8d;
}

.register-link a {
  color: #4a6fa5;
  text-decoration: none;
  font-weight: 600;
}

.register-link a:hover {
  text-decoration: underline;
}

@media (max-width: 480px) {
  .login-container {
    padding: 1.5rem;
  }
}
</style>
