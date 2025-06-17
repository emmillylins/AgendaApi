<template>
  <div class="register-view">
    <div class="register-container">
      <h1 class="register-title">Criar Conta</h1>
      <RegisterForm @register-success="handleRegisterSuccess" />
        <p class="login-link">
        Já tem uma conta? <router-link to="/">Faça login</router-link>
        </p>
    </div>
  </div>
</template>

<script>
import RegisterForm from '../components/RegisterForm.vue'

export default {
  name: 'RegisterView',
  components: {
    RegisterForm
  },
  methods: {
    handleRegisterSuccess() {
      this.$router.push('/agendas')
    }
  }
}
</script>

<style scoped>
.register-view {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: #f5f7fa;
  padding: 20px;
}

.register-container {
  width: 100%;
  max-width: 600px;
  padding: 2rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.register-title {
  color: #2c3e50;
  margin-bottom: 1.5rem;
  text-align: center;
}

.login-link {
  margin-top: 1.5rem;
  text-align: center;
  color: #718096;
}

.login-link a {
  color: #4a6fa5;
  text-decoration: none;
  font-weight: 600;
}

.login-link a:hover {
  text-decoration: underline;
}

@media (max-width: 480px) {
  .register-container {
    padding: 1.5rem;
  }
}
</style>