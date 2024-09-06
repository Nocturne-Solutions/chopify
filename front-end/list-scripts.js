document.addEventListener('DOMContentLoaded', function() {
    // Hacer una solicitud GET para obtener la lista de elementos
    fetch('https://localhost:7251/user') // Reemplaza con tu URL de endpoint para obtener la lista
        .then(response => response.json())
        .then(data => {
            const itemList = document.getElementById('itemList');
            data.forEach(item => {
                const listItem = document.createElement('li');
                listItem.className = 'list-group-item';
                listItem.innerHTML = `
                    <strong>${item.name}</strong> - ${item.tag}
                    <button class="btn vote-button" onclick="handleVote(this)"><i class="fas fa-thumbs-up"></i></button>
                    <div class="vote-message">Voto tomado</div>
                    <i class="fas fa-check check-icon"></i>
                `;
                itemList.appendChild(listItem);
            });
        })
        .catch((error) => {
            console.error('Error:', error);
        });
});

function handleVote(button) {
    const listItem = button.closest('.list-group-item');

    // Ocultar todos los botones de votar
    const voteButtons = document.querySelectorAll('.vote-button');
    voteButtons.forEach(btn => btn.classList.add('hidden'));

    // Mostrar mensaje de "Voto tomado"
    const voteMessage = button.nextElementSibling;
    voteMessage.style.display = 'block'; // Asegura que sea visible para la animación
    voteMessage.classList.add('show');

    // Ocultar el mensaje después de 3 segundos con animación de salida
    setTimeout(() => {
        voteMessage.classList.remove('show');
        voteMessage.classList.add('hide');

        // Esperar a que la transición termine antes de mostrar el icono de tilde
        voteMessage.addEventListener('transitionend', () => {
            voteMessage.style.display = 'none';
            voteMessage.classList.remove('hide');

            // Mostrar y animar el ícono de tilde verde al lado del ítem votado
            const checkIcon = listItem.querySelector('.check-icon');
            listItem.classList.add('checked');
            checkIcon.style.display = 'inline';

            // Animación con GSAP: Aparición, movimiento y partículas
            gsap.fromTo(checkIcon, 
                { opacity: 0, scale: 0.5, y: -20 }, // Estado inicial: pequeño y desplazado
                { 
                    opacity: 1, scale: 1, y: 0, duration: 0.6, ease: 'elastic.out(1, 0.5)', 
                    onComplete: () => {
                        // Añadir partículas usando GSAP
                        gsap.to(checkIcon, { 
                            duration: 1.5, 
                            particles: {
                                count: 50,
                                radius: { min: 5, max: 15 },
                                speed: { min: 10, max: 20 },
                                direction: "random",
                                color: "#28a745"
                            }
                        });
                    }
                }
            );
        }, { once: true });
    }, 3000);
}
