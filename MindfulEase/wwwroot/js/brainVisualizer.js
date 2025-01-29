// Asigură-te că doar un singur "let canvas" este declarat

window.onload = function () {
    // Verifică dacă canvas-ul există deja
    const canvas = document.getElementById('brainCanvas');
    if (!canvas) {
        console.error("Canvas-ul nu a fost găsit!");
        return; // Ieși din funcție dacă nu există canvas
    }

    // Setup scena, cameră și renderer
    const scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(75, canvas.clientWidth / canvas.clientHeight, 0.1, 1000);
    const renderer = new THREE.WebGLRenderer({ canvas });
    renderer.setSize(canvas.clientWidth, canvas.clientHeight);
    camera.position.z = 5;

    // Adăugăm un model de test (sferă ca "creier")
    const geometry = new THREE.SphereGeometry(1, 32, 32);
    const material = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
    const brain = new THREE.Mesh(geometry, material);
    scene.add(brain);

    // Funcție de animație
    function animate() {
        requestAnimationFrame(animate);
        brain.rotation.y += 0.01; // Rotire continuă
        renderer.render(scene, camera);
    }
    animate();
};
