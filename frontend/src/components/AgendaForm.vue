<template>
  <form @submit.prevent="handleSubmit" class="agenda-form">
    <div class="form-group">
      <label for="nome">Nome:</label>
      <input
        id="nome"
        v-model.trim="agenda.Nome"
        type="text"
        required
        class="form-input"
        :class="{ 'error': errors.Nome }"
        @input="clearError('Nome')"
      />
      <span v-if="errors.Nome" class="error-message">{{ errors.Nome }}</span>
    </div>

    <div class="form-group">
      <label for="email">Email:</label>
      <input
        id="email"
        v-model.trim="agenda.Email"
        type="email"
        required
        class="form-input"
        :class="{ 'error': errors.Email }"
        @input="clearError('Email')"
      />
      <span v-if="errors.Email" class="error-message">{{ errors.Email }}</span>
    </div>

    <div class="form-group">
      <label for="telefone">Telefone:</label>
      <input
        id="telefone"
        v-model.trim="agenda.Telefone"
        type="tel"
        required
        class="form-input"
        :class="{ 'error': errors.Telefone }"
        @input="formatPhoneNumber"
        @blur="validatePhone"
      />
      <span v-if="errors.Telefone" class="error-message">{{ errors.Telefone }}</span>
    </div>

    <div v-if="submitError" class="form-error">
      <i class="fas fa-exclamation-circle"></i> {{ submitError }}
    </div>

    <div class="form-actions">
      <button
        type="submit"
        :disabled="isSubmitting"
        class="submit-btn"
        :class="{ 'loading': isSubmitting }"
      >
        <template v-if="!isSubmitting">
          <i :class="isEditing ? 'fas fa-sync-alt' : 'fas fa-save'"></i>
          {{ isEditing ? 'Atualizar' : 'Salvar' }}
        </template>
        <template v-else>
          <i class="fas fa-spinner fa-spin"></i> Processando...
        </template>
      </button>
      
      <button
        v-if="isEditing"
        @click="cancelEdit"
        type="button"
        :disabled="isSubmitting"
        class="cancel-btn"
      >
        <i class="fas fa-times"></i> Cancelar
      </button>
    </div>
  </form>
</template>

<script>
export default {
  props: {
    agendaData: {
      type: Object,
      default: () => ({
        Id: null,
        Nome: '',
        Email: '',
        Telefone: ''
      })
    },
    isEditing: Boolean,
    serverErrors: {
      type: Object,
      default: () => ({})
    }
  },
  data() {
    return {
      agenda: { ...this.agendaData },
      isSubmitting: false,
      errors: {
        Nome: '',
        Email: '',
        Telefone: ''
      },
      submitError: ''
    }
  },
  watch: {
    agendaData: {
      handler(newVal) {
        this.agenda = { ...newVal }
        this.clearErrors()
      },
      deep: true
    },
    serverErrors(newErrors) {
      if (newErrors) {
        this.errors = { ...this.errors, ...newErrors }
      }
    }
  },
  methods: {
    validateForm() {
      this.clearErrors()
      let isValid = true

      if (!this.agenda.Nome.trim()) {
        this.errors.Nome = 'Nome é obrigatório'
        isValid = false
      }

      if (!this.agenda.Email.trim()) {
        this.errors.Email = 'Email é obrigatório'
        isValid = false
      } else if (!this.validateEmail(this.agenda.Email)) {
        this.errors.Email = 'Email inválido'
        isValid = false
      }

      if (!this.agenda.Telefone.trim()) {
        this.errors.Telefone = 'Telefone é obrigatório'
        isValid = false
      }

      return isValid
    },
    validateEmail(email) {
      const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
      return re.test(email)
    },
    validatePhone() {
      // Validação básica de telefone (ajuste conforme necessidade)
      const phone = this.agenda.Telefone.replace(/\D/g, '')
      if (phone.length < 10 || phone.length > 11) {
        this.errors.Telefone = 'Telefone inválido'
        return false
      }
      return true
    },
    formatPhoneNumber() {
      // Formatação automática do telefone (11) 99999-9999
      let phone = this.agenda.Telefone.replace(/\D/g, '')
      if (phone.length > 2) {
        phone = `(${phone.substring(0, 2)}) ${phone.substring(2)}`
      }
      if (phone.length > 10) {
        phone = `${phone.substring(0, 10)}-${phone.substring(10, 14)}`
      }
      this.agenda.Telefone = phone
    },
    clearError(field) {
      this.errors[field] = ''
    },
    clearErrors() {
      this.errors = {
        Nome: '',
        Email: '',
        Telefone: ''
      }
      this.submitError = ''
    },
    async handleSubmit() {
      if (!this.validateForm()) return

      this.isSubmitting = true
      this.submitError = ''

      try {
        await this.$emit('submit', { 
          ...this.agenda,
          Telefone: this.agenda.Telefone.replace(/\D/g, '') // Remove formatação antes de enviar
        })
      } catch (error) {
        console.error('Erro no formulário:', error)
        this.submitError = 'Ocorreu um erro ao processar a solicitação. Por favor, tente novamente.'
      } finally {
        this.isSubmitting = false
      }
    },
    cancelEdit() {
      this.clearErrors()
      this.$emit('cancel')
    }
  }
}
</script>

<style scoped>
.agenda-form {
  max-width: 600px;
  margin: 0 auto;
  padding: 20px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #333;
}

.form-input {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 16px;
  transition: border-color 0.3s;
}

.form-input:focus {
  border-color: #2196F3;
  outline: none;
  box-shadow: 0 0 0 2px rgba(33, 150, 243, 0.2);
}

.form-input.error {
  border-color: #f44336;
}

.error-message {
  display: block;
  margin-top: 0.5rem;
  color: #f44336;
  font-size: 0.875rem;
}

.form-error {
  margin: 1rem 0;
  padding: 0.75rem;
  background-color: #ffebee;
  color: #f44336;
  border-radius: 4px;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
}

.submit-btn, .cancel-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.3s;
}

.submit-btn {
  background-color: #2196F3;
  color: white;
}

.submit-btn:hover:not(:disabled) {
  background-color: #0b7dda;
}

.submit-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.cancel-btn {
  background-color: #f5f5f5;
  color: #333;
}

.cancel-btn:hover {
  background-color: #e0e0e0;
}

.fa-spinner {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>