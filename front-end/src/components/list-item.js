document.addEventListener('DOMContentLoaded', function () {
    const songList = document.getElementById('songList');

    let totalVotes = 0;
    let pendingItems = [];
    let voteTimeout;
    let selectedItem = null;

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

        items.sort((a, b) => {
            const votesA = parseInt(a.dataset.votes);
            const votesB = parseInt(b.dataset.votes);
            return votesB - votesA;
        });

        items.forEach(item => songList.appendChild(item));
        animateReorder();

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

        if (voteBtn.classList.contains('confirming')) {
            return;
        }

        // Cambiar el estado del botón a confirmar
        voteBtn.classList.add('confirming');
        voteIcon.classList.add('fade-out');
        confirmIcon.classList.add('fade-in');

        //clearTimeout(voteTimeout);

        voteTimeout = setTimeout(() => {
            voteBtn.classList.remove('confirming');
            voteIcon.classList.remove('fade-out');
            confirmIcon.classList.remove('fade-in');
        }, 2000);

        confirmIcon.addEventListener('click', function () {
            if (voteBtn.classList.contains('confirming')) {
                const currentVotes = parseInt(li.dataset.votes);
                li.dataset.votes = currentVotes + 1;
                totalVotes++;
                updateList();

                li.classList.add('song-voted');
                songList.querySelectorAll('.vote-btn').forEach(btn => btn.classList.add('hidden'));

                clearTimeout(voteTimeout);
                voteBtn.classList.remove('confirming');
                voteIcon.classList.remove('fade-out');
                confirmIcon.classList.remove('fade-in');
                selectedItem = li; // Marcar el ítem como seleccionado
            }
        });
    }

    function addSong(yourName, name, artists, coverUrl) {
        const li = document.createElement('li');
        li.dataset.votes = 0;
        li.innerHTML = `
            <div class="suggested-by">Sugerido por ${yourName}</div>
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

        pendingItems.push(li);
        
        updateList();

        const voteButton = li.querySelector('.vote-btn');
        voteButton.addEventListener('click', voteForItem);
    }

    // Testing songs for visualization.
    addSong("Juan Perez", "Echoes of Dawn", "The Nightingales", "https://images.unsplash.com/photo-1519681393784-d120267933ba");
    addSong("Maria Lopez", "Silent Whispers", "The Dreamers", "https://images.unsplash.com/photo-1531746790731-6c087fecd65a");
    addSong("Carlos Gomez", "Rising Sun", "Sunset Riders - Luna Bloom", "https://images.unsplash.com/photo-1470115636492-6d2b56c8f5b2");
    addSong("Ana Torres", "Midnight Drive", "Neon Sky", "https://images.unsplash.com/photo-1499346030926-9a72daac6c63");
    addSong("Diego Silva", "Ocean Breeze", "Wavecatchers", "https://images.unsplash.com/photo-1507525428034-b723cf961d3e");
    addSong("Laura Martinez", "Starlit Night", "Celestial Voices - Astro Harmony", "https://images.unsplash.com/photo-1506748686214-e9df14d4d9d0");
    addSong("Sofia Diaz", "Golden Horizon", "Aurora Sound - The Lightseekers", "https://images.unsplash.com/photo-1519125323398-675f0ddb6308");
    addSong("Pedro Sanchez", "Broken Chains", "The Renegades", "https://images.unsplash.com/photo-1539786951206-324e9f16a1d7");
    addSong("Lucia Fernandez", "Into the Wild", "Wilderness - Echo Beats", "https://images.unsplash.com/photo-1516877757170-0f2ee144eac9");
    addSong("Jorge Ruiz", "Last Serenade", "Melody Makers", "https://images.unsplash.com/photo-1522038894502-23a0c2ee4f6d");    
});
