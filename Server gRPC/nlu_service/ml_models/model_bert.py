from sentence_transformers import SentenceTransformer
from sklearn.metrics.pairwise import cosine_similarity

class BertModel:
    def __init__(self):
        self.model = SentenceTransformer('paraphrase-multilingual-MiniLM-L12-v2')

    def train(self, questions, answers):
        self.questions = questions
        self.answers = answers
        self.embeddings = self.model.encode(questions)

    def predict(self, query):
        query_emb = self.model.encode([query])
        sims = cosine_similarity(query_emb, self.embeddings)
        idx = sims.argmax()
        score = sims[0][idx]

        return self.answers[idx], float(score)