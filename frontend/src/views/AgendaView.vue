<template>
  <div class="agenda-container">
    <div class="agenda-content">
      <!-- Modal de Erro -->
      <transition name="fade">
        <div v-if="errorModal.show" class="modal-overlay">
          <div class="modal-content error-modal">
            <div class="modal-header">
              <i class="fas fa-exclamation-triangle error-icon"></i>
              <h3>Ocorreu um erro</h3>
            </div>
            <div class="modal-body">
              <p>{{ errorModal.message }}</p>
            </div>
            <div class="modal-footer">
              <button
                @click="retryAction"
                class="btn retry-btn"
                v-if="errorModal.retry"
              >
                <i class="fas fa-redo"></i> Tentar Novamente
              </button>
              <button @click="closeErrorModal" class="btn cancel-btn">
                <i class="fas fa-times"></i> Fechar
              </button>
            </div>
          </div>
        </div>
      </transition>

      <!-- Alerta de erro -->
      <transition name="slide-down">
        <div v-if="errorMessage" class="alert alert-error">
          <div class="alert-content">
            <i class="fas fa-exclamation-circle"></i>
            <span>{{ errorMessage }}</span>
          </div>
          <button @click="errorMessage = ''" class="alert-close">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </transition>

      <!-- Layout de duas colunas -->
      <div class="agenda-grid">
        <!-- Coluna da Lista de Contatos -->
        <div class="contacts-list">
          <div v-if="!loading" class="list-container">
            <AgendaList
              :agendas="agendas"
              @edit="openEditModal"
              @delete="confirmDelete"
            />
          </div>
          
          <div v-if="loading" class="loading-state">
            <div class="loading-spinner">
              <div class="spinner"></div>
            </div>
            <p>Carregando contatos...</p>
          </div>
        </div>

        <!-- Coluna do Formulário -->
        <div class="form-container">
          <div class="form-card">
            <h2>{{ isEditing ? "Editar Contato" : "Novo Contato" }}</h2>
            <AgendaForm
              :agendaData="currentAgenda"
              :isEditing="isEditing"
              :errors="formErrors"
              @submit="handleSubmit"
              @cancel="cancelEdit"
            />
          </div>
        </div>
      </div>

      <!-- Modal de Edição -->
      <transition name="fade">
        <div v-if="showModal" class="modal-overlay">
          <div class="modal-content">
            <div class="modal-header">
              <h2>Editar Contato</h2>
            </div>
            <div class="modal-body">
              <AgendaForm
                :agendaData="currentAgenda"
                :isEditing="true"
                :errors="formErrors"
                @submit="handleModalSubmit"
                @cancel="closeModal"
              />
            </div>
          </div>
        </div>
      </transition>

      <!-- Confirmação de Exclusão -->
      <transition name="fade">
        <div v-if="showDeleteModal" class="modal-overlay">
          <div class="modal-content delete-modal">
            <div class="modal-header">
              <i class="fas fa-trash-alt delete-icon"></i>
              <h3>Confirmar Exclusão</h3>
            </div>
            <div class="modal-body">
              <p>Tem certeza que deseja excluir este contato?</p>
            </div>
            <div class="modal-footer">
              <button
                @click="deleteAgenda"
                class="btn confirm-btn"
                :disabled="deleting"
              >
                <span v-if="deleting" class="btn-loading">
                  <span class="spinner"></span>
                </span>
                <span v-else>Confirmar</span>
              </button>
              <button @click="showDeleteModal = false" class="btn cancel-btn">
                Cancelar
              </button>
            </div>
            <div v-if="deleteError" class="modal-error">
              <i class="fas fa-exclamation-circle"></i>
              <span>{{ deleteError }}</span>
            </div>
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script>
import AgendaForm from "../components/AgendaForm.vue";
import AgendaList from "../components/AgendaList.vue";
import {
  getAgendasApi,
  createAgendaApi,
  updateAgendaApi,
  deleteAgendaApi,
} from "../api/agendas";

