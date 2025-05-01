import * as THREE from 'three';
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';

const canvas = document.getElementById('brainCanvas');
const scene = new THREE.Scene();
const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
const renderer = new THREE.WebGLRenderer({ canvas, alpha: true, antialias: true });
//renderer.setSize(window.innerWidth, window.innerHeight);
//document.body.appendChild(renderer.domElement);
function resizeRendererToDisplaySize() {
    const width = canvas.clientWidth;
    const height = canvas.clientHeight;
    if (canvas.width !== width || canvas.height !== height) {
        renderer.setSize(width, height, false);
        camera.aspect = width / height;
        camera.updateProjectionMatrix();
    }
}


const controls = new OrbitControls(camera, renderer.domElement);
controls.enableDamping = true;

scene.background = null;

const ambientLight = new THREE.AmbientLight(0xffffff, 0.3);
scene.add(ambientLight);

const light = new THREE.DirectionalLight(0xffffff, 0.6); // ajustat
light.position.set(10, 10, 10);
scene.add(light);


export let brain = null;
const loader = new GLTFLoader();
loader.load('/Brain.glb', function (gltf) {
    brain = gltf.scene;
    brain.scale.set(1.5, 1.5, 1.5);
    scene.add(brain);

    camera.position.set(17.046385270528695, 17.655871704708414, 0.500132956272668);
    controls.target.set(16.505236909331614, 17.56862497738409, 0.32105271621538645);
    controls.update();


    const body = brain.getObjectByName("Line03");
    if (body) {
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
    }
    else {
        console.warn("Nu s-a găsit corpul uman (Line03) în model.");
    }

    ["Left-Brain", "Right-Brain"].forEach(name => {
        const group = brain.getObjectByName(name);
        if (group) {
            group.traverse(child => {
                if (child.isMesh) {
                    // Aplică materialul pentru obiectele de tip Mesh
                    child.material = new THREE.MeshStandardMaterial({
                        color: 0xffd1df, // Culoare default
                        transparent: true,
                        opacity: 0.4,
                        depthWrite: true,
                        depthTest: true
                    });
                    child.renderOrder = 1;
                }
                // Dacă ai și alte obiecte de tip Group sau alte tipuri de obiecte, poți adăuga mai multe condiții
                else if (child.isGroup) {
                    // Dacă vrei să aplice culoarea și pentru obiectele din grupuri
                    child.traverse(grandChild => {
                        if (grandChild.isMesh) {
                            grandChild.material = new THREE.MeshStandardMaterial({
                                color: 0xffd1df, // Culoare default
                                transparent: true,
                                opacity: 0.4,
                                depthWrite: true,
                                depthTest: true
                            });
                            grandChild.renderOrder = 1;
                        }
                    });
                }
            });
        } else {
            console.warn(`${name} not found`);
        }
    });



    const simulatedData = [
        { BrainRegion: ["Amygdala"], Color: "#FF0000", Score: 0.9 },
        { BrainRegion: ["Brainstem"], Color: "#e3dac9", Score: 0.6 },
        { BrainRegion: ["Insula-L", "Insula-R"], Color: "#FF00FF", Score: 0.6 },
        { BrainRegion: ["Hippocampus-R"], Color: "#0000FF", Score: 0.7 },
        { BrainRegion: ["OFC-L", "OFC-R"], Color: "#FFA500", Score: 0.65 },
        { BrainRegion: ["DPC-L", "DPC-R"], Color: "#00FFFF", Score: 0.55 },
        { BrainRegion: ["VPC-L", "VPC-R"], Color: "#1ABC9C", Score: 0.5 },
        { BrainRegion: ["SPC-L", "SPC-R"], Color: "#4CAF50", Score: 0.65 },
        { BrainRegion: ["Thalamus-L", "Thalamus-R"], Color: "#9932CC", Score: 0.8 },
        { BrainRegion: ["NA-L", "NA-R"], Color: "#8B0000", Score: 0.75 },
        { BrainRegion: ["BasalGanglia-L", "BasalGanglia-R"], Color: "#8E44AD", Score: 0.6 },
        { BrainRegion: ["HypoThalamus-L", "HypoThalamus-R"], Color: "#FF69B4", Score: 0.7 },
        { BrainRegion: ["ACC-L", "ACC-R"], Color: "#4682B4", Score: 0.68 },
        { BrainRegion: ["Cerebellum-L", "Cerebellum-R"], Color: "#FF69B4", Score: 0.68 }
    ];

    const regionNameMap = {
        "VPC": "Ventromedial Prefrontal Cortex (VPC)",
        "NA": "Nucleus Accumbens (NA)",
        "Amygdala": "Amygdala",
        "ACC": "Anterior Cingulate Cortex (ACC)",
        "Insula": "Insula",
        "DPC": "Dorsolateral Prefrontal Cortex (DPC)",
        "OFC": "Orbitofrontal Cortex (OFC)",
        "Hippocampus": "Hippocampus",
        "BasalGanglia": "Basal Ganglia",
        "Striatum": "Striatum",
        "HypoThalamus": "Hypothalamus",
        "Cerebellum": "Cerebellum",
        "SPC": "Superior Parietal Cortex (SPC)",
    };

    function getFullRegionName(region) {
        // Extragem regiunea principală (fără L/R)
        const regionName = region.replace(/-L$|-R$/, '');

        // Dacă există o denumire completă în map, o returnăm, altfel returnăm regiunea originală
        return regionNameMap[regionName] || region;
    }


    simulatedData.forEach(regionData => {
        regionData.BrainRegion.forEach(name => {
            const regionGroup = brain.getObjectByName(name);  // Căutăm grupul

            console.log(`Verificare grup: ${name}`, regionGroup);

            if (regionGroup) {
                // Parcurge toate obiectele din grup
                regionGroup.traverse(child => {
                    if (child.isMesh) {
                        // Aplică materialul pe fiecare mesh din grup
                        child.material = new THREE.MeshStandardMaterial({
                            color: regionData.Color,
                            transparent: true,
                            opacity: regionData.Score, // Opacitatea bazată pe scor
                            depthWrite: true,
                            depthTest: true
                        });
                        child.renderOrder = 1; // Asigură-te că este redat după restul obiectelor
                    }
                });
            } else {
                console.warn(`Grupul nu a fost găsit: ${name}`); // Afișează un mesaj dacă grupul nu este găsit
            }
        });
    });

    // Logică pentru colorarea regiunilor pe baza emoțiilor
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

    console.log(emotionsData);
    const emotionDatePicker = document.getElementById("emotionDatePicker");
    const predictSlider = document.getElementById("predictSlider");
    const predictYears = document.getElementById("predictYears");
    const viewBrainDamageButton = document.getElementById("viewBrainDamageButton");
    const riskList = document.getElementById("riskList");
    const balancedList = document.getElementById("balancedList");
    const alertMessage = document.getElementById("alertMessage");

    // Setăm valoarea implicită pentru predicție
    predictYears.textContent = predictSlider.value + " years";

    predictSlider.addEventListener("input", function () {
        predictYears.textContent = this.value + " years";
        const years = parseInt(predictSlider.value);
        console.log("Predict button clicked for " + years + " years");

        const selectedDateValue = emotionDatePicker.value;
        if (selectedDateValue) {
            const selectedDate = new Date(selectedDateValue);
            console.log("Trigger fetchPrediction cu:", years, selectedDate);
            fetchPrediction(years, selectedDate);
        } else {
            console.warn("Nu a fost selectată o dată.");
            alert("Please select a date first");
        }
    });


    // Funcția care trimite cererea de predicție
    async function fetchPrediction(yearsSinceStart, selectedDate) {
        if (!selectedDate || isNaN(yearsSinceStart)) {
            alert("Selectează o dată validă și o valoare pentru ani.");
            return;
        }

        const body = {
            date: selectedDate.toISOString(),
            yearsSinceStart: yearsSinceStart
        };

        try {
            const response = await fetch("/Visualize/PredictBrainDamage", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(body)
            });

            if (!response.ok) throw new Error("Request failed");
            const prediction = await response.json();

            console.log("Rezultatul predicției:", prediction);

            updateBrainPredicted(prediction);


        } catch (error) {
            console.error("Eroare la predicție:", error);
            alert("A apărut o problemă la generarea predicției.");
        }
    }

   


    // Emoțiile negative și regiunile aferente pentru daunele cerebrale
    const negativeEmotions = ['anger', 'fear', 'sadness', 'disgust'];
    const damageColors = {
        high: "#FF0000", // Roșu pentru daune mari
        medium: "#FF8C00", // Portocaliu pentru daune medii
        low: "#FFD700" // Galben pentru daune mici
    };


    // Funcție pentru a evidenția regiunile în funcție de emoție
    function highlightEmotion(emotion) {
        if (!brain) return;

        // Resetează toate regiunile la opacitate slabă
        resetBrain();
        const emotionDescription = document.getElementById("emotionDescription");
        if (emotion === 'global') {
            emotionDescription.innerHTML = `
        <p>
            Each highlighted region corresponds to neural activity related to that emotion. You can hover or click on brain areas for additional info.
        </p>
        <ul>
            <li>Click an emoji to view related brain regions.</li>
            <li>Hover over a brain region to discover its role.</li>
            <li>Observe how emotions interact with brain structure.</li>
        </ul>`;

            ["Left-Brain", "Right-Brain"].forEach(name => {
                const group = brain.getObjectByName(name);
                if (group) {
                    group.traverse(child => {
                        if (child.isMesh) {
                            // Aplică materialul pentru obiectele de tip Mesh
                            child.material = new THREE.MeshStandardMaterial({
                                color: 0xffd1df, // Culoare default
                                transparent: true,
                                opacity: 0.4,
                                depthWrite: true,
                                depthTest: true
                            });
                            child.renderOrder = 1;
                        }
                        // Dacă ai și alte obiecte de tip Group sau alte tipuri de obiecte, poți adăuga mai multe condiții
                        else if (child.isGroup) {
                            // Dacă vrei să aplice culoarea și pentru obiectele din grupuri
                            child.traverse(grandChild => {
                                if (grandChild.isMesh) {
                                    grandChild.material = new THREE.MeshStandardMaterial({
                                        color: 0xffd1df, // Culoare default
                                        transparent: true,
                                        opacity: 0.4,
                                        depthWrite: true,
                                        depthTest: true
                                    });
                                    grandChild.renderOrder = 1;
                                }
                            });
                        }
                    });
                } else {
                    console.warn(`${name} not found`);
                }
            });
            // Parcurge simulatedData pentru a aplica culorile și scorurile
            simulatedData.forEach(regionData => {
                regionData.BrainRegion.forEach(regionName => {
                    const regionGroup = brain.getObjectByName(regionName);  // Căutăm grupul

                    console.log(`Verificare grup: ${regionName}`, regionGroup);

                    if (regionGroup) {
                        // Parcurge toate obiectele din grup
                        regionGroup.traverse(child => {
                            if (child.isMesh) {
                                // Aplică materialul pe fiecare mesh din grup
                                child.material = new THREE.MeshStandardMaterial({
                                    color: regionData.Color,
                                    transparent: true,
                                    opacity: regionData.Score, // Opacitatea bazată pe scor
                                    depthWrite: true,
                                    depthTest: true
                                });
                                child.renderOrder = 1; // Asigură-te că este redat după restul obiectelor
                            }
                        });
                    } else {
                        console.warn(`Grupul nu a fost găsit: ${regionName}`); // Afișează un mesaj dacă grupul nu este găsit
                    }
                });
            });
        } else {
            // Dacă selecția nu este pentru "global", continuă să colorezi regiunile pentru emoțiile specifice
            const regions = emotionMap[emotion];
            const color = emotionToColor[emotion];

            const uniqueRegions = new Set();

            // Adaugă fiecare regiune în set pentru a elimina duplicatele
            regions.forEach(region => {
                // Extrage numele regiunii de bază (fără L/R)
                const regionName = regionNameMap[region.split('-')[0]] || region;
                uniqueRegions.add(regionName);
            });

            const brainRegionRoles = {
                "Ventromedial Prefrontal Cortex (VPC)": "Involved in decision making and emotional regulation, related to rewards and satisfaction.",
                "Nucleus Accumbens (NA)": "Plays a key role in reward processing and motivation.",
                "Insula": "Associated with processing internal sensations and feelings, particularly related to emotions.",
                "Amygdala": "Critical in processing emotions, particularly fear and anger.",
                "Anterior Cingulate Cortex (ACC)": "Involved in emotional regulation, pain processing, and decision-making.",
                "Hypothalamus": "Controls the body’s responses to emotions, including fight or flight reactions.",
                "Dorsolateral Prefrontal Cortex (DPC)": "Involved in higher cognitive functions such as decision-making and controlling impulses.",
                "Orbitofrontal Cortex (OFC)": "Plays a role in reward processing and the evaluation of social and emotional stimuli.",
                "Striatum": "Involved in motivation and the formation of attachment and reward processing.",
                "HiPpocampuS": "Important for memory formation, especially related to fear and emotional experiences.",
                "Basal Ganglia": "Involved in emotional responses, movement regulation, and avoidance behavior."
            };

            emotionDescription.innerHTML = `
                <h4 style="color:${color};">● ${emotion.charAt(0).toUpperCase() + emotion.slice(1)} Emotion</h4>
                <br>
                <p>The following brain regions are most involved in ${emotion}:</p>
                        <ul>
                    ${Array.from(uniqueRegions).map(region => {
                return `<li title="${brainRegionRoles[region]}"">${region}</li>`;
            }).join('')}
                </ul>
            `;

            // Append emotion-specific medical details based on the selected emotion
            switch (emotion.toLowerCase()) {
                case 'joy':
                    emotionDescription.innerHTML += `
                <br>
                <p><strong>Description:</strong> Joy activates brain regions associated with rewards and motivation. VPC and NA are involved in the feeling of reward and satisfaction, while the Insula plays an important role in managing internal sensations.</p>
                <p><strong>Importance:</strong> Increased activity in these areas is often correlated with higher dopamine levels, the main neurotransmitter involved in pleasure and reward.</p>
                <p><strong>Did you know?</strong> Dopamine levels can increase by up to 100% in the nucleus accumbens during joyful experiences, such as listening to music or achieving goals.</p>
                `;
                    break;

                case 'sadness':
                    emotionDescription.innerHTML += `
                <br>
                <p><strong>Description:</strong> Sadness is associated with the activation of regions involved in processing negative emotions and regulating internal states. The Amygdala plays an important role in recognizing and reacting to emotional events.</p>
                <p><strong>Importance:</strong> Studies suggest that when sadness is felt, there is a decrease in activity in regions associated with positive responses and rewards, such as VPC and NA.</p>
                <p><strong>Did you know?</strong> Individuals experiencing sadness show up to a 35% increase in amygdala activity, which is associated with processing negative emotions and social pain.</p>
                `;
                    break;

                case 'anger':
                    emotionDescription.innerHTML += `
                <br>
                <p><strong>Description:</strong> Anger activates regions responsible for rapid, impulsive reactions like the Amygdala and Hypothalamus. DPC plays a role in decision-making and behavioral regulation.</p>
                <p><strong>Importance:</strong> Anger is associated with increased cortisol levels, a stress hormone that prepares the body for quick reactions to danger.</p>
                <p><strong>Did you know?</strong> Cortisol levels can spike by over 50% during intense anger, preparing the body for a fight-or-flight response.</p>
                `;
                    break;

                case 'love':
                    emotionDescription.innerHTML += `
                <br>
                <p><strong>Description:</strong> Love activates brain regions associated with rewarding and forming interpersonal relationships. OFC is involved in evaluating rewards, and the Striatum is essential for processing motivation and creating attachment feelings.</p>
                <p><strong>Importance:</strong> Research shows that love is strongly linked to oxytocin and dopamine levels, neurotransmitters essential for social bonding and feelings of attachment.</p>
                <p><strong>Did you know?</strong> Oxytocin, often called the "love hormone", can increase by up to 300% during bonding moments such as hugging or romantic interaction.</p>
                `;
                    break;

                case 'fear':
                    emotionDescription.innerHTML += `
                <br>
                <p><strong>Description:</strong> Fear activates the Amygdala and other brain regions related to evaluating threats and preparing the body for action. OFC helps regulate behavior in the face of fear, while the Hippocampus is involved in recalling past dangers.</p>
                <p><strong>Importance:</strong> Fear triggers a fight or flight response, and studies show the Amygdala plays a key role in processing dangers quickly.</p>
                <p><strong>Did you know?</strong> The amygdala can activate in less than 20 milliseconds after detecting a fearful stimulus, allowing for lightning-fast emotional responses.</p>
                `;
                    break;

                case 'surprise':
                    emotionDescription.innerHTML += `
                <br>
                <p><strong>Description:</strong> Surprise is often associated with a sudden activation of brain regions responsible for quick reactions and evaluating changes in the environment. DPC helps process reactions and adjust behavior, while SPC manages sensory perceptions.</p>
                <p><strong>Importance:</strong> Surprise plays a role in adapting quickly to new information, and research shows these regions are essential for processing unexpected stimuli.</p>
                <p><strong>Did you know?</strong> Surprise activates both the prefrontal cortex and sensory cortices within 150 milliseconds of encountering unexpected stimuli.</p>
                `;
                    break;

                case 'disgust':
                    emotionDescription.innerHTML += `
                <br>
                <p><strong>Description:</strong> Disgust activates regions associated with negative perceptions and rejecting stimuli. The Insula is associated with processing discomfort sensations, and the Basal Ganglia plays a role in regulating emotional responses to unpleasant stimuli.</p>
                <p><strong>Importance:</strong> Research suggests that disgust is related to avoidance reactions, and increased activity in these regions can lead to the avoidance of stimuli that may pose a health risk.</p>
                <p><strong>Did you know?</strong> Studies show that images of disgust can cause the insula to become active within 300 milliseconds, triggering instinctive avoidance behavior.</p>
                `;
                    break;

                default:
                    emotionDescription.innerHTML += `
                <br>
                <p><strong>No detailed information available for this emotion.</strong></p>
            `;
                    break;
            }

            regions?.forEach(regionName => {
                const regionGroup = brain.getObjectByName(regionName); // Căutăm grupul de regiuni

                if (regionGroup) {
                    // Dacă obiectul găsit este un grup, parcurge fiecare mesh din interiorul grupului
                    regionGroup.traverse(child => {
                        if (child.isMesh) {
                            // Aplică materialul pe fiecare mesh din grup
                            child.material.opacity = 0.9;
                            child.material.color.set(color);
                        }
                    });
                } else {
                    // Dacă este un mesh simplu (nu un grup), aplică direct materialul
                    const region = brain.getObjectByName(regionName);
                    if (region && region.isMesh) {
                        region.material.opacity = 0.9;
                        region.material.color.set(color);
                    }
                }
            });
        }
       
    }
    function updateBrainPredicted(prediction) {
        if (!brain || !prediction) return;

        resetBrain(); // curăță regiunile înainte de actualizare

        // Mapping între denumirile din modelul ML și cele din modelul 3D
        const regionMap = {
            ventromedialPrefrontalCortex: ["VPC-L", "VPC-R"],
            nucleusAccumbens: ["NA-L", "NA-R"],
            amygdala: ["Amygdala"],
            anteriorCingulateCortex: ["ACC-L", "ACC-R"],
            insula: ["Insula-L", "Insula-R"],
            hypothalamus: ["HypoThalamus-L", "HypoThalamus-R"],
            dorsolateralPrefrontalCortex: ["DPC-L", "DPC-R"],
            orbitofrontalCortex: ["OFC-L", "OFC-R"],
            striatum: ["Striatum"],
            hippocampus: ["Hippocampus-L", "Hippocampus-R"],
            superiorParietalCortex: ["SPC-L", "SPC-R"],
            basalGanglia: ["BasalGanglia-L", "BasalGanglia-R"]
        };

        // Culori pentru diferite niveluri de deteriorare
        const damageColors = {
            low: "#FFFF00",     // Galben
            medium: "#FFA500",  // Portocaliu
            high: "#FF0000"     // Roșu
        };

        for (const [mlRegionName, brainRegionNames] of Object.entries(regionMap)) {
            const value = prediction[mlRegionName];

            if (typeof value !== 'number') continue;

            let damageColor;
            if (value >= 0.67) {
                damageColor = damageColors.high;
            } else if (value >= 0.34) {
                damageColor = damageColors.medium;
            } else {
                damageColor = damageColors.low;
            }

            brainRegionNames.forEach(regionName => {
                const region = brain.getObjectByName(regionName);
                if (region) {
                    region.traverse(child => {
                        if (child.isMesh) {
                            if (!child.userData.defaultMaterial) {
                                child.userData.defaultMaterial = child.material.clone();
                            }
                            child.material = child.userData.defaultMaterial.clone();
                            child.material.color.set(damageColor);
                            child.material.opacity = 0.5;
                            child.material.transparent = true;
                            child.renderOrder = 1;
                        }
                    });
                }
            });
        }

        console.log("Brain updated with prediction:", prediction);
    }


    // Eveniment click pe butoane
    document.querySelectorAll('.emotion-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.emotion-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            const emotion = btn.getAttribute('data-emotion');
            highlightEmotion(emotion);
        });
    });
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

    function resetBrain() {
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
    }
    function applyEmotionHighlightForDate(date) {
        if (!brain) return;

        const selectedDayEmotions = emotionsData.filter(e => {
            const emotionDate = typeof e.Date === 'string' ? e.Date.slice(0, 10) : new Date(e.Date).toISOString().slice(0, 10);
            return emotionDate === date;
        });

        console.log(selectedDayEmotions);

        resetBrain();

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

        //applyEmotionHighlightForDate(currentDate);
    }



    function viewBrainDamage() {
        if (!brain) return;

        // Harta pentru daune cerebrale - în funcție de emoțiile negative
        const selectedDate = emotionDatePicker.value;
        const selectedDayEmotions = emotionsData.filter(e => {
            const emotionDate = typeof e.Date === 'string' ? e.Date.slice(0, 10) : new Date(e.Date).toISOString().slice(0, 10);
            return emotionDate === selectedDate;
        });
        console.log("View Brain Damage");
        resetBrain();
        // Logică pentru colorarea regiunilor în funcție de daunele cauzate de emoțiile negative
        selectedDayEmotions.forEach(emotionData => {
            const emotion = emotionData.EmotionLabel;
            const score = emotionData.AverageMoodValue;
            const regions = emotionMap[emotion] || [];

            // Dacă emoția este negativă, aplicăm culorile corespunzătoare
            if (negativeEmotions.includes(emotion)) {
                let damageColor;
                if (score >= 8) {
                    damageColor = damageColors.high; // Roșu pentru daune mari
                } else if (score >= 5) {
                    damageColor = damageColors.medium; // Portocaliu pentru daune medii
                } else {
                    damageColor = damageColors.low; // Galben pentru daune mici
                }

                // Aplicăm culoarea pentru regiunile afectate
                regions.forEach(regionName => {
                    const region = brain.getObjectByName(regionName);
                    if (region) {
                        region.traverse(child => {
                            if (child.isMesh) {
                                if (!child.userData.defaultMaterial) {
                                    child.userData.defaultMaterial = child.material.clone();
                                }
                                child.material = child.userData.defaultMaterial.clone();
                                child.material.color.set(damageColor); // Setăm culoarea daunei
                                child.material.opacity = 0.5; // Opacitate pentru vizibilitate
                                child.material.transparent = true;
                                child.renderOrder = 1; // Asigurăm că se suprapune corect
                            }
                        });
                    }
                });
            }
        });
    }

    // Funcție pentru actualizarea riscurilor și regiunilor echilibrate
    function updateRiskAndBalancedRegions(selectedDate) {
        const selectedDayEmotions = emotionsData.filter(e => {
            const emotionDate = typeof e.Date === 'string' ? e.Date.slice(0, 10) : new Date(e.Date).toISOString().slice(0, 10);
            return emotionDate === selectedDate;
        });

        // Resetăm listele
        riskList.innerHTML = '';
        balancedList.innerHTML = '';

        // Calculăm riscurile și regiunile echilibrate
        const riskRegions = [];
        const balancedRegions = [];

        selectedDayEmotions.forEach(emotionData => {
            const emotion = emotionData.EmotionLabel;
            const score = emotionData.AverageMoodValue;
            const regions = emotionMap[emotion] || [];

            // Dacă emoția este negativă și scorul este mai mare de 5, o considerăm un risc
            if (negativeEmotions.includes(emotion)) {
                if (score > 7) {
                    riskRegions.push(...regions); // Adăugăm regiunile afectate de risc
                } else {
                    balancedRegions.push(...regions); // Adăugăm regiunile echilibrate
                }
            }
        });

        // Populăm listele
        const uniqueRiskRegions = new Set();
        const uniqueBalancedRegions = new Set();

        // Adăugăm regiunile din riskRegions
        riskRegions.forEach(region => {
            const regionName = getFullRegionName(region);
            uniqueRiskRegions.add(regionName);  // Adăugăm doar denumirea completă, fără L/R
        });

        // Adăugăm regiunile din balancedRegions
        balancedRegions.forEach(region => {
            const regionName = getFullRegionName(region);
            uniqueBalancedRegions.add(regionName);  // Adăugăm doar denumirea completă, fără L/R
        });

        // Populăm listele cu denumirile complete (fără duplicate)
        const riskListContent = Array.from(uniqueRiskRegions).map(region => {
            return `<li title="${regionNameMap[region]}"">${region}</li>`;
        }).join('');

        const balancedListContent = Array.from(uniqueBalancedRegions).map(region => {
            return `<li title="${regionNameMap[region]}"">${region}</li>`;
        }).join('');

        // Adăugăm conținutul generat în listele de risc și echilibru
        if (uniqueRiskRegions.size > 0) {
            riskList.innerHTML = riskListContent;
            document.getElementById("noRiskMessage").style.display = 'none';  // Ascundem mesajul de "fără riscuri"
        } else {
            riskList.innerHTML = '';
            document.getElementById("noRiskMessage").style.display = 'block';  // Afișăm mesajul de "fără riscuri"
        }

        if (uniqueBalancedRegions.size > 0) {
            balancedList.innerHTML = balancedListContent;
            document.getElementById("noBalancedMessage").style.display = 'none';  // Ascundem mesajul de "toate regiunile echilibrate"
        } else {
            balancedList.innerHTML = '';
            document.getElementById("noBalancedMessage").style.display = 'block';  // Afișăm mesajul de "toate regiunile echilibrate"
        }

    }

    // Adăugăm eveniment pentru schimbarea datei
    emotionDatePicker.addEventListener("change", function () {
        const selectedDate = this.value;

        // Actualizăm riscurile și regiunile echilibrate
        updateRiskAndBalancedRegions(selectedDate);

        // Activăm butonul pentru a vizualiza daunele cerebrale
        viewBrainDamageButton.disabled = false;

    });

    
    // Funcție pentru a activa vizualizarea daunei cerebrale
    viewBrainDamageButton.addEventListener("click", function () {
        viewBrainDamage();
    });

    
    

}, undefined, function (error) {
    console.error('An error occurred while loading the model:', error);
});

