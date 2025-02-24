import axios from 'axios';

// Create an axios instance with the base URL
const instance = axios.create({
  baseURL: 'https://localhost:7051/api'
});

export function setAuthToken(token) {
  if (token) {
    instance.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete instance.defaults.headers.common['Authorization'];
  }
}

export default instance;
