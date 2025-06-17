import apiClient from './authClient';

// Funções básicas da API
export const getAgendasApi = async () => {
  const response = await apiClient.get('/agendas');
  return response.data;
};

export const createAgendaApi = async (agenda) => {
  const response = await apiClient.post('/agendas', agenda);
  return response.data;
};

export const updateAgendaApi = async (agenda) => {
  const response = await apiClient.put('/agendas', agenda);
  return response.data;
};

export const deleteAgendaApi = async (id) => {
  const response = await apiClient.delete(`/agendas/${id}`);
  return response.data;
};