export default {
  components: { AgendaForm, AgendaList },
  data() {
    return {
      agendas: [],
      currentAgenda: { Nome: "", Email: "", Telefone: "" },
      formErrors: {},
      isEditing: false,
      showModal: false,
      showDeleteModal: false,
      agendaToDelete: null,
      errorMessage: "",
      deleteError: "",
      loading: false,
      deleting: false,
      errorModal: {
        show: false,
        message: "",
        retry: null,
        originalData: null,
      },
    };
  },
  created() {
    this.loadAgendas();
  },
  methods: {
    async loadAgendas() {
      this.loading = true;
      try {
        this.agendas = await getAgendasApi();
      } catch (error) {
        this.showErrorModal(
          error.message || "Erro ao carregar contatos",
          this.loadAgendas
        );
      } finally {
        this.loading = false;
      }
    },

    async handleSubmit(agenda) {
      try {
        if (this.isEditing) {
          await updateAgendaApi(agenda);
        } else {
          await createAgendaApi(agenda);
        }
        await this.loadAgendas(); // Recarrega a lista completa
        this.resetCurrentAgenda();
        this.isEditing = false;
        this.showModal = false;
      } catch (error) {
        if (error.validationErrors) {
          this.formErrors = error.validationErrors;
        } else {
          this.showErrorModal(
            error.message || "Erro ao salvar contato",
            () => this.handleSubmit(agenda),
            agenda
          );
        }
      }
    },

    handleModalSubmit(agenda) {
      this.handleSubmit(agenda);
    },

    async deleteAgenda() {
      this.deleting = true;
      try {
        await deleteAgendaApi(this.agendaToDelete);
        await this.loadAgendas(); // Atualiza toda a lista após exclusão
        this.showDeleteModal = false;
      } catch (error) {
        this.deleteError = error.message || "Erro ao excluir contato";
      } finally {
        this.deleting = false;
      }
    },

    openEditModal(agenda) {
      this.currentAgenda = { ...agenda };
      this.isEditing = true;
      this.showModal = true;
    },

    closeModal() {
      this.showModal = false;
      this.resetCurrentAgenda();
    },

    cancelEdit() {
      this.resetCurrentAgenda();
      this.isEditing = false;
    },

    confirmDelete(id) {
      this.agendaToDelete = id;
      this.showDeleteModal = true;
    },

    showErrorModal(message, retry = null, originalData = null) {
      this.errorModal = {
        show: true,
        message,
        retry,
        originalData,
      };
    },

    closeErrorModal() {
      this.errorModal.show = false;
      this.resetCurrentAgenda();
    },

    async retryAction() {
      if (this.errorModal.retry) {
        try {
          await this.errorModal.retry();
          this.errorModal.show = false;
        } catch (error) {
          this.showErrorModal(
            error.message || "Erro ao tentar novamente",
            this.errorModal.retry,
            this.errorModal.originalData
          );
        }
      }
    },

    resetCurrentAgenda() {
      this.currentAgenda = { Nome: "", Email: "", Telefone: "" };
      this.formErrors = {};
    },
  },
};
</script>

<style scoped>
/* Variáveis de cores */
:root {
  --primary-color: #4361ee;
  --danger-color: #f72585;
  --success-color: #4cc9f0;
  --warning-color: #f8961e;
  --light-color: #f8f9fa;
  --dark-color: #212529;
  --border-radius: 8px;
  --box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  --transition: all 0.3s ease;
}

/* Estilos base */
.agenda-container {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.agenda-content {
  position: relative;
}

/* Layout de duas colunas */
.agenda-grid {
  display: grid;
  gap: 2rem;
  align-items: flex-start;
}

.contacts-list {
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--box-shadow);
  padding: 1.5rem;
  height: 100%;
}

.form-container {
  position: sticky;
  top: 1rem;
}

