<template>
  <div id="app">
    <!-- Header Global -->
    <header class="app-header">
      <div class="header-container">
        <div class="header-content">
          <h1 class="header-title">AgendaAPI</h1>
          <button 
            v-if="showLogoutButton" 
            @click="logout" 
            class="logout-button">
            Sair
          </button>
        </div>
      </div>
    </header>

    <!-- Conteúdo Principal -->
    <main class="main-content">
      <router-view v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" :key="$route.fullPath" />
        </transition>
      </router-view>
    </main>
  </div>
</template>

<script>
import { authService } from "./api/auth"

export default {
  name: 'App',
  computed: {
    isAuthenticated() {
      return authService.isAuthenticated()
    },
    showLogoutButton() {
      // Mostra o botão apenas se estiver autenticado E na rota /agendas
      return this.isAuthenticated && this.$route.path === '/agendas'
    }
  },
  methods: {
    logout() {
      authService.logout()
      this.$router.push('/login')
    }
  }
}
</script>

<style>
/* Reset e estilos globais */
:root {
  --primary-color: #4a6fa5;
  --secondary-color: #6c757d;
  --success-color: #28a745;
  --danger-color: #dc3545;
  --light-color: #f8f9fa;
  --dark-color: #343a40;
  --border-radius: 4px;
  --box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  line-height: 1.6;
  color: #333;
  background-color: #f5f5f5;
}

#app {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Estilos do Header */
.app-header {
  background-color: #4a6fa5;
  color: white;
  padding: 0.5rem 1rem;
  position: sticky;
  top: 0;
  z-index: 1000;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  height: 60px; /* Altura fixa */
}

.header-container {
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
  height: 100%;
}

.header-content {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  position: relative;
}

.header-title {
  font-size: 1.5rem;
  font-weight: 500;
  color: white;
  text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.2);
  margin: 0;
  padding: 0;
}

.logout-button {
  background-color: transparent;
  color: white;
  border: 1px solid white;
  border-radius: 4px;
  padding: 6px 12px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s;
  position: absolute;
  right: 0;
}

/* Estilos do Conteúdo Principal */
.main-content {
  flex: 1;
  background-color: white;
  border-radius: var(--border-radius);
  box-shadow: var(--box-shadow);
}

/* Botão de Logout */
.logout-button {
  background-color: transparent;
  color: white;
  border: 1px solid white;
  border-radius: 4px;
  padding: 6px 12px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s;
  margin-left: auto; /* Garante que fique à direita */
}

.logout-button:hover {
  background-color: rgba(255, 255, 255, 0.1);
}

/* Transições e Responsividade */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

@media (max-width: 768px) {
  .main-content {
    margin: 10px;
    padding: 15px;
    width: calc(100% - 20px);
  }
  
  .header-title {
    font-size: 1.3rem;
  }
  
  .logout-button {
    padding: 4px 8px;
    font-size: 0.8rem;
  }
}

/* Animações */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>