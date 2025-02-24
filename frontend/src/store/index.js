import { createStore } from 'vuex';
import axios, { setAuthToken } from '../services/axios'; // Import the configured axios instance and setAuthToken

const store = createStore({
  state: {
    user: null,
    projects: [],
    token: null // Add token to the state
  },
  mutations: {
    SET_USER(state, user) {
      state.user = user;
      console.log('User set:', user);
    },
    SET_PROJECTS(state, projects) {
      state.projects = projects;
    },
    SET_TOKEN(state, token) {
      state.token = token; // Mutation to set token
      setAuthToken(token); // Set the token in axios instance
      console.log('Token set:', token);
    }
  },
  actions: {
    async loginConstruct({ commit }, { email, password }) {
      const response = await axios.post('/auth/login', { email, password }); // No need to specify the full URL
      const { user, token } = response.data;
      commit('SET_USER', user);
      commit('SET_TOKEN', token); // Store the token
    },
    setUserConstruct({ commit }, { user, token }) {
      console.log('User set:', user);
      console.log('Token set:', token);
      commit('SET_USER', user);
      commit('SET_TOKEN', token); // Store the token
    },
    async fetchProjects({ commit }) {
      const response = await axios.get('/constructionprojects');
      commit('SET_PROJECTS', response.data);
    },
    async createProject({ dispatch }, project) {
      await axios.post('/constructionprojects', project);
      dispatch('fetchProjects');
    },
    async updateProject({ dispatch }, project) {
      await axios.put(`/constructionprojects/${project.projectId}`, project);
      dispatch('fetchProjects');
    },
    async deleteProject({ dispatch }, projectId) {
      await axios.delete(`/constructionprojects/${projectId}`);
      dispatch('fetchProjects');
    }
  }
});

export default store;
