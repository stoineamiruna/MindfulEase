import * as THREE from 'three';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';

console.log(emotionsData);

// Setare scenă și cameră
const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
camera.position.set(17, 17.6, 0.5);

const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);

const controls = new OrbitControls(camera, renderer.domElement);
controls.target.set(16.5, 17.5, 0.3);
controls.update();

const light = new THREE.AmbientLight(0xffffff, 1);
scene.add(light);

let brain = null;

// Așteaptă creierul încărcat din brainVisualization
function waitForBrainModel(callback) {
    const check = () => {
        if (window.brainModel) {
            callback(window.brainModel);
            console.log("E aici!");
        } else {
            setTimeout(check, 100);
        }
    };
    check();
}

function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
}
animate();

// ------------------ EMOTION LOGIC -----------------------

function initEmotionLogic() {
    const emotionDatePicker = document.getElementById("emotionDatePicker");

    const emotionToColor = {
        joy: "#F6BE00",
        sadness: "blue",
        anger: "red",
        love: "pink",
        fear: "magenta",
        surprise: "orange",
        disgust: "green"
    };

    const emotionMap = {
        joy: ["VPC-L", "VPC-R", "NA-L", "NA-R"],
        sadness: ["Amygdala", "ACC-L", "ACC-R", "Insula-L", "Insula-R"],
        anger: ["Amygdala", "HypoThalamus-L", "HypoThalamus-R", "DPC-L", "DPC-R"],
        love: ["Insula-L", "Insula-R", "OFC-L", "OFC-R", "Striatum"],
        fear: ["Amygdala", "OFC-L", "OFC-R", "Hippocampus-L", "Hippocampus-R"],
        surprise: ["DPC-L", "DPC-R", "SPC-L", "SPC-R"],
        disgust: ["Insula-L", "Insula-R", "BasalGanglia-L", "BasalGanglia-R", "OFC-L", "OFC-R"]
    };

    if (typeof emotionsData === 'undefined') {
        console.error("Nu există emotionsData în context.");
        return;
    }

    const allAvailableDates = [...new Set(emotionsData.map(e => e.Date))];
    const currentDate = allAvailableDates[0];
    if (emotionDatePicker) emotionDatePicker.value = currentDate;
    console.log(allAvailableDates);
    function scaleIntensity(score) {
        return Math.min(1, Math.max(0.2, score / 10));
    }

    function applyEmotionHighlightForDate(date) {
        if (!brain) return;

        const selectedDayEmotions = emotionsData.filter(e => {
            const emotionDate = typeof e.Date === 'string' ? e.Date.slice(0, 10) : new Date(e.Date).toISOString().slice(0, 10);
            return emotionDate === date;
        });

        console.log(selectedDayEmotions);

        brain.traverse(obj => {
            if (obj.isMesh) {
                if (!obj.userData.defaultMaterial) {
                    obj.userData.defaultMaterial = obj.material.clone();
                }
                obj.material = obj.userData.defaultMaterial.clone();
                obj.material.color.set("#cccccc");
                obj.material.opacity = 0.1;
                obj.material.transparent = true;
            }
        });

        const body = brain.getObjectByName("Line03");
        if (body && body.isMesh) {
            body.material = new THREE.MeshStandardMaterial({
                color: new THREE.Color("rgba(0,123,255,1)"),
                transparent: true,
                opacity: 0.2,
                depthWrite: false,
                depthTest: true,
                roughness: 0.1,
                metalness: 0.0
            });
            body.renderOrder = 0;
        } else {
            console.warn("Nu s-a găsit corpul uman (Line03) în model.");
        }

        selectedDayEmotions.forEach(emotionData => {
            const emotion = emotionData.EmotionLabel;
            const score = emotionData.AverageMoodValue;

            const regions = emotionMap[emotion] || [];
            const color = emotionToColor[emotion] || "#ff00ff";
            const opacity = scaleIntensity(score);

            regions.forEach(regionName => {
                const region = brain.getObjectByName(regionName);
                if (region) {
                    region.traverse(child => {
                        if (child.isMesh) {
                            if (!child.userData.defaultMaterial) {
                                child.userData.defaultMaterial = child.material.clone();
                            }
                            child.material = child.userData.defaultMaterial.clone();
                            child.material.color.set(color);
                            child.material.opacity = opacity;
                            child.material.transparent = true;
                            child.renderOrder = 1;
                        }
                    });
                } else {
                    console.warn(`Region "${regionName}" not found.`);
                }
            });
        });
    }

    if (emotionDatePicker) {
        emotionDatePicker.addEventListener("change", function () {
            const selectedDate = this.value;
            applyEmotionHighlightForDate(selectedDate);
        });

        applyEmotionHighlightForDate(currentDate);
    }
}

// 🔁 Așteptăm să fie disponibil brainModel
waitForBrainModel(function (loadedBrain) {
    brain = loadedBrain;
    scene.add(brain); // doar dacă nu e deja adăugat în altă parte
    if (!brain) {
        console.log("Creierul e aici!");
    }
    initEmotionLogic();
});
