import React from 'react';
import { User } from '../types';

interface IUserContext {
    user?: User;
    handleUserChange?: (user?:User) => void;
}

const defaultState = {
    user: undefined,
};

export const userContext = React.createContext<IUserContext>(defaultState);