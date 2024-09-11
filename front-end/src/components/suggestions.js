document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchInput');
    const songList = document.getElementById('songList');
    const listContainer = document.querySelector('.list-container');
    const loadingSpinner = document.getElementById('loadingSpinner');
    const token = localStorage.getItem('sessionToken');
    const username = localStorage.getItem('sessionUser');

    let debounceTimeout;

    function updateSuggestions(li, suggestedBy)
    {
        let name = li.getAttribute('data-song-name');
        let artists = li.getAttribute('data-song-artists');
        let coverUrl = li.getAttribute('data-song-cover-url');

        li.classList.add('suggested');

        li.innerHTML = `
            <img src="${coverUrl}" alt="Cover" class="song-cover">
            <div class="song-details">
                <div class="song-name">${name}</div>
                <div class="song-artists">${artists}</div>
            </div>
            <div class="suggested-label">Ya fue sugerida por <span class="sugest-by">${suggestedBy}</span></div>`;
    }

    function checkSuggestions()
    {
        fetch(API_BASE_URL + '/song/suggestions', {
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
            songList.items.forEach(item => {
                if (data.some(suggestion => suggestion.songId === item.id)) {
                    let suggestedBy = data.find(suggestion => suggestion.songId === item.id).suggestedBy;
                    updateSuggestions(item, suggestedBy);
                }
            });
        })
        .catch(error => {
            console.error('Error al obtener las sugerencias:', error);
        });
    }

    function suggestItem(event) {
        const button = event.currentTarget;
        const li = button.closest('li');
        const suggestBtn = li.querySelector('.suggest-btn');
        const suggestIcon = suggestBtn.querySelector('.fa-lightbulb');
        const confirmIcon = suggestBtn.querySelector('.fa-check');

        if (suggestBtn.classList.contains('confirming')) {
            let songId = li.getAttribute('data-song-id');

            fetch(API_BASE_URL + '/suggestion/suggest', {
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
                } else if (response.status === 409) {
                    alert('Otro usuario ya ha sugerido esta canción.');
                } else if (response.ok) {
                    window.location.href = '../pages/ranking.html';
                } else {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
            })
            .catch(error => {
                console.error('Error al sugerir la canción:', error);
                alert('Error al sugerir la canción. Inténtalo de nuevo más tarde.');
            });
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

    function addSong(id, name, artists, coverUrl, isSuggested = false, suggestedBy = '') {
        const li = document.createElement('li');
        li.classList.toggle('suggested', isSuggested);
        li.setAttribute('data-song-id', id);
        li.setAttribute('data-song-name', name);
        li.setAttribute('data-song-artists', artists);
        li.setAttribute('data-song-cover-url', coverUrl);

        li.innerHTML = `
            <img src="${coverUrl}" alt="Cover" class="song-cover">
            <div class="song-details">
                <div class="song-name">${name}</div>
                <div class="song-artists">${artists}</div>
            </div>
            ${
                isSuggested
                    ? `<div class="suggested-label">Ya fue sugerida por <span class="sugest-by">${suggestedBy}</span></div>`
                    : `
                <button class="suggest-btn">
                    <i class="fas fa-lightbulb"></i>
                    <i class="fas fa-check"></i>
                </button>
            `}
        `;

        li.classList.add('new-item');
        songList.appendChild(li);

        setTimeout(() => {
            li.classList.remove('new-item');
        }, 300);

        if (!isSuggested) {
            const suggestButton = li.querySelector('.suggest-btn');
            suggestButton.addEventListener('click', suggestItem);
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
                resetButtonAndSpinner();
                window.location.href = '../index.html';
            } else if (!response.ok) {
              throw new Error(`HTTP error! Status: ${response.status}`);
            }

            return response.json();
        })
        .then(data => {
            resetButtonAndSpinner();

            data.forEach(item => {
                addSong(item.id, item.name, item.artist, item.coverUrl, item.isSuggested, item.suggestedBy);
            });
        })
        .catch(error => {
            console.error('Error al obtener los datos:', error);
            filterSongs(query);  
        });
    }

    function resetButtonAndSpinner() {
        songList.style.display = 'block';
        loadingSpinner.style.display = 'none';
    }

    function showSpinner() {
        songList.style.display = 'none';
        loadingSpinner.style.display = 'block';
    }

    checkSession();

    //setInterval(checkSuggestions, 2000);
    
    filterSongs('');
});
