<template>
  <header class="app-header">
    <div class="header-content">
      <h1>AgendaAPI</h1>
    </div>
  </header>
</template>

<script>
export default {
  name: 'AppHeader'
}
</script>

<style scoped>
.app-header {
  background-color: #2196F3;
  color: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  width: 100%;
  top: 0;
}

.header-content {
  margin: 0 auto;
  padding: 0 20px;
  text-align: center;
}

.app-header h1 {
  margin: 0;
  font-size: 1.8rem;
  font-weight: 500;
}
</style>