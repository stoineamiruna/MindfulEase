import pandas as pd
import random
import numpy as np

# Setări generale
NUM_PARTICIPANTS = 200
YEARS_SPAN = 10
YEAR_INTERVAL = 2
NUM_POINTS = YEARS_SPAN // YEAR_INTERVAL + 1
SEED = 42
random.seed(SEED)
np.random.seed(SEED)

# Listele de regiuni și emoții
brain_regions = [
    "VentromedialPrefrontalCortex", "NucleusAccumbens", "Amygdala", "AnteriorCingulateCortex",
    "Insula", "Hypothalamus", "DorsolateralPrefrontalCortex", "OrbitofrontalCortex",
    "Striatum", "Hippocampus", "SuperiorParietalCortex", "BasalGanglia"
]

emotions = ["joy", "sadness", "anger", "love", "fear", "surprise", "disgust"]
positive_emotions = {"joy", "love"}
negative_emotions = {"sadness", "anger", "fear", "surprise", "disgust"}

# Mapping emoții -> regiuni
emotion_brain_mapping = {
    "joy": ["VentromedialPrefrontalCortex", "NucleusAccumbens"],
    "sadness": ["Amygdala", "AnteriorCingulateCortex", "Insula"],
    "anger": ["Amygdala", "Hypothalamus", "DorsolateralPrefrontalCortex"],
    "love": ["Insula", "OrbitofrontalCortex", "Striatum"],
    "fear": ["Amygdala", "AnteriorCingulateCortex", "Hippocampus"],
    "surprise": ["DorsolateralPrefrontalCortex", "SuperiorParietalCortex"],
    "disgust": ["Insula", "OrbitofrontalCortex", "BasalGanglia"]
}

# Tipuri de participanți
participant_types = {
    "optimist": {"joy": 7, "love": 6, "anger": 2, "sadness": 2, "fear": 3, "surprise": 4, "disgust": 2},
    "anxious": {"joy": 2, "love": 3, "anger": 5, "sadness": 6, "fear": 8, "surprise": 5, "disgust": 6},
    "stable": {"joy": 5, "love": 5, "anger": 4, "sadness": 4, "fear": 4, "surprise": 4, "disgust": 4}
}

# Generare date
def generate_data():
    all_data = []

    for participant_id in range(1, NUM_PARTICIPANTS + 1):
        sex = random.choice(["M", "F"])
        start_age = random.randint(18, 40)
        p_type = random.choice(list(participant_types.keys()))
        emotion_base = participant_types[p_type]

        damage_offset = {"optimist": 0.2, "anxious": 0.4, "stable": 0.3}
        previous_damage = {
            region: np.clip(np.random.normal(damage_offset[p_type], 0.05), 0, 1)
            for region in brain_regions
        }

        for i in range(NUM_POINTS):
            age = start_age + i * YEAR_INTERVAL
            years_since_start = i * YEAR_INTERVAL

            participant_emotions = {
                emo: int(np.clip(np.random.normal(loc=emotion_base[emo], scale=2), 0, 10))
                for emo in emotions
            }

            current_damage = {}
            for region in brain_regions:
                damage = previous_damage[region]
                influence = 0.0

                for emo, regions in emotion_brain_mapping.items():
                    if region in regions:
                        emo_score = participant_emotions[emo]
                        if emo in positive_emotions:
                            influence -= emo_score * 0.01
                        else:
                            influence += emo_score * 0.01

                age_effect = (age - 18) * 0.001
                change = influence + age_effect + np.random.normal(0, 0.005)
                new_damage = np.clip(damage + change, 0, 1)
                current_damage[region] = round(new_damage, 4)

            # Trăsături noi
            positive_sum = sum([participant_emotions[e] for e in positive_emotions])
            negative_sum = sum([participant_emotions[e] for e in negative_emotions])
            emotion_balance = positive_sum - negative_sum
            total_emotion = sum(participant_emotions.values())

            row = {
                "ParticipantID": participant_id,
                "Age": age,
                "Sex": sex,
                "YearsSinceStart": years_since_start,
                **participant_emotions,
                **current_damage,
                "EmotionBalance": emotion_balance,
                "TotalEmotionIntensity": total_emotion
            }

            all_data.append(row)
            previous_damage = current_damage

    return pd.DataFrame(all_data)

# Generare și salvare dataset
df = generate_data()
df.to_csv("synthetic_brain_damage_dataset.csv", index=False)
print("Dataset generated successfully with", len(df), "rows!")
