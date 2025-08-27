import type { InternalAxiosRequestConfig } from 'axios'
import axios from 'axios'

export const axiosInstance = axios.create({baseURL: 'http://89.108.73.166:5016/'})

const setupConfig = (config: InternalAxiosRequestConfig) => {
  const token = localStorage.getItem('token')
  const access = localStorage.getItem('access')

  config.headers.authorization = `Bearer ${token}`
  config.headers.access = access

  return config
}

axiosInstance.interceptors.request.use(setupConfig)
