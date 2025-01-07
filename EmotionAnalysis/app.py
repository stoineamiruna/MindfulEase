from flask import Flask, request, jsonify
from transformers import pipeline

app = Flask(__name__)

# Încarcă modelul EmoLLMs
sentiment_analyzer = pipeline("text-classification", model="bhadresh-savani/bert-base-uncased-emotion")

@app.route('/analyze-emotion', methods=['POST'])
def analyze_emotion():
    text = request.json.get("text")  # Primi textul din cererea JSON
    if not text:
        return jsonify({"error": "No text provided"}), 400

    result = sentiment_analyzer(text)  # Analizează emoțiile textului
    return jsonify(result)  # Returnează rezultatul ca JSON

if __name__ == "__main__":
    app.run(debug=True, host='0.0.0.0', port=5000)
