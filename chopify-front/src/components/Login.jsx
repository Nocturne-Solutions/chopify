// components/Login.js
import React, { useState } from 'react';
import { Button, TextField } from '@mui/material';

const Login = () => {
    const [username, setUsername] = useState('');

    const handleSubmit = (event) => {
        event.preventDefault();
        localStorage.setItem('username', username);
        window.location.href = '/topmusic';
    };

    return (
        <div>
            <h1>Iniciar sesión en Chopify</h1>
            <form onSubmit={handleSubmit}>
                <TextField
                    label="Username"
                    value={username}
                    onChange={(event) => setUsername(event.target.value)}
                />
                <Button type="submit">Iniciar sesión</Button>
            </form>
        </div>
    );
};

export default Login;