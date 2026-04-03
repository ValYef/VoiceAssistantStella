from ml_models.model_tfidf import TfidfModel
from ml_models.model_bert import BertModel
from data_loader import load_data

class NLUService:
    def __init__(self, dataset_path):
        questions, answers, intents = load_data(dataset_path)
        
        # TF-IDF
        self.model_tfidf = TfidfModel()
        self.model_tfidf.train(questions, intents)
        
        # BERT
        self.model_bert = BertModel()
        self.model_bert.train(questions, answers)

        # для поиска intent по answer
        self.intent_to_answer = dict(zip(intents, answers))

    def predict(self, text, model_type="bert"):
        if model_type == "tfidf":
            intent = self.model_tfidf.predict(text)
            answer = self.intent_to_answer.get(intent, "Не знаю відповіді")
            return {"intent": intent, "answer": answer}
        else:
            answer = self.model_bert.predict(text)
            # ищем intent по answer
            intent = None
            for k, v in self.intent_to_answer.items():
                if v == answer:
                    intent = k
                    break
            return {"intent": intent, "answer": answer}