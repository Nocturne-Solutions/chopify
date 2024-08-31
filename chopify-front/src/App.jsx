// App.js
import React from 'react';
import Login from './components/Login';
import TopMusic from './components/TopMusic';
const App = () => {
    const username = localStorage.getItem('username');

    if (!username) {
        return <Login />;
    }

    return <TopMusic />;
};

export default App;