document.addEventListener('DOMContentLoaded', function() {
    fetch('http://localhost:8080/suggestions')
        .then(response => response.json())
        .then(data => {
            const itemList = document.getElementById('itemList');
            data.forEach(item => {

            });
        })
        .catch((error) => {
            console.error('Error:', error);
        });
});