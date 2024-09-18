document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchInput');
    const songList = document.getElementById('songList');
    const listContainer = document.querySelector('.list-container');
    const loadingSpinner = document.getElementById('loadingSpinner');
    const token = localStorage.getItem('sessionToken');
    const username = localStorage.getItem('sessionUser');

    let debounceTimeout;
    let status;

    const suggestButtonHtml = `<button class="suggest-btn">
                                    <i class="fas fa-lightbulb"></i>
                                    <i class="fas fa-check"></i>
                                </button>`;

    function suggestedLabelHtml(suggestedBy) {
        return `<div class="suggested-label">Ya fue sugerida por <span class="suggested-by">${suggestedBy}</span></div>`;
    }

    function inCooldownLabelHtml(cooldownTimeLeft) {
        return `<div class="cooldown-label">Se puede volver a sugerir en <span class="cooldown-time">${secondsToHHMMSS(cooldownTimeLeft)}</span></div>`;
    }

    init();
    
    async function init() {
        checkSession();
        status = await checkStatus();
        if (status.state !== STATES.VOTING) {
            window.location.href = 'winner.html';
        }
        filterSongs('');
        setInterval(async function() {
            checkSuggestions();
            checkInCooldown();
            status = await checkStatus();
            if (status.state !== STATES.VOTING) {
                window.location.href = 'winner.html';
            }
        }, 1000);
    }

    function updateListElement(li, data)
    {
        let name = li.getAttribute('data-song-name');
        let artists = li.getAttribute('data-song-artists');
        let coverUrl = li.getAttribute('data-song-cover-url');

        li.classList.toggle('suggested', data.isSuggested);
        li.classList.toggle('inCooldown', data.isInCooldown);

        li.innerHTML = `
            <img src="${coverUrl}" alt="Cover" class="song-cover">
            <div class="song-details">
                <div class="song-name">${name}</div>
                <div class="song-artists">${artists}</div>
            </div>
            ${
                data.isInCooldown
                    ? inCooldownLabelHtml(data.cooldownTimeLeft)
                    : (data.isSuggested 
                        ? suggestedLabelHtml(data.suggestedBy) 
                        : suggestButtonHtml)
            }
        `;

        if (!data.isInCooldown && !data.isSuggested) {
            li.querySelector('.suggest-btn').addEventListener('click', suggestItem);
        }
    }

    function checkSuggestions()
    {
        fetch(API_BASE_URL + '/suggestion', {
            method: 'GET',
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
            songList.querySelectorAll('li').forEach(li => {
                let songId = li.getAttribute('data-song-id');
                let suggestion = data.find(suggestion => suggestion.id === songId);

                if (suggestion && !li.classList.contains('suggested') && !li.classList.contains('inCooldown')) {
                    updateListElement(li, { isSuggested: true, isInCooldown: false, suggestedBy: suggestion.suggestedBy, cooldownTimeLeft: 0 });
                }
                else if (!suggestion && li.classList.contains('suggested') && !li.classList.contains('inCooldown')) {
                    updateListElement(li, { isSuggested: false, isInCooldown: false, suggestedBy: '', cooldownTimeLeft: 0 });
                }
            });
        })
        .catch(error => {
            console.error('Error al obtener las sugerencias:', error);
        });
    }

    function checkInCooldown()
    {
        fetch(API_BASE_URL + '/cooldown', {
            method: 'GET',
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
            songList.querySelectorAll('li').forEach(li => {
                let songId = li.getAttribute('data-song-id');
                let cooldown = data.find(cooldown => cooldown.id === songId);

                if (cooldown && !li.classList.contains('inCooldown')) {
                    updateListElement(li, { isSuggested: li.classList.contains('suggested'), isInCooldown: true, suggestedBy: li.querySelector('.suggested-by').textContent, cooldownTimeLeft: cooldown.cooldownTimeLeft });
                }
                else if (!cooldown && li.classList.contains('inCooldown')) {
                    updateListElement(li, { isSuggested: li.classList.contains('suggested'), isInCooldown: false, suggestedBy: li.querySelector('.suggested-by').textContent, cooldownTimeLeft: 0 });
                }
                else if (cooldown && li.classList.contains('inCooldown')) {
                    li.querySelector('.cooldown-time').textContent = secondsToHHMMSS(cooldown.cooldownTimeLeft);
                }
            });
        })
        .catch(error => {
            console.error('Error al obtener las canciones en enfriamiento:', error);
        });
    }

    function suggestItem(event) {
        const button = event.currentTarget;
        const li = button.closest('li');
        const suggestBtn = li.querySelector('.suggest-btn');
        const suggestIcon = suggestBtn.querySelector('.fa-lightbulb');
        const confirmIcon = suggestBtn.querySelector('.fa-check');

        const spinner = document.createElement('div');
        spinner.classList.add('suggest-spinner');

        if (suggestBtn.classList.contains('confirming')) {
            let songId = li.getAttribute('data-song-id');

            clearTimeout(suggestTimeout);

            suggestBtn.disabled = true;
            searchInput.disabled = true;
            document.querySelectorAll('button').forEach(btn => btn.disabled = true);

            suggestIcon.style.display = 'none';
            confirmIcon.style.display = 'none';
            suggestBtn.appendChild(spinner);

            fetch(API_BASE_URL + '/suggestion', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ spotifySongId: songId, suggestedBy: username })
            })
            .then(response => {
                if (response.status === 401) {
                    alert('La sesión ha expirado o no has iniciado sesión.');
                    window.location.href = '../index.html';
                } else if (response.status === 409 || response.status === 404) {
                    response.json().then(data => {
                        alert(data.message);
                    });
                } else if (response.ok) {
                    window.location.href = '../pages/ranking.html';
                } else {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
            })
            .catch(error => {
                spinner.remove();
                suggestIcon.style.display = 'block';
                confirmIcon.style.display = 'none';

                suggestBtn.disabled = false;
                searchInput.disabled = true;
                document.querySelectorAll('button').forEach(btn => btn.disabled = false);

                suggestBtn.classList.remove('confirming');
                suggestIcon.classList.remove('fade-out');
                confirmIcon.classList.remove('fade-in');

                console.error('Error al sugerir la canción:', error);
                alert('Error al sugerir la canción. Inténtalo de nuevo más tarde.');
            });

            return;
        }

        suggestBtn.classList.add('confirming');
        suggestIcon.classList.add('fade-out');
        confirmIcon.classList.add('fade-in');

        suggestTimeout = setTimeout(() => {
            suggestBtn.classList.remove('confirming');
            suggestIcon.classList.remove('fade-out');
            confirmIcon.classList.remove('fade-in');
        }, 2000);
    }

    function addSong(song) {
        const li = document.createElement('li');
        li.classList.toggle('suggested', song.isSuggested);
        li.classList.toggle('inCooldown', song.isInCooldown);
        li.setAttribute('data-song-id', song.id);
        li.setAttribute('data-song-name', song.name);
        li.setAttribute('data-song-artists', song.artist);
        li.setAttribute('data-song-cover-url', song.coverUrl);

        li.innerHTML = `
            <img src="${song.coverUrl}" alt="Cover" class="song-cover">
            <div class="song-details">
                <div class="song-name">${song.name}</div>
                <div class="song-artists">${song.artist}</div>
            </div>
            ${
                song.isInCooldown
                    ? inCooldownLabelHtml(song.cooldownTimeLeft)
                    : (song.isSuggested 
                        ? suggestedLabelHtml(song.suggestedBy)
                        : suggestButtonHtml)
            }
        `;

        li.classList.add('new-item');
        songList.appendChild(li);

        setTimeout(() => {
            li.classList.remove('new-item');
        }, 300);

        if (!song.isSuggested && !song.isInCooldown) {
            li.querySelector('.suggest-btn').addEventListener('click', suggestItem);
        }
    }

    searchInput.addEventListener('input', function () {
        const query = this.value.toLowerCase();

        clearTimeout(debounceTimeout);

        debounceTimeout = setTimeout(() => {
            listContainer.scrollTop = 0;
            filterSongs(query);
        }, 300);
    });

    function filterSongs(query) {
        songList.innerHTML = '';

        if (query != '')
        {
            endpoint = '/song/fetch/' + encodeURIComponent(query);
        }
        else
        {
            endpoint = '/song/most-popular';
        }

        showSpinner();

        fetch(API_BASE_URL + endpoint, {
            method: 'GET',
            headers: {
              'Authorization': `Bearer ${token}`
            }
        })
        .then(response => {
            if (response.status === 401) {
                alert('La sesión ha expirado o no has iniciado sesión.');
                hideSpinner();
                window.location.href = '../index.html';
            } else if (!response.ok) {
              throw new Error(`HTTP error! Status: ${response.status}`);
            }

            return response.json();
        })
        .then(data => {
            hideSpinner();

            data.forEach(item => {
                addSong(item);
            });
        })
        .catch(error => {
            console.error('Error al obtener los datos:', error);
            filterSongs(query);  
        });
    }

    function hideSpinner() {
        songList.style.display = 'block';
        loadingSpinner.style.display = 'none';
    }

    function showSpinner() {
        songList.style.display = 'none';
        loadingSpinner.style.display = 'block';
    }
});
