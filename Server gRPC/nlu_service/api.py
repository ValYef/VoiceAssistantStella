from fastapi import FastAPI
from service import NLUService

app = FastAPI()

nlu_service = NLUService("data/dataset_civic_terms.csv")

@app.get("/")
def root():
    return {"message": "NLU service is running"}

@app.get("/predict")
def predict(text: str, model_type: str = "bert"):
    result = nlu_service.predict(text, model_type)
    return result