from transformers import pipeline

# Încarcă modelul EmoLLMs
sentiment_analyzer = pipeline("text-classification", model="bhadresh-savani/bert-base-uncased-emotion")

# Exemplu de text
text = "I am so happy today!"

# Analizează emoțiile
result = sentiment_analyzer(text)
print(result)
