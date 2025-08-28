import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react'

export const apiSlice = createApi({
  reducerPath: 'api',
  // baseQuery: fakeBaseQuery(),
  baseQuery: fetchBaseQuery({
    baseUrl: 'http://89.108.73.166:5016/',
  }),
  endpoints: () => ({}),
  tagTypes: ['Engine'],
})
