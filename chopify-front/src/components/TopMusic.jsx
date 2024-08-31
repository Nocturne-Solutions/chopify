import React, {useState} from 'react';

import { Button, List, ListItem, ListItemText } from '@mui/material';

const TopMusic = () => {
const [songs, setSongs] = useState([
{ id: 1, title: 'Song 1' },
{ id: 2, title: 'Song 2' },
{ id: 3, title: 'Song 3' },
]);

const handleSuggestSong = () => {
// Lógica para sugerir una canción
};

const handleVote = () => {
// Lógica para votar
};

return (
<div>
<h1>Top Music</h1>
<List>
{songs.map((song) => (
<ListItem key={song.id}>
<ListItemText primary={song.title} />
</ListItem>
))}
</List>
<Button onClick={handleSuggestSong}>Sugerir canción</Button>
<Button onClick={handleVote}>Votar</Button>
</div>
);
};

export default TopMusic;