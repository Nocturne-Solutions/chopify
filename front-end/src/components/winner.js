document.addEventListener('DOMContentLoaded', function() {
    const token = localStorage.getItem('sessionToken');
    const winnerContainer = document.getElementById('winnerContent');
    const loadingContainer = document.getElementById('loadingContent');

    let status;

    init();

    async function init() {
        showSpinner();
        checkSession();
        status = await checkStatus();
        if (status.state === STATES.WAITING) {
            updateWinner(await getWinner());
        }
        updateView();

        setInterval(async function() {
            status = await checkStatus();
            updateView();
        }, 1000);
    }

    function updateView() {
        if (status.state === STATES.STOPPING) {
            showSpinner();
            document.getElementById('loadingText').textContent = 'Esperando a que se activen las votaciones...';
            document.getElementById("loadingText").style.display = "block";
        } else if (status.state === STATES.VOTING) {
            hideSpinner();
            window.location.href = 'ranking.html';
        } else if (status.state === STATES.WAITING) {
            hideSpinner();
            document.getElementById('waitingMessage').textContent = `Siguiente ronda en ${secondsToMMSS(status.currentStateRemainingTimeSeconds)}`;
        } else if (status.state === STATES.UNKNOWN) {
            showSpinner();
            document.getElementById('loadingText').textContent = 'Intentando reconectar con el servidor...';
            document.getElementById("loadingText").style.display = "block";
        }
    }

    async function getWinner() {
        const response = await fetch(API_BASE_URL + '/winner/last', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            },
        });

        if (response.status === 401) {
            alert('La sesión ha expirado o no has iniciado sesión.');
            window.location.href = '../index.html';
        } 
        else if (!response.ok) {
            console.error('Error al obtener el ganador: ', error);
            await getWinner();
        }

        return await response.json();
    }

    function updateWinner(winner) {
        document.getElementById('winnerName').textContent = winner.name;
        document.getElementById('winnerArtists').textContent = winner.artists;
        document.getElementById('winnerCover').src = winner.coverUrl;
        document.getElementById('winnerSuggestedBy').textContent = `Sugerida por ${winner.suggestedBy} 🎉`;
        document.getElementById('winnerVotes').textContent = `${winner.votes} votos`;
    }

    function hideSpinner() {
        winnerContainer.style.display = 'block';
        loadingContainer.style.display = 'none';
    }

    function showSpinner() {
        winnerContainer.style.display = 'none';
        loadingContainer.style.display = 'block';

        document.getElementById("loadingText").style.display = "none";
    }
});