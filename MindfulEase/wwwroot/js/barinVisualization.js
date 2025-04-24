// Import? Three.js (nu uita s? pui corect calea c?tre fi?ierul three.module.min.js)
import * as THREE from '../lib/three/build/three.module.min.js';

// Crearea scenei 3D
const scene = new THREE.Scene();

// Crearea camerei
const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);

const userId = '@User.Identity.GetUserId()'; 
// Crearea renderer-ului
const renderer = new THREE.WebGLRenderer();
renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);

// Crearea unei surse de lumin? pentru scen?
const light = new THREE.AmbientLight(0x404040); // Lumin? ambiental?
scene.add(light);

// Func?ie de anima?ie
function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
}

// Pozi?ioneaz? camera
camera.position.z = 5;

// Încarc? modelul 3D al creierului
const loader = new THREE.GLTFLoader(); // Asigur?-te c? ai inclus acest loader în fi?ierul t?u JS
loader.load('/models/Brain.glb', function (gltf) {
    scene.add(gltf.scene);
    gltf.scene.scale.set(2, 2, 2); // M?re?te/scaleaz? modelul
}, undefined, function (error) {
    console.error(error);
});

// Aplica culorile ?i scorurile pentru regiunile creierului
fetch(`/api/emotion/user/${userId}/date/${selectedDate}/brain-activity`)
    .then(response => response.json())
    .then(data => {
        data.forEach(regionData => {
            const region = scene.getObjectByName(regionData.BrainRegion); // G?se?te regiunile 3D
            if (region) {
                region.material.color.set(regionData.Color); // Seteaz? culoarea
                region.material.opacity = regionData.Score; // Seteaz? opacitatea (scorul)
            }
        });
    });

// Începe anima?ia
animate();
