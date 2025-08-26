import { User } from "../../types";
// import { loginInSystem } from "../../api";
import { AppDispatch } from "../store";
import { authSlice } from "./slice";

/**
 * Вход пользователя в систему
 * @param login логин
 * @param password пароль
 */
export const authentication =
  (login: string, password: string) => async (dispatch: AppDispatch) => {
    try {
      // await loginInSystem(login, password).then((res) => {
      //   console.log(res);
      //   // authSlice.actions.replaceCurrentUser()
      //   localStorage.setItem("token", res); // Сохранение токена в localStorage
      // });
    } catch (error) {
      console.error(error);
      alert("Ошибка аутентификации");
    }
  };

/**
 * Устанавливает значение текущего пользователя
 * @param value значение текущего пользователя
 */
export const setCurrentUser =
  (value: User | null) => (dispatch: AppDispatch) => {
    dispatch(authSlice.actions.replaceCurrentUser(value));
  };
