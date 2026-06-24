from pydoc import text
import os
import numpy as np
from ml_models.model_tfidf import TfidfModel
from ml_models.model_bert import BertModel
from data_loader import load_data
from sklearn.metrics.pairwise import cosine_similarity

CONFIDENCE_THRESHOLD = 0.75
LOW_VALUE_WORDS = {"ок", "гаразд", "ага", "так", "ні"}

class NLUService:
    def get_or_create_embeddings(self, model, questions, cache_path="dataset_embeddings.npy"):
        if os.path.exists(cache_path):
            print("Loading cached BERT embeddings...")
            return np.load(cache_path)

        print("Encoding dataset with BERT (first run)...")
        embeddings = model.encode(questions, batch_size=64, show_progress_bar=True)
        np.save(cache_path, embeddings)
        return embeddings
    
    def __init__(self, dataset_path):
        questions, answers, intents = load_data(dataset_path)

        # TF-IDF
        self.model_tfidf = TfidfModel()
        self.model_tfidf.train(questions, intents)

        # BERT intent classifier
        self.model_bert = BertModel()
        self.model_bert.train(questions, intents)
        
        self.questions = questions
        self.intents = intents

        self.dataset_embeddings = self.get_or_create_embeddings(
            self.model_bert.model,
            questions,
            cache_path="dataset_embeddings.npy"
        )

        # ответная база
        self.intent_responses = dict(zip(intents, answers))


    def predict(self, text, model_type="bert"):
        
        if len(text.strip().split()) < 2 or text.strip().lower() in LOW_VALUE_WORDS:
            return {
                "intent": None,
                "answer": None,
                "confidence": 0.0
            }

        if model_type == "tfidf":
            intent = self.model_tfidf.predict(text)
            confidence = 1.0
        else:
            query_emb = self.model_bert.model.encode([text], show_progress_bar=False)
            sims = cosine_similarity(query_emb, self.dataset_embeddings)[0]
            best_idx = sims.argmax()
            intent = self.intents[best_idx]
            confidence = sims[best_idx]

        if confidence < CONFIDENCE_THRESHOLD:
            return {
                "intent": None,
                "answer": "Я не впевнена, що правильно зрозуміла. Можеш уточнити?",
                "confidence": float(confidence)
            }
        answer = self.get_response(intent)

        return {
            "intent": intent,
            "answer": answer,
            "confidence": float(confidence)
        }
    def get_response(self, intent):
        return self.intent_responses.get(
            intent,
            "Я не знаю відповіді на це питання"
        )
    