.form-card {
  background: white;
  border-radius: var(--border-radius);
  box-shadow: var(--box-shadow);
  padding: 1.5rem;
}

/* Ajustes para o formulário */
.form-card h2 {
  margin-top: 0;
  margin-bottom: 1.5rem;
  color: var(--primary-color);
  font-size: 1.5rem;
  text-align: center;
}

/* Responsividade */
@media (max-width: 1024px) {
  .agenda-grid {
    grid-template-columns: 1fr;
  }
  
  .form-container {
    position: static;
    margin-top: 2rem;
  }
}

/* Alertas e notificações */
.alert {
  position: fixed;
  top: 1rem;
  right: 1rem;
  max-width: 400px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  border-radius: var(--border-radius);
  box-shadow: var(--box-shadow);
  z-index: 100;
  animation: slideIn 0.3s ease-out;
}

.alert-error {
  background-color: #fff5f5;
  border-left: 4px solid var(--danger-color);
  color: var(--danger-color);
}

.alert-content {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.alert-close {
  background: none;
  border: none;
  color: inherit;
  cursor: pointer;
  margin-left: 1rem;
  opacity: 0.7;
  transition: var(--transition);
}

.alert-close:hover {
  opacity: 1;
}

/* Modais */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}

.modal-content {
  background-color: white;
  border-radius: var(--border-radius);
  box-shadow: var(--box-shadow);
  width: 90%;
  max-width: 500px;
  overflow: hidden;
}

.modal-header {
  padding: 1.5rem 1.5rem 0;
  text-align: center;
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  padding: 0 1.5rem 1.5rem;
  display: flex;
  justify-content: center;
  gap: 1rem;
}

/* Modal de erro específico */
.error-modal {
  border-top: 4px solid var(--danger-color);
}

.error-icon {
  color: var(--danger-color);
  font-size: 1.5rem;
  margin-bottom: 1rem;
}

/* Modal de exclusão */
.delete-modal {
  border-top: 4px solid var(--warning-color);
}

.delete-icon {
  color: var(--warning-color);
  font-size: 1.5rem;
  margin-bottom: 1rem;
}

.modal-error {
  padding: 0.75rem 1.5rem;
  background-color: #fff5f5;
  color: var(--danger-color);
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
}

/* Botões */
.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: var(--border-radius);
  font-weight: 500;
  cursor: pointer;
  transition: var(--transition);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.confirm-btn {
  background-color: var(--danger-color);
  color: white;
}

.confirm-btn:hover:not(:disabled) {
  background-color: #d3166d;
  transform: translateY(-2px);
}

.cancel-btn {
  background-color: #e9ecef;
  color: var(--dark-color);
}

.cancel-btn:hover {
  background-color: #dee2e6;
  transform: translateY(-2px);
}

.retry-btn {
  background-color: var(--primary-color);
  color: white;
}

.retry-btn:hover:not(:disabled) {
  background-color: #3a56d4;
  transform: translateY(-2px);
}

/* Loading */
.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  text-align: center;
}

.loading-spinner {
  margin-bottom: 1rem;
}

.spinner {
  width: 3rem;
  height: 3rem;
  border: 4px solid rgba(0, 0, 0, 0.1);
  border-radius: 50%;
  border-top-color: var(--primary-color);
  animation: spin 1s linear infinite;
}

.btn-loading .spinner {
  width: 1rem;
  height: 1rem;
  border-width: 2px;
  margin-right: 0.5rem;
}

/* Animações */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@keyframes slideIn {
  from {
    transform: translateY(-20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.3s ease;
}

.slide-down-enter-from,
.slide-down-leave-to {
  transform: translateY(-20px);
  opacity: 0;
}

/* Responsividade */
@media (max-width: 768px) {
  .agenda-container {
    padding: 1rem;
  }
  
  .modal-content {
    width: 95%;
  }
  
  .modal-footer {
    flex-direction: column;
  }
  
  .btn {
    width: 100%;
  }
}
</style>