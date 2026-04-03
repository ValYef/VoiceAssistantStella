from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import LogisticRegression

class TfidfModel:
    def __init__(self):
        self.vectorizer = TfidfVectorizer()
        self.model = LogisticRegression()

    def train(self, questions, intents):
        X = self.vectorizer.fit_transform(questions)
        self.model.fit(X, intents)

    def predict(self, text):
        X = self.vectorizer.transform([text])
        return self.model.predict(X)[0]