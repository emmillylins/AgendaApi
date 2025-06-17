<template>
  <form @submit.prevent="handleSubmit" class="register-form">
    <!-- Campos do formulário -->
    <div class="form-group">
      <label>Nome Completo:</label>
      <input
        v-model="userData.nome"
        required
        class="form-input"
        :class="{ 'input-error': fieldErrors.nome }"
      />
      <span v-if="fieldErrors.nome" class="error-text">{{
        fieldErrors.nome
      }}</span>
    </div>

    <div class="form-group">
      <label>Nome de Usuário:</label>
      <input
        v-model="userData.nomeUsuario"
        @input="formatNomeUsuario"
        required
        class="form-input"
        :class="{ 'input-error': fieldErrors.nomeUsuario }"
      />
      <small class="input-hint">
        (somente letras minúsculas, números e underline, sem espaços)
      </small>
      <span v-if="fieldErrors.nomeUsuario" class="error-text">{{
        fieldErrors.nomeUsuario
      }}</span>
    </div>

    <div class="form-group">
      <label>CPF:</label>
      <input
        v-model="userData.cpf"
        required
        class="form-input"
        :class="{ 'input-error': fieldErrors.cpf }"
        v-mask="'###.###.###-##'"
      />
      <span v-if="fieldErrors.cpf" class="error-text">{{
        fieldErrors.cpf
      }}</span>
    </div>

    <div class="form-group">
      <label>Data de Nascimento:</label>
      <input
        v-model="userData.dataNasc"
        type="text"
        required
        class="form-input"
        :class="{ 'input-error': fieldErrors.dataNasc }"
        v-mask="'##/##/####'"
        placeholder="dd/mm/aaaa"
      />
      <span v-if="fieldErrors.dataNasc" class="error-text">{{
        fieldErrors.dataNasc
      }}</span>
    </div>

    <div class="form-group">
      <label>Telefone:</label>
      <input
        v-model="userData.telefone"
        required
        class="form-input"
        :class="{ 'input-error': fieldErrors.telefone }"
        v-mask="'(##) #####-####'"
      />
      <span v-if="fieldErrors.telefone" class="error-text">{{
        fieldErrors.telefone
      }}</span>
    </div>

    <div class="form-group">
      <label>Email:</label>
      <input
        v-model="userData.email"
        type="email"
        required
        class="form-input"
        :class="{ 'input-error': fieldErrors.email }"
      />
      <span v-if="fieldErrors.email" class="error-text">{{
        fieldErrors.email
      }}</span>
    </div>

    <div class="form-group">
      <label>Senha:</label>
      <input
        v-model="userData.senha"
        type="password"
        required
        class="form-input"
        :class="{ 'input-error': fieldErrors.senha }"
      />
      <span v-if="fieldErrors.senha" class="error-text">{{
        fieldErrors.senha
      }}</span>
    </div>

    <div class="form-group">
      <label>Confirme a Senha:</label>
      <input
        v-model="userData.confirmSenha"
        type="password"
        required
        class="form-input"
        :class="{ 'input-error': fieldErrors.confirmSenha }"
      />
      <span v-if="fieldErrors.confirmSenha" class="error-text">{{
        fieldErrors.confirmSenha
      }}</span>
    </div>

    <button type="submit" class="submit-btn" :disabled="isSubmitting">
      <span v-if="isSubmitting">
        <i class="fas fa-spinner fa-spin"></i> Registrando...
      </span>
      <span v-else>Registrar</span>
    </button>

    <div v-if="errorMessage" class="error-message">
      <i class="fas fa-exclamation-circle"></i> {{ errorMessage }}
    </div>
  </form>
</template>

<script>
import { authService } from "../api/auth";
import { mask } from "vue-the-mask";

