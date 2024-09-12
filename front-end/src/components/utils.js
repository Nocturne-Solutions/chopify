function checkSession() {
    const token = localStorage.getItem('sessionToken');
    const sessionExpiry = localStorage.getItem('sessionExpiry');

    if (!token || !sessionExpiry || new Date(sessionExpiry) <= new Date()) {
      alert('La sesión ha expirado o no has iniciado sesión.');
      window.location.href = '../index.html';
      return;
    }

    fetch(API_BASE_URL + '/user/validate-token', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
    })
      .then(response => {
        if (response.status === 401) {
            alert('La sesión ha expirado o no has iniciado sesión.');
            window.location.href = '../index.html';
        } else if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }

        return response.json();
      })
      .then(data => {
        if (!data.valid) {
          alert('La sesión ha expirado o no has iniciado sesión.');
          window.location.href = '../index.html';
        }
      })
      .catch(error => {
        console.error('Error en la validación del token:', error);
        checkSession();
      });
}