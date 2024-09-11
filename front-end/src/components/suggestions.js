document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchInput');
    const songList = document.getElementById('songList');
    const listContainer = document.querySelector('.list-container');
    const token = localStorage.getItem('sessionToken');

    let suggestTimeout;
    let debounceTimeout;

    function suggestItem(event) {
        const button = event.currentTarget;
        const li = button.closest('li');
        const suggestBtn = li.querySelector('.suggest-btn');
        const suggestIcon = suggestBtn.querySelector('.fa-lightbulb');
        const confirmIcon = suggestBtn.querySelector('.fa-check');

        if (suggestBtn.classList.contains('confirming')) {
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

    function addSong(name, artists, coverUrl, isSuggested = false, suggestedBy = '') {
        const li = document.createElement('li');
        li.classList.toggle('suggested', isSuggested);

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
            endpoint = '/music/fetch/' + encodeURIComponent(query);
        }
        else
        {
            endpoint = '/music/most-popular';
        }

        fetch(API_BASE_URL + endpoint, {
            method: 'GET',
            headers: {
              'Authorization': `Bearer ${token}`
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            return response.json();
        })
        .then(data => {
            data.forEach(item => {
                addSong(item.name, item.artist, item.coverUrl, false, '');
            });
        })
        .catch(error => alert('Error obteniendo datos: ' + error.message));
    }

    checkSession();
    
    filterSongs('');
});
