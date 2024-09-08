document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.getElementById('searchInput');
    const songList = document.getElementById('songList');
    const listContainer = document.querySelector('.list-container');

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
        songs
            .filter(
                (song) =>
                    song.name.toLowerCase().includes(query) ||
                    song.artists.toLowerCase().includes(query)
            )
            .forEach((song) => {
                addSong(song.name, song.artists, song.coverUrl, song.isSuggested, song.suggestedBy);
            });
    }

    const songs = [
        {
            name: 'Echoes of Dawn',
            artists: 'The Nightingales',
            coverUrl: 'https://images.unsplash.com/photo-1519681393784-d120267933ba',
            isSuggested: true,
            suggestedBy: 'Ramiro#87av5f3c',
        },
        {
            name: 'Silent Whispers',
            artists: 'The Dreamers',
            coverUrl: 'https://images.unsplash.com/photo-1531746790731-6c087fecd65a',
            isSuggested: true,
            suggestedBy: 'Lucas#65a2bc3d',
        },
        {
            name: 'Rising Sun',
            artists: 'Sunset Riders - Luna Bloom',
            coverUrl: 'https://images.unsplash.com/photo-1470115636492-6d2b56c8f5b2',
            isSuggested: false,
            suggestedBy: '',
        },
        {
            name: 'Midnight Drive',
            artists: 'Neon Sky',
            coverUrl: 'https://images.unsplash.com/photo-1499346030926-9a72daac6c63',
            isSuggested: false,
            suggestedBy: '',
        },
        {
            name: 'Ocean Breeze',
            artists: 'Wavecatchers',
            coverUrl: 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e',
            isSuggested: false,
            suggestedBy: '',
        },
        {
            name: 'Starlit Night',
            artists: 'Celestial Voices - Astro Harmony',
            coverUrl: 'https://images.unsplash.com/photo-1506748686214-e9df14d4d9d0',
            isSuggested: true,
            suggestedBy: 'Lucia#1ab15af7',
        },
        {
            name: 'Golden Horizon',
            artists: 'Aurora Sound - The Lightseekers',
            coverUrl: 'https://images.unsplash.com/photo-1519125323398-675f0ddb6308',
            isSuggested: false,
            suggestedBy: '',
        },
        {
            name: 'Broken Chains',
            artists: 'The Renegades',
            coverUrl: 'https://images.unsplash.com/photo-1539786951206-324e9f16a1d7',
            isSuggested: false,
            suggestedBy: '',
        },
        {
            name: 'Into the Wild',
            artists: 'Wilderness - Echo Beats',
            coverUrl: 'https://images.unsplash.com/photo-1516877757170-0f2ee144eac9',
            isSuggested: false,
            suggestedBy: '',
        },
        {
            name: 'Last Serenade',
            artists: 'Melody Makers',
            coverUrl: 'https://images.unsplash.com/photo-1522038894502-23a0c2ee4f6d',
            isSuggested: false,
            suggestedBy: '',
        },
    ];

    filterSongs('');
});
