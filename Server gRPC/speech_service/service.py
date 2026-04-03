import json
from pydoc import text
from vosk import KaldiRecognizer
import speech_service.speech_pb2 as speech_pb2
import speech_service.speech_pb2_grpc as speech_pb2_grpc
import requests

class SpeechRecognizerServicer(speech_pb2_grpc.SpeechRecognizerServicer):
    def __init__(self, model):
        self.model = model
        self.nlu_url = "http://localhost:8000/predict"

    def StreamRecognize(self, request_iterator, context):
        recognizer = KaldiRecognizer(self.model, 16000)
        recognizer.SetWords(True)

        for chunk in request_iterator:
            if recognizer.AcceptWaveform(chunk.audio_data):
                result = json.loads(recognizer.Result())
                text = result.get("text", "")

                if text:
                    nlu_result = self.call_nlu(text)

                    yield speech_pb2.RecognizeResponse(
                        text=text,
                        answer=nlu_result.get("answer", "")
                    )
                else:
                    partial = json.loads(recognizer.PartialResult())
                    if partial.get("partial"):
                        yield speech_pb2.RecognizeResponse(
                            text=partial["partial"]
                        )

        final = json.loads(recognizer.FinalResult())
        text = final.get("text", "")

        if text:
            nlu_result = self.call_nlu(text)

            yield speech_pb2.RecognizeResponse(
                text=text,
                answer=nlu_result.get("answer", "")
            )
    
    def call_nlu(self, text):
        try:
            response = requests.get(
                self.nlu_url,
                params={
                    "text": text,
                    "model_type": "bert"
                },
                timeout=2
            )
            return response.json()
        except Exception as e:
            print(f"NLU error: {e}")
            return {"answer": "Помилка обробки запиту"}