export default {
  name: "RegisterForm",
  directives: { mask },
  data() {
    return {
      userData: {
        nome: "",
        nomeUsuario: "",
        cpf: "",
        dataNasc: "",
        telefone: "",
        email: "",
        senha: "",
        confirmSenha: "",
      },
      fieldErrors: {},
      errorMessage: "",
      isSubmitting: false,
    };
  },
  methods: {
    formatNomeUsuario() {
      this.userData.nomeUsuario = this.userData.nomeUsuario
        .replace(/\s+/g, "_")
        .toLowerCase()
        .replace(/[^a-z0-9_]/g, "");
    },

    validateForm() {
      this.fieldErrors = {};
      let isValid = true;

      // Validação básica dos campos
      if (!this.userData.nome || this.userData.nome.trim().length < 3) {
        this.fieldErrors.nome = "Nome deve ter pelo menos 3 caracteres";
        isValid = false;
      }

      if (
        !this.userData.nomeUsuario ||
        !/^[a-z0-9_]+$/.test(this.userData.nomeUsuario)
      ) {
        this.fieldErrors.nomeUsuario =
          "Use apenas letras minúsculas, números e _";
        isValid = false;
      }

      if (
        !this.userData.cpf ||
        this.userData.cpf.replace(/\D/g, "").length !== 11
      ) {
        this.fieldErrors.cpf = "CPF inválido (11 dígitos necessários)";
        isValid = false;
      }

      if (
        !this.userData.dataNasc ||
        this.userData.dataNasc.replace(/\D/g, "").length !== 8
      ) {
        this.fieldErrors.dataNasc = "Data inválida (use dd/mm/aaaa)";
        isValid = false;
      }

      if (
        !this.userData.telefone ||
        this.userData.telefone.replace(/\D/g, "").length !== 11
      ) {
        this.fieldErrors.telefone = "Telefone inválido (11 dígitos com DDD)";
        isValid = false;
      }

      if (
        !this.userData.email ||
        !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.userData.email)
      ) {
        this.fieldErrors.email = "Email inválido";
        isValid = false;
      }

      if (!this.userData.senha || this.userData.senha.length < 6) {
        this.fieldErrors.senha = "Senha deve ter pelo menos 6 caracteres";
        isValid = false;
      }

      if (this.userData.senha !== this.userData.confirmSenha) {
        this.fieldErrors.confirmSenha = "As senhas não coincidem";
        isValid = false;
      }

      return isValid;
    },

    async handleSubmit() {
      if (this.isSubmitting) return;

      // Validação antes de enviar
      if (!this.validateForm()) {
        return;
      }

      this.isSubmitting = true;
      this.errorMessage = "";
      this.fieldErrors = {};

      try {
        const payload = {
          nome: this.userData.nome.trim(),
          nomeUsuario: this.userData.nomeUsuario,
          cpf: this.userData.cpf.replace(/\D/g, ""),
          dataNasc: this.formatDate(this.userData.dataNasc),
          telefone: this.userData.telefone.replace(/\D/g, ""),
          email: this.userData.email.trim().toLowerCase(),
          senha: this.userData.senha,
        };

        const response = await authService.register(payload);

        if (response.success === false) {
          throw new Error(response.message || "Falha no registro");
        }

        this.$emit("register-success", {
          email: payload.email,
        });
      } catch (error) {
        console.error("Erro no registro:", error);
        this.handleBackendError(error);
      } finally {
        this.isSubmitting = false;
      }
    },

    formatDate(dateString) {
      if (!dateString) return "";
      const [day, month, year] = dateString.split("/");
      return `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}`;
    },

    handleBackendError(error) {
      console.log("Erro completo:", error); // Para debug

      // Limpa erros anteriores
      this.fieldErrors = {};
      this.errorMessage = "";

      // Verifica se é um erro de resposta do servidor
      if (error.response && error.response.data) {
        const responseData = error.response.data;
        console.log("Dados da resposta:", responseData); // Para debug

        // Trata diferentes formatos de erro do backend
        if (responseData.errors) {
          // Se os erros vierem como objeto (por campo)
          if (typeof responseData.errors === "object") {
            Object.keys(responseData.errors).forEach((field) => {
              const fieldName = field.toLowerCase(); // Padroniza o nome do campo
              this.fieldErrors[fieldName] = Array.isArray(
                responseData.errors[field]
              )
                ? responseData.errors[field].join(", ")
                : responseData.errors[field];
            });
          }
          // Se os erros vierem como array de strings
          else if (Array.isArray(responseData.errors)) {
            this.errorMessage = responseData.errors.join("\n");
          }
        }
        // Se houver uma mensagem direta
        else if (responseData.message) {
          this.errorMessage = responseData.message;
        }
      }
      // Se for um erro lançado manualmente
      else if (error.message) {
        this.errorMessage = error.message;
      }
      // Erro genérico
      else {
        this.errorMessage =
          "Ocorreu um erro durante o registro. Por favor, tente novamente.";
      }

      // Força a atualização da UI se necessário
      this.$forceUpdate();
    },
  },
};
</script>

<style scoped>
.register-form {
  max-width: 500px;
  margin: 0 auto;
  padding: 20px;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.form-input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
}

.form-input:focus {
  outline: none;
  border-color: #4361ee;
  box-shadow: 0 0 0 2px rgba(67, 97, 238, 0.2);
}

.input-error {
  border-color: #f44336;
}

.error-text {
  display: block;
  color: #f44336;
  font-size: 0.85rem;
  margin-top: 0.25rem;
}

.input-hint {
  display: block;
  color: #666;
  font-size: 0.85rem;
  margin-top: 0.25rem;
}

.submit-btn {
  width: 100%;
  padding: 12px;
  background-color: #4361ee;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.submit-btn:hover:not(:disabled) {
  background-color: #3a56d4;
}

.submit-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.error-message {
  margin-top: 1rem;
  padding: 12px;
  background-color: #ffebee;
  color: #f44336;
  border-radius: 4px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.error-message i {
  font-size: 1.2rem;
}
</style>
