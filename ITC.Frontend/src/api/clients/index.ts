import { axiosInstance } from '../axiosInstance'

import { EnginesClient } from './BFFClient'

export const Clients = {
  EnginesClient: new EnginesClient('', axiosInstance),
}

export * from './BFFClient'
