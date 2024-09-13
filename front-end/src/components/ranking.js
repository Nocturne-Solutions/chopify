document.addEventListener('DOMContentLoaded', function () {
    const songList = document.getElementById('songList');
    const suggestButton = document.getElementById('suggestBtn');
    const username = localStorage.getItem('sessionUser');
	const loadingSpinner = document.getElementById('loadingSpinner');
    const token = localStorage.getItem('sessionToken');

    let totalVotes = 0;
    let pendingItems = [];
    let voteTimeout;
    let selectedItem = null;
    let voteSongId = null;

    init();

    async function init() {
        showSpinner();
        await checkSession();
        await GetVoteSongId();
        refreshRanking();

        setInterval(async function() {
            await GetVoteSongId();
            refreshRanking();
        }, 5000);
    }

    suggestButton.addEventListener('click', function() {
        window.location.href = 'suggestions.html';
    });

    document.querySelector('.user-label').textContent = username;

    suggestButton.classList.add('hidden');

    function capturePositions() {
        const items = Array.from(songList.children);
        items.forEach(item => {
            const rect = item.getBoundingClientRect();
            item.dataset.top = rect.top;
        });
    }

    function animateReorder() {
        const items = Array.from(songList.children);
        items.forEach(item => {
            const oldTop = parseFloat(item.dataset.top);
            const newTop = item.getBoundingClientRect().top;
            const deltaY = oldTop - newTop;
            item.style.transition = 'none';
            item.style.transform = `translateY(${deltaY}px)`;
            requestAnimationFrame(() => {
                item.style.transition = 'transform 0.5s ease';
                item.style.transform = '';
            });
        });
    }

    function updateList() {
        while (pendingItems.length > 0) {
            const item = pendingItems.shift();
            item.classList.add('new-item');
            songList.appendChild(item);
            setTimeout(() => {
                item.classList.remove('new-item');
            }, 500);
        }

        const items = Array.from(songList.children);
        capturePositions();
    
        const sortedItems = [...items].sort((a, b) => parseInt(b.dataset.votes) - parseInt(a.dataset.votes));
        let needsReorder = false;
    
        for (let i = 0; i < items.length; i++) {
            if (items[i] !== sortedItems[i]) {
                needsReorder = true;
                break;
            }
        }
    
        if (needsReorder) {
            sortedItems.forEach(item => songList.appendChild(item));
            animateReorder();
        }
    
        items.forEach(item => {
            const votes = parseInt(item.dataset.votes);
            const percentage = totalVotes > 0 ? (votes / totalVotes) * 100 : 0;
            const voteProgress = item.querySelector('.vote-progress');
            voteProgress.style.width = `${percentage}%`;
            voteProgress.innerHTML = `<span>${votes} / ${totalVotes}</span>`;
        });
    }

    function voteForItem(event) {
        const button = event.currentTarget;
        const li = button.closest('li');
        const voteBtn = li.querySelector('.vote-btn');
        const voteIcon = voteBtn.querySelector('.fa-thumbs-up');
        const confirmIcon = voteBtn.querySelector('.fa-check');

        const spinner = document.createElement('div');
        spinner.classList.add('vote-spinner');

        if (voteBtn.classList.contains('confirming')) {
            let songId = li.getAttribute('data-song-id');

            clearTimeout(voteTimeout);
            
            voteBtn.disabled = true;
            document.querySelectorAll('button').forEach(btn => btn.disabled = true);

            voteIcon.style.display = 'none';
            confirmIcon.style.display = 'none';
            voteBtn.appendChild(spinner);

            fetch(API_BASE_URL + '/vote', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ id: songId, user: username })
            })
            .then(response => {
                spinner.remove();
                voteIcon.style.display = 'block';
                confirmIcon.style.display = 'none';

                voteBtn.disabled = false;
                document.querySelectorAll('button').forEach(btn => btn.disabled = false);

                voteBtn.classList.remove('confirming');
                voteIcon.classList.remove('fade-out');
                confirmIcon.classList.remove('fade-in');

                if (response.status === 401) {
                    alert('La sesión ha expirado o no has iniciado sesión.');
                    window.location.href = '../index.html';
                } else if (response.status === 409) {
                    response.json().then(data => {
                        alert(data.message);
                    });
                } else if (response.ok) {
                    voteSongId = songId;
                    selectedItem = li;
                    li.classList.add('song-voted');
                    li.dataset.votes = parseInt(li.dataset.votes) + 1;
                    totalVotes++;
                    hideVoteButtons();
                    hideSuggestButton();
                    updateList();
                } else {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
            })
            .catch(error => {
                spinner.remove();
                voteIcon.style.display = 'block';
                confirmIcon.style.display = 'none';

                voteBtn.disabled = false;
                document.querySelectorAll('button').forEach(btn => btn.disabled = false);

                voteBtn.classList.remove('confirming');
                voteIcon.classList.remove('fade-out');
                confirmIcon.classList.remove('fade-in');

                console.error('Error al sugerir la canción:', error);
                alert('Error al sugerir la canción. Inténtalo de nuevo más tarde.');
            });

            return;
        }

        voteBtn.classList.add('confirming');
        voteIcon.classList.add('fade-out');
        confirmIcon.classList.add('fade-in');

        voteTimeout = setTimeout(() => {
            voteBtn.classList.remove('confirming');
            voteIcon.classList.remove('fade-out');
            confirmIcon.classList.remove('fade-in');
        }, 2000);
    }

    function addSong(id, name, artists, coverUrl, suggestBy, votes) {
        const li = document.createElement('li');

        li.setAttribute('data-song-id', id);
        li.setAttribute('data-song-name', name);
        li.setAttribute('data-song-artists', artists);
        li.setAttribute('data-song-cover-url', coverUrl);

        if (id === voteSongId) {
            li.classList.add('song-voted');
            selectedItem = li;
        }
        
        li.dataset.votes = votes;
        li.innerHTML = `
            <div class="suggested-by">Sugerido por ${suggestBy}</div>
            <img src="${coverUrl}" alt="Cover" class="song-cover">
            <div class="song-details">
                <div class="song-name">${name}</div>
                <div class="song-artists">${artists}</div>
                <div class="vote-section">
                    <div class="vote-bar">
                        <div class="vote-progress"><span>0 / ${totalVotes}</span></div>
                    </div>
                </div>
            </div>
            <button class="vote-btn ${selectedItem ? 'hidden-vote-btn' : ''}">
                <i class="fas fa-thumbs-up"></i>
                <i class="fas fa-check"></i>
            </button>
        `;

        if (voteSongId === null) {
            li.querySelector('.vote-btn').classList.add('show');
        }

        pendingItems.push(li);

        const voteButton = li.querySelector('.vote-btn');
        voteButton.addEventListener('click', voteForItem);
    }

    function refreshRanking()
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
            totalVotes = 0;

            songList.querySelectorAll('li').forEach(li => {
                let songId = li.getAttribute('data-song-id');
                let suggestionIndex = data.findIndex(suggestion => suggestion.id === songId);

                if (suggestionIndex !== -1) {
                    let suggestion = data[suggestionIndex];
                    
                    data.splice(suggestionIndex, 1);
            
                    li.dataset.votes = suggestion.votes;
                    totalVotes += suggestion.votes;

                    if (suggestion.id === voteSongId) {
                        li.classList.add('song-voted');
                        selectedItem = li;
                    }
                    else {
                        li.classList.remove('song-voted');
                    }
                }
                else {
                    li.classList.add('deleted-item');
                    setTimeout(() => {
                        li.classList.remove('deleted-item');
                        li.remove();
                    }, 500);
                }
            });

            data.forEach(suggestion => {
                addSong(suggestion.id, suggestion.name, suggestion.artist, suggestion.coverUrl, suggestion.suggestedBy, suggestion.votes);
                totalVotes += suggestion.votes;
            });
            
            hideSpinner();

            updateList();
        })
        .catch(error => {
            console.error('Error al obtener las sugerencias:', error.message);
            showSpinner();
        });
    }

    async function GetVoteSongId()
    {
        const response = await fetch(API_BASE_URL + '/vote/' + encodeURIComponent(username), {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            },
        });

        if (response.status === 401) {
            alert('La sesión ha expirado o no has iniciado sesión.');
            window.location.href = '../index.html';
        } 
        else if (response.status === 404) {
            voteSongId = null;
            showVoteButtons();
            showSuggestButton();
        }
        else if (response.ok) {
            const data = await response.json();
            voteSongId = data.id;
            hideVoteButtons();
            hideSuggestButton();
        }
        else {
            console.error('Error al obtener las sugerencias:', error.message);
            await GetVoteSongId();
        }
    }

    function hideSpinner() {
        songList.style.display = 'block';
        loadingSpinner.style.display = 'none';
    }

    function showSpinner() {
        songList.style.display = 'none';
        loadingSpinner.style.display = 'block';
    }

    function showSuggestButton() {
        suggestButton.classList.remove('hidden');
        suggestButton.classList.add('show');
    }

    function hideSuggestButton() {
        suggestButton.classList.add('hidden');
        suggestButton.classList.remove('show');
    }

    function showVoteButtons() {
        songList.querySelectorAll('.vote-btn').forEach(btn => btn.classList.remove('hidden'));
        songList.querySelectorAll('.vote-btn').forEach(btn => btn.classList.add('show'));
    }

    function hideVoteButtons() {
        songList.querySelectorAll('.vote-btn').forEach(btn => btn.classList.add('hidden'));
        songList.querySelectorAll('.vote-btn').forEach(btn => btn.classList.remove('show'));
    }
});
