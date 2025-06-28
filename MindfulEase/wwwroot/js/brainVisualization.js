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

function showLegend(type) {
    const legendContainer = document.getElementById('emotionLegend');
    legendContainer.innerHTML = "";
    const ul = document.createElement('ul');
    const title = document.createElement('h4');
    title.style.color = "#088aa7";
    if (type === "emotional") {
        const emotions = {
            "Joy": "#F6BE00",
            "Sadness": "blue",
            "Fear": "magenta",
            "Anger": "red",
            "Love": "#FF69B4",
            "Surprise": "#FFD700",
            "Disgust": "green"
        };
        title.textContent = "Emotion Legend";
        legendContainer.appendChild(title);

        for (const [emotion, color] of Object.entries(emotions)) {
            const li = document.createElement('li');
            li.style.color = color;
            li.innerHTML = `<span>&#128522;</span> ${emotion}`;
            ul.appendChild(li);
        }
    }
    else if (type === "damage") {
        const damages = {
            "🟡 Low Risk": "#F6BE00",
            "🟠 Moderate Risk": "orange",
            "🔴 High Risk": "red"
        };
        title.textContent = "Damage Legend";
        legendContainer.appendChild(title);
        for (const [risk, color] of Object.entries(damages)) {
            const li = document.createElement('li');
            li.style.color = color;
            li.innerHTML = ` ${risk}`;
            ul.appendChild(li);
        }
    }

    legendContainer.appendChild(ul);
}
function hidePredictionButton(buttonId) {
    const showBtn = document.getElementById(`showPredictionInfoBtn${buttonId}`);
    const modal = document.getElementById(`predictionModal${buttonId}`);
    const modalContent = document.getElementById(`predictionModalContent${buttonId}`);
    const modalTitle = document.getElementById(`predictionModalTitle${buttonId}`);

    if (showBtn) {
        showBtn.style.display = "none";
        showBtn.dataset.listenerAdded = ""; // dacă vrei să refolosești în viitor
    }

    if (modal) {
        modal.style.display = "none";
    }

    if (modalContent) {
        modalContent.innerHTML = "";
    }

    if (modalTitle) {
        modalTitle.textContent = "";
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
        love: ["Insula-L", "Insula-R", "OFC-L", "OFC-R"],
        fear: ["Amygdala", "OFC-L", "OFC-R", "Hippocampus-R"],
        surprise: ["DPC-L", "DPC-R", "SPC-L", "SPC-R"],
        disgust: ["Insula-L", "Insula-R", "BasalGanglia-L", "BasalGanglia-R", "OFC-L", "OFC-R"]
    };
    const brainRegionRoles = {
        "Ventromedial Prefrontal Cortex (VPC)": "Involved in decision making and emotional regulation, related to rewards and satisfaction.",
        "Nucleus Accumbens (NA)": "Plays a key role in reward processing and motivation.",
        "Insula": "Associated with processing internal sensations and feelings, particularly related to emotions.",
        "Amygdala": "Critical in processing emotions, particularly fear and anger.",
        "Anterior Cingulate Cortex (ACC)": "Involved in emotional regulation, pain processing, and decision-making.",
        "Hypothalamus": "Controls the body’s responses to emotions, including fight or flight reactions.",
        "Dorsolateral Prefrontal Cortex (DPC)": "Involved in higher cognitive functions such as decision-making and controlling impulses.",
        "Orbitofrontal Cortex (OFC)": "Plays a role in reward processing and the evaluation of social and emotional stimuli.",
        "Hippocampus": "Important for memory formation, especially related to fear and emotional experiences.",
        "Basal Ganglia": "Involved in emotional responses, movement regulation, and avoidance behavior.",
        "Superior Parietal Cortex (SPC)": "Supports spatial awareness and attention, integrating sensory information for body orientation and movement."
    };
    const brainRegionDescriptions = {
        "Ventromedial Prefrontal Cortex (VPC)": `
        <p><strong>VPC (Ventromedial Prefrontal Cortex)</strong> is a brain region located in the lower part of the frontal lobe, associated with integrating emotions into decision-making processes. This area mediates the balance between emotional responses and rational behaviors.</p><br>

        <p>VPC is considered a hub between the limbic and cognitive systems, strongly connected with the <em>amygdala</em>, <em>hippocampus</em>, and <em>insula</em>. This gives it the ability to integrate past emotional experiences with current situations to guide adaptive behavior. fMRI studies show increased activity in the VPC during decisions involving social rewards or empathy (Bechara et al., 2000; Ochsner & Gross, 2005).</p><br>

        <p>Dysfunctions in this area are correlated with various psychiatric disorders: individuals with lesions in the VPC may display impulsive decisions, antisocial behaviors, or a lack of empathy (Koenigs & Tranel, 2007). In depression, VPC can show hypoactivity, reducing the individual's ability to regulate negative thoughts and perceive rewards.</p><br>
        <p>From a therapeutic perspective, <em>transcranial magnetic stimulation (TMS)</em> and <em>neurofeedback</em> techniques have been investigated as methods to activate this region, aiding cognitive restructuring in depression and anxiety. Additionally, cognitive-behavioral therapies involving decision-making and cognitive reframing can indirectly train VPC activity.</p><br>
        <p><strong>Relevant statistics:</strong> According to a longitudinal study published in *Nature Neuroscience* (Drevets et al., 2008), approximately 65% of patients with major depression show changes in volume or activity in the VPC.</p><br>
        <p><strong>Sources:</strong><br>
        - Bechara et al. (2000), *Science* — “Emotion, decision making and the orbitofrontal cortex”<br>
        - Koenigs & Tranel (2007), *Journal of Neuroscience* — “Prefrontal cortex damage impairs moral judgment”<br>
        - Drevets et al. (2008), *Nat. Neuroscience* — “Functional anatomical correlates of antidepressant response”<br>
        - Ochsner & Gross (2005), *Trends in Cognitive Sciences* — “The cognitive control of emotion”</p>
        `,

        "Nucleus Accumbens (NA)": `
        <p><strong>Nucleus Accumbens (NA)</strong> is a central brain region associated with reward processing, motivation, and pleasure. This area is part of the reward system and is closely linked to the release of dopamine, a neurotransmitter essential for feelings of reward and satisfaction.</p><br>

        <p>Specifically, NA activation is correlated with positive responses to pleasurable stimuli, such as social rewards, music, food, and personal achievements. Under normal conditions, NA promotes adaptive behaviors by reinforcing positive rewards. However, dysfunctions in NA are often involved in addiction disorders, depression, and anhedonia (Salimpoor et al., 2011).</p><br>

        <p>Studies have shown that during addiction experiences, such as substance abuse, NA activity is altered, contributing to the compulsive desire to seek rewards, which leads to a vicious cycle. Additionally, in depression, NA activity may be reduced, leading to a decrease in motivation and a loss of interest in activities that would typically be pleasurable.</p><br>

        <p><strong>Relevant statistics:</strong> According to a study in *Neuron* (Everitt & Robbins, 2005), 70-80% of patients with addiction disorders show significant changes in NA activity.</p><br>

        <p><strong>Sources:</strong><br>
        - Salimpoor et al. (2011), *Science* — “Anatomy of a musical reward”<br>
        - Everitt & Robbins (2005), *Neuron* — “Neural mechanisms of drug addiction”<br>
        `,

        "Insula": `
        <p><strong>Insula</strong> is a deep cortical region located within the temporal lobes, associated with awareness of the body's internal sensations, such as pain, hunger, and thirst, as well as with processing emotions such as disgust and empathy. It is strongly activated in response to physiological or emotional stimuli that involve a body-mind reaction.</p><br>

        <p>The insula plays a fundamental role in integrating internal sensory information with affective evaluations. For example, when we feel discomfort or danger, the insula is responsible for triggering physiological responses, such as increased heart rate or sweating. In anxiety disorders or post-traumatic stress disorder (PTSD), the insula is hyperactivated, contributing to exaggerated perceptions of danger and a constant state of alertness.</p><br>

        <p>Additionally, the insula is involved in empathic processing and identifying the emotions of others, being essential for social functions, including the development of emotional bonds and interpersonal relationships. Recent studies suggest that changes in the volume and activity of the insula are correlated with personality disorders and a lack of empathy.</p><br>

        <p><strong>Relevant statistics:</strong> In a study from *Brain Research* (Critchley et al., 2004), it was found that 40% of individuals with generalized anxiety disorder showed increased insula volume.</p><br>

        <p><strong>Sources:</strong><br>
        - Calder et al. (2007), *Neuron* — “The insula and emotion”<br>
        - Critchley et al. (2004), *Brain Research* — “Neuroanatomy of the insula: Implications for emotion processing”<br>
        `,

        "Amygdala": `
        <p><strong>Amygdala</strong> is a small but highly important subcortical structure that plays a central role in processing emotions, particularly fear, anger, and anxiety. It is activated in response to stimuli involving danger or threat and regulates physiological and behavioral responses through its connections with the <em>hypothalamus</em> and <em>prefrontal cortex</em>.</p><br>

        <p>The activity of the amygdala determines rapid responses to dangerous stimuli, generating reactions such as "fight or flight". However, dysfunctions in amygdala activity are associated with emotional and behavioral disorders such as phobias, PTSD, and generalized anxiety. Specifically, the amygdala is involved in excessive fear states and exaggerated emotional reactions.</p><br>

        <p>Studies have shown that, in PTSD, the amygdala becomes hyperactive, contributing to the constant re-experiencing of traumatic memories, a process essential for developing post-traumatic symptoms. Additionally, increased amygdala activity in stressful conditions may contribute to panic reactions.</p><br>

        <p><strong>Relevant statistics:</strong> In a study from *Nature Neuroscience* (Zald, 2003), it was observed that amygdala activity can increase by up to 35% in cases of severe depression, correlating with heightened feelings of sadness and fear.</p><br>

        <p><strong>Sources:</strong><br>
        - Zald (2003), *Nature Neuroscience* — “The amygdala and emotional processing”<br>
        - Etkin et al. (2006), *Biological Psychiatry* — “Amygdala regulation of emotional responses”<br>
        `,

        "Anterior Cingulate Cortex (ACC)": `
        <p><strong>Anterior Cingulate Cortex (ACC)</strong> plays an essential role in emotional regulation, managing internal conflicts, and processing pain. It is located in the anterior part of the cingulate cortex, near the frontal lobe, and is closely connected to limbic structures, including the <em>amygdala</em> and <em>hypothalamus</em>.</p><br>

        <p>ACC is active during the processing of information that requires self-control and internal conflict evaluation. It also plays an important role in maintaining attention to emotions and modulating emotional responses based on social and situational context. Dysfunctions in ACC are often associated with disorders such as depression, anxiety disorders, and personality disorders.</p><br>

        <p>Studies suggest that low activity in ACC can lead to rumination and persistence in negative thoughts, a phenomenon characteristic of depression. Additionally, ACC plays a significant role in the perception of emotional pain and stress management, and imbalances in this area may be involved in anxiety disorders and treatment-resistant depression.</p><br>

        <p><strong>Relevant statistics:</strong> In a study from *Biological Psychiatry* (Bush et al., 2000), it was found that patients with major depression have reduced ACC activity, and stimulation of this area led to improved emotional control.</p><br>

        <p><strong>Sources:</strong><br>
        - Bush et al. (2000), *Biological Psychiatry* — “The role of the anterior cingulate cortex in the regulation of emotion”<br>
        - Posner et al. (2007), *Neuron* — “The anterior cingulate cortex and cognitive control”<br>
        `,


        "Hypothalamus": `
        <p><strong>The hypothalamus</strong> is a small but vital structure located at the base of the brain, involved in regulating the body's physiological functions including body temperature, heart rate, metabolism, and hormone secretion. The hypothalamus plays a crucial role in the body's emotional and physiological responses, being directly linked to the HPA axis (hypothalamic-pituitary-adrenal), which regulates the stress response.</p><br>

        <p>This homeostasis control center is essential for managing "fight or flight" reactions, activating the release of the stress hormone <em>cortisol</em> and mobilizing the body's resources in the face of a threat. Changes in hypothalamic activity are frequently associated with chronic stress disorders, depression, and sleep disturbances. Imbalances in this area can also contribute to eating disorders and hormonal dysfunctions.</p><br>

        <p>Chronic stress and repeated activation of the HPA axis can lead to structural and functional changes in the hypothalamus, increasing the risk of developing conditions such as anxiety and depression. In addition, elevated cortisol levels in the blood can lead to a reduction in the volume of the hippocampus, a region essential for memory processing.</p><br>

        <p><strong>Relevant statistics:</strong> Research published in *Nature Neuroscience* (Sapolsky, 2004) showed that chronic stress exposure and frequent hypothalamic activation can lead to negative changes in brain structure and deficits in emotional regulation.</p><br>

        <p><strong>Sources:</strong><br>
        - Sapolsky (2004), *Nature Neuroscience* — “The effects of chronic stress on the brain”<br>
        - McEwen (2007), *Neuron* — “Stress, brain, and emotions: The role of the hypothalamus in stress response”<br>
        `,



        "Dorsolateral Prefrontal Cortex (DPC)": `
        <p><strong>The dorsolateral prefrontal cortex (DPC)</strong> is a key region for higher cognitive functions such as decision-making, planning, abstract thinking, and self-control. It is located in the upper part of the prefrontal cortex and is involved in processing information related to planning and logical reasoning.</p><br>

        <p>The DPC is essential for inhibiting impulses and regulating behaviors by evaluating the consequences of actions. In personality disorders, depression, and ADHD, dysfunctions in the DPC are commonly observed, and stimulation of this region is being investigated as a therapeutic method for improving impulse control and emotional regulation. Additionally, the DPC plays a central role in establishing adequate self-control and minimizing impulsive reactions under stress.</p><br>

        <p>In depression, reduced activity in the DPC is associated with a diminished capacity for rational decision-making and regulation of negative emotions, which can lead to mental stagnation and rumination. Transcranial magnetic stimulation (TMS) therapy targeting the DPC has shown improvements in cases of treatment-resistant depression.</p><br>

        <p><strong>Relevant statistics:</strong> In a study published in the *Journal of Cognitive Neuroscience* (Miller & Cohen, 2001), it was shown that DPC stimulation improves the ability to solve complex problems and regulate impulsive behaviors.</p><br>

        <p><strong>Sources:</strong><br>
        - Miller & Cohen (2001), *Journal of Cognitive Neuroscience* — “An integrative theory of prefrontal cortex function”<br>
        - Fuster (2008), *Neuroscience of Cognitive Development* — “The prefrontal cortex and cognitive behavior”<br>
        `,


        "Orbitofrontal Cortex (OFC)": `
        <p><strong>The orbitofrontal cortex (OFC)</strong> is involved in processing rewards and evaluating social stimuli, playing a critical role in behavior assessment and impulse regulation. Located in the lower part of the frontal lobe, the OFC is essential in interpreting social information such as facial expressions and tone of voice, and in forming affective responses.</p><br>

        <p>The OFC is active during decisions involving rewards and punishments and in the development of social attachments. Dysfunction in the OFC can contribute to impulsive behaviors, personality disorders, and difficulties in interpreting social cues. In personality disorders—especially sociopathy and antisocial behaviors—the OFC is often implicated, and reduced activity in this region is linked to a lack of empathy and poor self-control.</p><br>

        <p>Furthermore, the OFC plays a significant role in evaluating moral behavior and making ethical decisions. Changes in OFC activity are observed in various behavioral disorders and impulse control disorders, and stimulating this region is a potential approach in treatments targeting such conditions.</p><br>

        <p><strong>Relevant statistics:</strong> A study in *Neuropsychology* (Kringelbach & Rolls, 2004) found that reduced activity in the OFC was associated with impulsive decision-making and antisocial behavior.</p><br>

        <p><strong>Sources:</strong><br>
        - Kringelbach & Rolls (2004), *Neuropsychology* — “The orbitofrontal cortex and decision making”<br>
        - Bechara et al. (2000), *Science* — “The role of the orbitofrontal cortex in the processing of reward”<br>
        `,



        "Hippocampus": `
        <p><strong>The hippocampus</strong> is one of the brain regions essential for long-term memory formation, spatial information processing, and the consolidation of emotional memories. Located in the medial temporal lobe, the hippocampus is crucial for integrating emotional experiences into declarative memory, helping to form and recall memories related to significant life events.</p><br>

        <p>The hippocampus is highly sensitive to chronic stress, and prolonged exposure to cortisol—the stress hormone—can reduce its volume, negatively impacting the ability to learn and recall information. In post-traumatic stress disorder (PTSD), the hippocampus plays a major role, as traumatic memories are quickly activated and repeatedly revisited, leading to a persistent state of stress and anxiety.</p><br>

        <p>Studies show that reduced hippocampal volume is observed in cases of chronic depression and anxiety disorders, and stimulation of this area can help alleviate symptoms associated with these conditions. The hippocampus is also critical for emotional learning, and changes in its activity are implicated in negative affectivity and affective disorders.</p><br>

        <p><strong>Relevant statistics:</strong> A study in *Nature Neuroscience* (Lupien et al., 2009) found that chronic stress exposure reduces hippocampal volume, which can lead to significant problems with memory and learning.</p><br>

        <p><strong>Sources:</strong><br>
        - Lupien et al. (2009), *Nature Neuroscience* — “The effects of chronic stress on hippocampal volume”<br>
        - Bremner et al. (2003), *Journal of Neuroscience* — “Structural changes in the hippocampus of individuals with post-traumatic stress disorder”<br>
        `,



        "Basal Ganglia": `
        <p><strong>The basal ganglia</strong> are a group of brain nuclei involved in movement control, motor learning, and the regulation of emotional behavior. They play a central role in danger avoidance processes and automatic responses to stimuli, being essential for the formation and maintenance of habits and automatic behaviors.</p><br>

        <p>The basal ganglia are heavily involved in emotional processing, regulating involuntary responses to negative stimuli and modulating facial expressions and automatic motor reactions. Imbalances in this region are often linked to compulsive behavior disorders such as obsessive-compulsive disorder (OCD), nervous tics, and, in some cases, personality disorders.</p><br>

        <p>Additionally, the basal ganglia are involved in modulating disgust reactions and avoidance learning, being crucial for protective behaviors and in processing aversive experiences. In psychiatric disorders such as depression, changes in basal ganglia activity can contribute to automatic responses and social withdrawal.</p><br>

        <p><strong>Relevant statistics:</strong> A study in *Neuroscience* (Trost et al., 2012) demonstrated that basal ganglia dysfunction is closely related to compulsive behaviors and abnormal activity in reward-related regions.</p><br>

        <p><strong>Sources:</strong><br>
        - Trost et al. (2012), *Neuroscience* — “Basal ganglia dysfunction in obsessive-compulsive disorder”<br>
        - Graybiel (2008), *Neuron* — “The basal ganglia and the formation of habits”<br>
        `,
        "Superior Parietal Cortex (SPC)": `
        <p><strong>The superior parietal cortex (SPC)</strong> is located in the upper part of the parietal lobe and plays a critical role in spatial awareness, sensory integration, and attention regulation. This region helps the brain process and integrate visual, tactile, and proprioceptive information to generate an internal map of the body in space.</p><br>

        <p>The SPC is essential for coordinating movements and directing attention to specific spatial locations. It supports working memory related to spatial orientation and contributes to our sense of body ownership and agency. In neuropsychological conditions such as neglect syndrome, lesions in the SPC result in the inability to attend to one side of space, despite intact sensory input.</p><br>

        <p>In the context of emotional and cognitive disorders, dysfunction in the SPC may contribute to disrupted attention control, disorientation, and difficulties in body perception. Studies also suggest that altered activity in this region may be involved in depersonalization and dissociative experiences, especially under high stress or trauma.</p><br>

        <p><strong>Relevant statistics:</strong> A study in *Cerebral Cortex* (Vossel et al., 2014) demonstrated that the SPC is a central node in the dorsal attention network, with decreased activation linked to impairments in attentional shifting and spatial tracking under cognitive load.</p><br>

        <p><strong>Sources:</strong><br>
        - Vossel et al. (2014), *Cerebral Cortex* — “The role of the superior parietal cortex in spatial attention and cognitive control”<br>
        - Culham & Kanwisher (2001), *Nature Reviews Neuroscience* — “Neuroimaging of cognitive functions of the parietal cortex”<br>
        `



    };


    console.log(emotionsData);
    const emotionDatePicker = document.getElementById("emotionDatePicker");
    const predictSlider = document.getElementById("predictSlider");
    const predictYears = document.getElementById("predictYears");
    const viewBrainDamageButton = document.getElementById("viewBrainDamageButton");
    const riskList = document.getElementById("riskList");
    const balancedList = document.getElementById("balancedList");
    const projectionSlider = document.getElementById('projectionSlider');
    const projectionYears = document.getElementById('projectionYears');
    //var today = new Date().toISOString().split('T')[0];
    //document.getElementById("emotionDatePicker").setAttribute("max", today);
    // Setăm valoarea implicită pentru predicție
    predictYears.textContent = predictSlider.value + " years";

    predictSlider.addEventListener("input", async function () {
        hidePredictionButton(1);
        showLegend("damage");
        predictYears.textContent = this.value + " years";
        const years = parseInt(predictSlider.value);
        console.log("Predict button clicked for " + years + " years");

        const selectedDateValue = emotionDatePicker.value;
        if (selectedDateValue) {
            const selectedDate = new Date(selectedDateValue);
            console.log("Trigger fetchPrediction cu:", years, selectedDate);

            document.getElementById("loadingSpinner1").style.display = "block";

            try {
                await fetchPrediction(years, selectedDate);
            } catch (error) {
                console.error("Eroare la fetchPrediction:", error);
            } finally {
                document.getElementById("loadingSpinner1").style.display = "none";
            }
        } else {
            console.warn("Nu a fost selectată o dată.");
            alert("Please select a date first");
        }
    });


    

    async function showPredictionButton(prediction, buttonId) {
        const regionDictionary = {
            "amygdala": "Amygdala",
            "anteriorCingulateCortex": "Anterior Cingulate Cortex (ACC)",
            "basalGanglia": "Basal Ganglia",
            "dorsolateralPrefrontalCortex": "Dorsolateral Prefrontal Cortex (DPC)",
            "hippocampus": "Hippocampus",
            "hypothalamus": "Hypothalamus",
            "insula": "Insula",
            "nucleusAccumbens": "Nucleus Accumbens",
            "orbitofrontalCortex": "Orbitofrontal Cortex (OFC)",
            "superiorParietalCortex": "Superior Parietal Cortex",
            "ventromedialPrefrontalCortex": "Ventromedial Prefrontal Cortex (VMPFC)"
        };

        const recommendations = {
            "Amygdala": "This region governs fear, threat detection, and emotional reactivity. A heightened score may signal emotional hypersensitivity. To soothe the Amygdala, try daily mindfulness practice, calming breathwork, and reducing overstimulation from news or social media.",
            "Anterior Cingulate Cortex (ACC)": "Responsible for emotion appraisal and attention regulation. An elevated risk may reflect internal conflict or cognitive-emotional dysregulation. Activities like journaling and CBT can help bring clarity and calm to this region.",
            "Basal Ganglia": "Tied to motivation and motor/emotional control. If overactivated, it may indicate stress buildup or compulsive behaviors. Try grounding activities like yoga, tai chi, or rhythmic movement.",
            "Dorsolateral Prefrontal Cortex (DPC)": "The center of reasoning and executive function. A risk score here may reflect burnout or mental fatigue. Strategic decision-making exercises, puzzle games, and prioritizing rest can restore its strength.",
            "Hippocampus": "Core for memory and emotional context. When strained, this may relate to trauma or excessive stress. Therapeutic writing, storytelling, and recalling positive memories may promote resilience.",
            "Hypothalamus": "Regulates stress response and hormonal balance. An at-risk signal can relate to chronic stress. Focus on sleep hygiene, regular meals, and nature exposure to harmonize this system.",
            "Insula": "Handles emotional self-awareness. A high score might suggest disconnection from bodily sensations. Practice body scans, mindful eating, or mirror meditation to reestablish this link.",
            "Nucleus Accumbens": "Processes pleasure and reward. High stress here could reduce motivation or joy. Engage in hobbies, celebrate small wins, and foster social connection for dopamine boosts.",
            "Orbitofrontal Cortex (OFC)": "Involved in emotional regulation and reward evaluation. At-risk scores may reflect impulsivity or emotional swings. Use structured goal setting and emotion labeling to regain control.",
            "Superior Parietal Cortex": "Governs attention and sensory coordination. Overactivity might lead to sensory overload or disorientation. Grounding practices and sensory breaks are recommended.",
            "Ventromedial Prefrontal Cortex (VMPFC)": "Integrates emotion with decision-making. High activation might reflect emotional confusion. Practicing gratitude journaling and perspective-taking can aid integration."
        };

        let report = `<h3>🧠 Brain-Based Emotional Risk Assessment</h3>
    <p>This predictive report is generated based on your current emotional and contextual inputs, interpreted through a machine learning model trained on the <strong>NKI-Rockland Sample (NKI-RS)</strong> — a large-scale, open-access neuroimaging dataset used in neuroscience and psychology research. The values reflect the <strong>relative activation or involvement of various brain regions in emotional processing</strong> and potential dysregulation. Scores range from 0 (low involvement) to 1 (high involvement).</p><br/>`;

        const highRisk = [];
        const lowRisk = [];

        // Split regions into high and low risk
        for (let region in prediction) {
            const regionName = regionDictionary[region] || region;
            const score = prediction[region];

            if (score > 0.5) {
                highRisk.push({ regionName, score, recommendation: recommendations[regionName] });
            } else {
                lowRisk.push({ regionName, score });
            }
        }

        // Section: All Scores Overview
        report += `<h4>📊 Full Overview of Brain Region Scores:</h4><ul>`;
        const allRegions = [...highRisk, ...lowRisk];
        allRegions.sort((a, b) => b.score - a.score);
        for (const { regionName, score } of allRegions) {
            report += `<li><strong>${regionName}</strong>: ${score.toFixed(3)}</li>`;
        }
        report += `</ul><hr/>`;

        // Section: High-Risk Explanation
        if (highRisk.length > 0) {
            report += `
        <h4>🚨 Regions with Elevated Emotional Risk (Score > 0.5)</h4>
        <p>The following regions show higher-than-average activation and may indicate an increased level of emotional load, cognitive effort, or stress response. This does not necessarily imply pathology, but rather an opportunity for reflection and support.</p>
        <p>These predictions are not diagnostic and should be interpreted as indicators for potential mental strain. Interventions such as therapy, self-regulation practices, and healthy lifestyle adjustments may be helpful for restoring balance in these regions.</p>
        <br/>`;

            for (const { regionName, score, recommendation } of highRisk) {
                report += `
                <div style="margin-bottom: 20px;">
                    <h4>🔸 ${regionName}</h4>
                    <p><strong>Risk Score:</strong> ${score.toFixed(3)}</p>
                    <p>${recommendation}</p>
                </div>`;
            }
        }

        // Section: Low-Risk Explanation
        if (lowRisk.length > 0) {
            report += `
        <h4>✅ Regions with Stable or Low Activation (Score ≤ 0.5)</h4>
        <p>The brain regions listed below are currently within a balanced range of activity. This suggests good emotional regulation, healthy cognitive functioning, or reduced stress impact in these areas. Maintaining this balance through rest, mindfulness, and emotional awareness is beneficial for long-term mental health.</p>
        <ul>`;
            for (const { regionName, score } of lowRisk) {
                report += `<li><strong>${regionName}</strong>: ${score.toFixed(3)}</li>`;
            }
            report += `</ul>`;
        }

        report += `<hr/><p style="font-size: 0.9em;"><em>This report is for informational purposes only and does not substitute for clinical advice. For concerns regarding emotional health or neurological function, please consult a licensed healthcare provider.</em></p>`;

        // Găsim elementele specifice acestui buton/modal
        const modalTitle = document.getElementById(`predictionModalTitle${buttonId}`);
        const modalContent = document.getElementById(`predictionModalContent${buttonId}`);
        const modal = document.getElementById(`predictionModal${buttonId}`);
        const showBtn = document.getElementById(`showPredictionInfoBtn${buttonId}`);

        if (!modal || !modalTitle || !modalContent || !showBtn) {
            console.warn(`Elementele pentru butonul ${buttonId} nu au fost găsite.`);
            return;
        }

        // Actualizăm și afișăm modalul
        modalTitle.textContent = "Brain Prediction Report";
        modalContent.innerHTML = report;
        showBtn.style.display = "block";

        // Adăugăm handler o singură dată (verificare înainte)
        if (!showBtn.dataset.listenerAdded) {
            showBtn.addEventListener("click", function () {
                modal.style.display = "flex";
            });
            showBtn.dataset.listenerAdded = "true";
        }
    }
    


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
            showPredictionButton(prediction, 1);
            hidePredictionButton(2);

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
            <li>Click on a brain region to see detailed scientific information about its function and emotional relevance.</li>
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

            
            console.log("Setare innerHTML...");
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

            setTimeout(() => {
                const items = document.querySelectorAll('#emotionDescription li');

                items.forEach(li => {
                    li.style.cursor = "pointer";
                    li.addEventListener('click', () => {
                        const regionName = li.textContent.trim();
                        const description = brainRegionDescriptions[regionName];
                        if (description) {
                            document.getElementById('regionTitle').textContent = regionName;
                            document.getElementById('regionInfo').innerHTML = description;
                            document.getElementById('regionModal').style.display = 'flex';
                        }
                    });
                });
            }, 0); 

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
            hippocampus: ["Hippocampus-R"],
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
            showLegend("emotional");
            const selectedDate = this.value;
            if (predictSlider) {
                predictSlider.value = 0;
                if (predictYears) {
                    predictYears.textContent = predictSlider.value; // actualizează textul pentru predictYears
                }
            }
            applyEmotionHighlightForDate(selectedDate);
            hidePredictionButton(1);
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
            return `<li title="${brainRegionRoles[region]}"">${region}</li>`;
        }).join('');

        const balancedListContent = Array.from(uniqueBalancedRegions).map(region => {
            return `<li title="${brainRegionRoles[region]}"">${region}</li>`;
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
            document.getElementById("balancedRegionsText").style.display = 'block';
        } else {
            balancedList.innerHTML = '';
            document.getElementById("noBalancedMessage").style.display = 'block';  // Afișăm mesajul de "toate regiunile echilibrate"
            document.getElementById("balancedRegionsText").style.display = 'none';
        }

        setTimeout(() => {
            const items = document.querySelectorAll('#riskList li');

            items.forEach(li => {
                li.style.cursor = "pointer";
                li.addEventListener('click', () => {
                    const regionName = li.textContent.trim();
                    const description = brainRegionDescriptions[regionName];
                    if (description) {
                        document.getElementById('regionTitle').textContent = regionName;
                        document.getElementById('regionInfo').innerHTML = description;
                        document.getElementById('regionModal').style.display = 'flex';
                    }
                });
            });
        }, 0); 
        setTimeout(() => {
            const items = document.querySelectorAll('#balancedList li');

            items.forEach(li => {
                li.style.cursor = "pointer";
                li.addEventListener('click', () => {
                    const regionName = li.textContent.trim();
                    const description = brainRegionDescriptions[regionName];
                    if (description) {
                        document.getElementById('regionTitle').textContent = regionName;
                        document.getElementById('regionInfo').innerHTML = description;
                        document.getElementById('regionModal').style.display = 'flex';
                    }
                });
            });
        }, 0); 

    }

    // Adăugăm eveniment pentru schimbarea datei
    emotionDatePicker.addEventListener("change", function () {
        const selectedDate = this.value;
        showLegend("emotional");
        // Actualizăm riscurile și regiunile echilibrate
        updateRiskAndBalancedRegions(selectedDate);

        // Activăm butonul pentru a vizualiza daunele cerebrale
        viewBrainDamageButton.disabled = false;
        hidePredictionButton(1);

    });

    
    // Funcție pentru a activa vizualizarea daunei cerebrale
    viewBrainDamageButton.addEventListener("click", function () {
        showLegend("damage");
        viewBrainDamage();
    });

    async function fetchWeightedPrediction(yearsSinceStart) {
        if (isNaN(yearsSinceStart)) {
            alert("Selectează o valoare pentru ani.");
            return;
        }

        const body = {
            yearsSinceStart: yearsSinceStart
        };

        try {
            const response = await fetch("/Visualize/PredictBrainDamageWeighted", {
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
            showPredictionButton(prediction, 2);
            hidePredictionButton(1);

        } catch (error) {
            console.error("Eroare la predicție:", error);
            alert("A apărut o problemă la generarea predicției.");
        }
    }

    // Setăm valoarea implicită pentru predicție
    projectionYears.textContent = projectionSlider.value + " years";

    projectionSlider.addEventListener("input", async function () {
        hidePredictionButton(2);
        projectionYears.textContent = this.value + " years";
        const years = parseInt(projectionSlider.value);
        console.log("Predicting for " + years + " years");
       
        document.getElementById("loadingSpinner2").style.display = "block";

        try {
            await fetchWeightedPrediction(years);
        } catch (error) {
            console.error("Eroare la fetchPrediction:", error);
        } finally {
            document.getElementById("loadingSpinner2").style.display = "none";
        }
    });

    
    document.getElementById("tabBrainProjection").addEventListener("click", async function () {
        document.getElementById("loadingSpinner2").style.display = "block";

        try {
            await fetchWeightedPrediction(0);
        } catch (error) {
            console.error("Eroare la fetchPrediction:", error);
        } finally {
            document.getElementById("loadingSpinner2").style.display = "none";
            showLegend("damage");
        }
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
    showLegend("emotional");
    document.getElementById("informativePanel").style.display = "block";
    document.getElementById("myBrainPanel").style.display = "none";
    document.getElementById("brainProjectionPanel").style.display = "none"; // Ascundem tabul de Brain Projection
    this.classList.add("active");
    document.getElementById("tabMyBrain").classList.remove("active");
    document.getElementById("tabBrainProjection").classList.remove("active");
    hidePredictionButton(2);
});

document.getElementById("tabMyBrain").addEventListener("click", function () {
    showLegend("emotional");
    document.getElementById("informativePanel").style.display = "none";
    document.getElementById("myBrainPanel").style.display = "block";
    document.getElementById("brainProjectionPanel").style.display = "none"; // Ascundem tabul de Brain Projection
    this.classList.add("active");
    document.getElementById("tabInformative").classList.remove("active");
    document.getElementById("tabBrainProjection").classList.remove("active");
    hidePredictionButton(2);
});

document.getElementById("tabBrainProjection").addEventListener("click", function () {
    document.getElementById("informativePanel").style.display = "none";
    document.getElementById("myBrainPanel").style.display = "none"; // Ascundem tabul de MyBrain
    document.getElementById("brainProjectionPanel").style.display = "block"; // Afișăm tabul de Brain Projection
    this.classList.add("active");
    document.getElementById("tabInformative").classList.remove("active");
    document.getElementById("tabMyBrain").classList.remove("active");
});

// Funcție pentru pan, simulează mișcarea pe touchpad
function panCamera(direction) {
    const panSpeed = 0.1;

    // Vectorul de pan va fi compus din axele camerei
    const panOffset = new THREE.Vector3();

    // Obține axele camerei
    const cameraRight = new THREE.Vector3();
    camera.getWorldDirection(panOffset); // direcția în față
    cameraRight.crossVectors(camera.up, panOffset).normalize(); // axa dreapta
    const cameraUp = camera.up.clone().normalize(); // axa sus

    // Alegem direcția de deplasare
    switch (direction) {
        case 'left':
            panOffset.copy(cameraRight).multiplyScalar(-panSpeed);
            break;
        case 'right':
            panOffset.copy(cameraRight).multiplyScalar(panSpeed);
            break;
        case 'up':
            panOffset.copy(cameraUp).multiplyScalar(panSpeed);
            break;
        case 'down':
            panOffset.copy(cameraUp).multiplyScalar(-panSpeed);
            break;
    }

    // Aplicăm offset-ul poziției camerei și țintei
    camera.position.add(panOffset);
    controls.target.add(panOffset);
    controls.update();
}


// Declarație pentru intervalul de mișcare
let panInterval = null;

// Începe mișcarea continuă la apăsarea butonului
function startPan(direction) {
    if (!panInterval) {
        panInterval = setInterval(() => {
            panCamera(direction);
        }, 16);  // Aproape 60fps
    }
}

// Oprește mișcarea la eliberarea butonului
function stopPan() {
    clearInterval(panInterval);
    panInterval = null;
}


let zoomInterval = null;

function startZoom(direction) {
    const zoomSpeed = 0.05;
    const currentDistance = camera.position.distanceTo(controls.target);
    let zoomChange = 0;

    if (direction === 'in' && currentDistance > 0.5) {
        zoomChange = -zoomSpeed * currentDistance;
    } else if (direction === 'out' && currentDistance < 100) {
        zoomChange = zoomSpeed * currentDistance;
    }

    // Dacă nu există un interval activ, începem zoom-ul continuu
    if (zoomChange !== 0 && !zoomInterval) {
        zoomInterval = setInterval(() => {
            const directionVector = new THREE.Vector3().subVectors(camera.position, controls.target).normalize();
            camera.position.addScaledVector(directionVector, zoomChange);
            controls.target.addScaledVector(directionVector, zoomChange);
            controls.update();
        }, 16); // Aproape 60fps
    }
}

function stopZoom() {
    if (zoomInterval) {
        clearInterval(zoomInterval);
        zoomInterval = null;
    }
}


window.addEventListener('load', () => {
    showLegend("emotional");
    var today = new Date().toISOString().split('T')[0];
    document.getElementById("emotionDatePicker").setAttribute("max", today);
    const emotionDescription = document.getElementById("emotionDescription");
    emotionDescription.innerHTML = `
            <p>
                Each highlighted region corresponds to neural activity related to that emotion. You can hover or click on brain areas for additional info.
            </p>
            <ul>
                <li>Click an emoji to view related brain regions.</li>
                <li>Hover over a brain region to discover its role.</li>
                <li>Click on a brain region to see detailed scientific information about its function and emotional relevance.</li>
                <li>Observe how emotions interact with brain structure.</li>
            </ul>`; 
    const panUpButton = document.getElementById('pan-up');
    const panDownButton = document.getElementById('pan-down');
    const panLeftButton = document.getElementById('pan-left');
    const panRightButton = document.getElementById('pan-right');
    const zoomInButton = document.getElementById('zoom-in');
    const zoomOutButton = document.getElementById('zoom-out');
    // Verifică dacă butoanele sunt găsite corect
    console.log(panUpButton, panDownButton, panLeftButton, panRightButton, zoomInButton, zoomOutButton);


    if (panUpButton) {
        panUpButton.addEventListener('mousedown', function () {
            startPan('up');
        });
        panUpButton.addEventListener('mouseup', function () {
            stopPan();
        });
        panUpButton.addEventListener('mouseleave', function () {  // Asigură-te că pan-ul se oprește dacă utilizatorul părăsește butonul
            stopPan();
        });
    }

    if (panDownButton) {
        panDownButton.addEventListener('mousedown', function () {
            startPan('down');
        });
        panDownButton.addEventListener('mouseup', function () {
            stopPan();
        });
        panDownButton.addEventListener('mouseleave', function () {
            stopPan();
        });
    }

    if (panLeftButton) {
        panLeftButton.addEventListener('mousedown', function () {
            startPan('left');
        });
        panLeftButton.addEventListener('mouseup', function () {
            stopPan();
        });
        panLeftButton.addEventListener('mouseleave', function () {
            stopPan();
        });
    }

    if (panRightButton) {
        panRightButton.addEventListener('mousedown', function () {
            startPan('right');
        });
        panRightButton.addEventListener('mouseup', function () {
            stopPan();
        });
        panRightButton.addEventListener('mouseleave', function () {
            stopPan();
        });
    }

    if (zoomInButton) {
        zoomInButton.addEventListener('mousedown', function () {
            startZoom('in');
        });
        zoomInButton.addEventListener('mouseup', function () {
            stopZoom();
        });
        zoomInButton.addEventListener('mouseleave', function () {
            stopZoom();
        });
    }

    if (zoomOutButton) {
        zoomOutButton.addEventListener('mousedown', function () {
            startZoom('out');
        });
        zoomOutButton.addEventListener('mouseup', function () {
            stopZoom();
        });
        zoomOutButton.addEventListener('mouseleave', function () {
            stopZoom();
        });
    }
    
});



