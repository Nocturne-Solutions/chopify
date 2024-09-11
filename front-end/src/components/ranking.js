document.addEventListener('DOMContentLoaded', function() {
    const suggestButton = document.getElementById('suggestBtn');
    const username = localStorage.getItem('sessionUser');
    const token = localStorage.getItem('sessionToken');
    const sessionExpiry = localStorage.getItem('sessionExpiry');
      
    /*fetch('https://tu-servidor.com/protected-data', {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })
      .then(response => response.json())
      .then(data => {
        console.log('Datos protegidos:', data);
      })
      .catch(error => console.error('Error obteniendo datos protegidos:', error));*/

    suggestButton.addEventListener('click', function() {
        window.location.href = 'suggestions.html';
    });

    checkSession();

    document.querySelector('.user-label').textContent = username;
});