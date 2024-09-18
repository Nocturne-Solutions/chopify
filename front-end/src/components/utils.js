async function checkSession() {
    const token = localStorage.getItem('sessionToken');
    const sessionExpiry = localStorage.getItem('sessionExpiry');

    if (!token || !sessionExpiry || new Date(sessionExpiry) <= new Date()) {
      alert('La sesión ha expirado o no has iniciado sesión.');
      window.location.href = '../index.html';
      return;
    }

    try
    {
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
    catch (error)
    {
      console.error('Error en la validación del token:', error);
      await checkSession();
    }
}

const STATES = {
  UNKNOWN: -1,
  STOPPING: 0,
  VOTING: 1,
  WAITING: 2
};

async function checkStatus() {
  const token = localStorage.getItem('sessionToken');

  try
  {
    const result = await fetch(API_BASE_URL + '/voting-system/status', {
      method: 'GET',
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
      console.error('Error al obtener el estado del sistema de votación: ', error);
      window.location.href = '../pages/winner.html';
      return { state: STATES.UNKNOWN, currentStateRemainingTimeSeconds: 0 };
    }
  
    return await result.json();
  }
  catch (error)
  {
    console.error('Error al obtener el estado del sistema de votación: ', error);
    return { state: STATES.UNKNOWN, currentStateRemainingTimeSeconds: 0 };
  }
}

function secondsToMMSS(seconds) {
  let minutes = Math.floor(Math.round(seconds) / 60);
  let remainingSeconds = Math.round(seconds) % 60;

  minutes = minutes < 10 ? '0' + minutes : minutes;
  remainingSeconds = remainingSeconds < 10 ? '0' + remainingSeconds : remainingSeconds;

  return minutes + ':' + remainingSeconds;
}

function secondsToHHMMSS(seconds) {
  let hours = Math.floor(Math.round(seconds) / 3600);
  let minutes = Math.floor(Math.round(seconds) / 60) % 60;
  let remainingSeconds = Math.round(seconds) % 60;

  hours = hours < 10 ? '0' + hours : hours;
  minutes = minutes < 10 ? '0' + minutes : minutes;
  remainingSeconds = remainingSeconds < 10 ? '0' + remainingSeconds : remainingSeconds;

  return hours + ':' + minutes + ':' + remainingSeconds;
}