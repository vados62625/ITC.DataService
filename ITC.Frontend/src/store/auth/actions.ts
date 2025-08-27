import { createAsyncThunk } from "@reduxjs/toolkit";
import { AuthSlice } from "./slice";
import { addNotification } from "../notification";
export const loginAndFetchUser = createAsyncThunk(
  'auth/loginAndFetch',
  async ({ login, password }: { login: string; password: string }, { rejectWithValue, dispatch }) => {
    try {
      // 1. Логин в систему
      const loginResponse = await fetch('http://89.108.73.166:5017/api/v1/Authorization/Signin', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          userId: login,
          password: password
        })
      });

      if (!loginResponse.ok) {
        dispatch(
          addNotification({
            status: 'alert',
            message: 'Возникла ошибка при входе в систему',
          })
        )
      }

      const loginData = await loginResponse.json();
      const token = loginData.token || loginData.accessToken;

      // Сохраняем токен
      localStorage.setItem("token", token);

      // 2. Получаем информацию о пользователе
      const userResponse = await fetch('http://89.108.73.166:5017/api/v1/Users/AuthorizedContext', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      if (!userResponse.ok) {
        dispatch(
          addNotification({
            status: 'alert',
            message: 'Ошибка получения данных пользователя',
          })
        )
      }

      const userData = await userResponse.json();
      dispatch(AuthSlice.actions.replaceCurrentUser({ id: userData.id, userName: userData.name }))

      return true
    } catch (error: any) {
      dispatch(
        addNotification({
          status: 'alert',
          message: 'Возникла ошибка при входе в систему',
        })
      )
    }
  }
);