async function checkSession() {
    const token = localStorage.getItem('sessionToken');
    const sessionExpiry = localStorage.getItem('sessionExpiry');

    if (!token || !sessionExpiry || new Date(sessionExpiry) <= new Date()) {
      alert('La sesión ha expirado o no has iniciado sesión.');
      window.location.href = '../index.html';
      return;
    }

    const result = await fetch(API_BASE_URL + '/user/validate-token', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
    });

    if (result.status === 401) {
      alert('La sesión ha expirado o no has iniciado sesión.');
      window.location.href = '../index.html';
      return;
    }

    if (!result.ok) {
      console.error('Error en la validación del token:', error);
      checkSession();
    }

    const data = await result.json();

    if (!data.valid) {
      alert('La sesión ha expirado o no has iniciado sesión.');
      window.location.href = '../index.html';
    }
}