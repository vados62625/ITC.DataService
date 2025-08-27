import React, { FC, useState } from "react";
import { Button } from "@consta/uikit/Button";
import { Text } from "@consta/uikit/Text";
import { TextField } from "@consta/uikit/TextField";
import css from "./style.css";
import { loginAndFetchUser, useAppDispatch } from "../../store";
import { useNavigate } from "react-router-dom";
import { RoutePaths } from "../../types";

export const Auth: FC = () => {
  const [login, setLogin] = useState<string | null>(null);
  const [password, setPassword] = useState<string | null>(null);

  const navigate = useNavigate();
  const dispatch = useAppDispatch()

  const onChangeLogin = ({ value }: { value: string | null }) =>
    setLogin(value);

  const onChangePassword = ({ value }: { value: string | null }) =>
    setPassword(value);

  const onLogin = () => {
    dispatch(loginAndFetchUser({ login: login ?? '', password: password ?? '' })).unwrap().then(isLogin => {
      if (isLogin) {
        navigate(RoutePaths.Registry);
      }
      else {
        onChangeLogin({ value: null })
        onChangePassword({ value: null })
      }
    }
    )
  };

  return (
    <div className="container-column align-center justify-center w-100 h-100">
      <div className={css.loginForm}>
        <Text size="xl" weight="bold" className="m-b-6">
          Войти в систему
        </Text>
        <TextField
          className="m-b-6"
          onChange={onChangeLogin}
          value={login}
          type="text"
          placeholder="Введите логин"
          label="Логин"
          required
          withClearButton
          width="full"
        />
        <TextField
          className="m-b-6"
          onChange={onChangePassword}
          value={password}
          type="password"
          placeholder="Введите пароль"
          label="Пароль"
          required
          withClearButton
          width="full"
        />
        <Button
          className="m-t-4"
          label="Войти"
          size="m"
          onClick={onLogin}
        />
      </div>
    </div>
  );
};
