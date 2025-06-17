<template>
  <form @submit.prevent="handleSubmit" class="login-form">
    <div class="form-group">
      <label for="email">Email:</label>
      <input
        id="email"
        v-model.trim="credentials.email"
        type="email"
        required
        class="form-input"
        :class="{ 'input-error': errors.email }"
        @input="clearError('email')"
      />
      <span v-if="errors.email" class="error-text">{{ errors.email }}</span>
    </div>

    <div class="form-group">
      <label for="password">Senha:</label>
      <input
        id="password"
        v-model.trim="credentials.password"
        type="password"
        required
        minlength="6"
        class="form-input"
        :class="{ 'input-error': errors.password }"
        @input="clearError('password')"
      />
      <span v-if="errors.password" class="error-text">{{
        errors.password
      }}</span>
    </div>

    <button
      type="submit"
      class="submit-btn"
      :disabled="isSubmitting"
      :class="{ loading: isSubmitting }"
    >
      <span v-if="isSubmitting">
        <i class="fas fa-spinner fa-spin"></i> Entrando...
      </span>
      <span v-else>Entrar</span>
    </button>

    <div v-if="authError" class="auth-error-message">
      <i class="fas fa-exclamation-circle"></i> {{ authError }}
    </div>
  </form>
</template>

<script>
import { authService } from "../api/auth";
import { validateEmail } from "../utils/validators";

export default {
  name: "LoginForm",
  data() {
    return {
      credentials: {
        email: "",
        password: "",
      },
      errors: {
        email: "",
        password: "",
      },
      authError: "",
      isSubmitting: false,
    };
  },
  methods: {
    validateForm() {
      let isValid = true;

      // Validação de email
      if (!this.credentials.email) {
        this.errors.email = "Email é obrigatório";
        isValid = false;
      } else if (!validateEmail(this.credentials.email)) {
        this.errors.email = "Email inválido";
        isValid = false;
      }

      // Validação de senha
      if (!this.credentials.password) {
        this.errors.password = "Senha é obrigatória";
        isValid = false;
      } else if (this.credentials.password.length < 6) {
        this.errors.password = "Senha deve ter pelo menos 6 caracteres";
        isValid = false;
      }

      return isValid;
    },

    clearError(field) {
      this.errors[field] = "";
      this.authError = "";
    },

    async handleSubmit() {
      if (!this.validateForm()) return;

      this.isSubmitting = true;
      this.authError = "";

      try {
        const response = await authService.login(this.credentials);

        if (response.success) {
          this.$emit("login-success");
          console.log("Evento de login emitido!");
        } else {
          this.authError = response.message || "Falha no login";
        }
      } catch (error) {
        console.error("Erro no login:", error);
        this.authError = error.message || "Erro inesperado ao fazer login.";

        // Tratamento especial para erros 401 (não autorizado)
        if (error.response?.status === 401) {
          this.authError = "Credenciais inválidas. Por favor, tente novamente.";
        }
      } finally {
        this.isSubmitting = false;
      }
    },
  },
};
</script>

<style scoped>
.login-form {
  width: 100%;
  max-width: 400px;
  margin: 0 auto;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #4a5568;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  font-size: 1rem;
}

.form-input:focus {
  outline: none;
  border-color: #4a6fa5;
  box-shadow: 0 0 0 3px rgba(74, 111, 165, 0.2);
}

.submit-btn {
  width: 100%;
  padding: 0.75rem;
  background-color: #4a6fa5;
  color: white;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: background-color 0.3s;
}

.submit-btn:hover:not(:disabled) {
  background-color: #3a5a8c;
}

.submit-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.error-message {
  margin-top: 1rem;
  padding: 0.75rem;
  background-color: #fff5f5;
  color: #e53e3e;
  border-radius: 6px;
  text-align: center;
}
</style>
