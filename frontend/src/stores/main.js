import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
//import axios from 'axios'

import axios from '../services/axios'; // Import the configured axios instance and setAuthToken

// Set up axios instance with authentication token handling
export const setAuthToken = (token) => {
  if (token) {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
  } else {
    delete axios.defaults.headers.common['Authorization']
  }
}

export const useMainStore = defineStore('main', () => {
  const userName = ref('John Doe')
  const userId = ref('')
  const userEmail = ref('doe.doe.doe@example.com')
  const token = ref(null)
  const projects = ref([])

  const userAvatar = computed(
    () =>
      `https://api.dicebear.com/7.x/avataaars/svg?seed=${userEmail.value.replace(
        /[^a-z0-9]+/gi,
        '-'
      )}`
  )

  const isFieldFocusRegistered = ref(false)
  const clients = ref([])
  const history = ref([])

  function setUser(payload) {
    console.log('User set:', payload)
    if (payload.name) {
      userName.value = payload.email
    }
    if (payload.userid) {
      userId.value = payload.userid
    }
    if (payload.email) {
      userEmail.value = payload.email
    }
    if (payload.token) {
      token.value = payload.token
      setAuthToken(payload.token)
    }
  }

  async function login(email, password) {
    try {
      const response = await axios.post('/auth/login', { email, password })
      const { user, token } = response.data
      console.log('User set:', user)
      setUser({ name: user.email, userid: user.userId, email: user.email, token })

    } catch (error) {
      alert('Login failed: ' + error.message)
    }
  }

  async function fetchProjects() {
    try {
      const response = await axios.get('/constructionprojects')
      projects.value = response.data
    } catch (error) {
      alert('Failed to fetch projects: ' + error.message)
    }
  }

  async function createProject(project) {
    try {
      await axios.post('/constructionprojects', project)
      fetchProjects()
    } catch (error) {
      alert('Failed to create project: ' + error.message)
    }
  }

  async function updateProject(project) {
    try {
      await axios.put(`/constructionprojects/${project.projectId}`, project)
      fetchProjects()
    } catch (error) {
      alert('Failed to update project: ' + error.message)
    }
  }

  async function deleteProject(projectId) {
    try {
      await axios.delete(`/constructionprojects/${projectId}`)
      fetchProjects()
    } catch (error) {
      alert('Failed to delete project: ' + error.message)
    }
  }

  

  return {
    userName,
    userEmail,
    token,
    projects,
    userAvatar,
    isFieldFocusRegistered,
    clients,
    history,
    setUser,
    login,
    fetchProjects,
    createProject,
    updateProject,
    deleteProject,
  }
})