camera.position.z = 5;

function animate() {

    requestAnimationFrame(animate);
    resizeRendererToDisplaySize();
    controls.update();
    renderer.render(scene, camera);
    //console.log("Camera Position:", camera.position);
    //console.log("Camera Target:", controls.target)
}
animate();


document.getElementById("tabInformative").addEventListener("click", function () {
    document.getElementById("informativePanel").style.display = "block";
    document.getElementById("myBrainPanel").style.display = "none";
    this.classList.add("active");
    document.getElementById("tabMyBrain").classList.remove("active");
});

document.getElementById("tabMyBrain").addEventListener("click", function () {
    document.getElementById("informativePanel").style.display = "none";
    document.getElementById("myBrainPanel").style.display = "block";
    this.classList.add("active");
    document.getElementById("tabInformative").classList.remove("active");
});
window.addEventListener('load', () => {
    const emotionDescription = document.getElementById("emotionDescription");
    emotionDescription.innerHTML = `
            <p>
                Each highlighted region corresponds to neural activity related to that emotion. You can hover or click on brain areas for additional info.
            </p>
            <ul>
                <li>Click an emoji to view related brain regions.</li>
                <li>Hover over a brain region to discover its role.</li>
                <li>Observe how emotions interact with brain structure.</li>
            </ul>`;  
});