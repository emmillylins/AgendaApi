<template>
  <div class="agenda-list">
    <table class="agenda-table">
      <thead>
        <tr>
          <th>Nome</th>
          <th>Email</th>
          <th>Telefone</th>
          <th>Ações</th>
        </tr>
      </thead>
      <tbody>
        <!-- Adicione v-if para garantir que agendas existe -->
        <tr v-if="agendas && agendas.length" v-for="agenda in agendas" :key="agenda.id || agenda.Id">
          <td>{{ agenda.nome || agenda.Nome }}</td>
          <td>{{ agenda.email || agenda.Email }}</td>
          <td>{{ agenda.telefone || agenda.Telefone }}</td>
          <td class="actions">
            <button @click="$emit('edit', agenda)" class="edit-btn">
              <i class="fas fa-edit"></i> Editar
            </button>
            <button @click="$emit('delete', agenda.id || agenda.Id)" class="delete-btn">
              <i class="fas fa-trash"></i> Excluir
            </button>
          </td>
        </tr>
        <!-- Mensagem se não houver dados -->
        <tr v-else>
          <td colspan="4" class="no-data">Nenhum contato encontrado</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
export default {
  props: {
    agendas: {
      type: Array,
      required: true,
      default: () => []
    }
  }
}
</script>

<style scoped>
.agenda-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 20px;
}

.agenda-table th, .agenda-table td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #e0e0e0;
}

.agenda-table th {
  background-color: #f5f5f5;
  font-weight: 600;
}

.actions {
  display: flex;
  gap: 8px;
}

.edit-btn, .delete-btn {
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  display: inline-flex;
  align-items: center;
  gap: 5px;
}

.edit-btn {
  background-color: #2196F3;
  color: white;
}

.delete-btn {
  background-color: #f44336;
  color: white;
}

.edit-btn:hover {
  background-color: #0b7dda;
}

.delete-btn:hover {
  background-color: #da190b;
}

.no-data {
  text-align: center;
  padding: 20px;
  color: #666;
}
</style>