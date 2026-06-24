import json
from pydoc import text
from vosk import KaldiRecognizer
import speech_pb2 as speech__pb2
import speech_pb2_grpc as speech__pb2_grpc
import requests

class SpeechRecognizerServicer(speech__pb2_grpc.SpeechRecognizerServicer):
    def __init__(self, model):
        self.model = model
        self.nlu_url = "http://nlu:8000/predict"

    def StreamRecognize(self, request_iterator, context):
        recognizer = KaldiRecognizer(self.model, 16000)
        recognizer.SetWords(True)

        for chunk in request_iterator:
            if recognizer.AcceptWaveform(chunk.audio_data):
                result = json.loads(recognizer.Result())
                recognized_text = result.get("text", "")

                if recognized_text:
                    print(f"[ASR FINAL] {recognized_text}")
                    
                    nlu_result = self.call_nlu(recognized_text)
                    print("[NLU]", nlu_result)

                    yield speech__pb2.RecognizeResponse(
                        text=recognized_text,
                        answer=nlu_result.get("answer", ""),
                        intent=nlu_result.get("intent", "") or "",
                        confidence=float(nlu_result.get("confidence", 0.0))
                    )

            else:
                partial = json.loads(recognizer.PartialResult())
                partial_text = partial.get("partial", "")

                if partial_text:
                    print(f"[ASR PARTIAL] {partial_text}")
                    yield speech__pb2.RecognizeResponse(
                        text=partial_text,
                        answer="",
                        intent="",
                        confidence=0.0
                    )

        final = json.loads(recognizer.FinalResult())
        final_text = final.get("text", "")

        if final_text:
            print(f"[ASR FINAL END] {final_text}")
            
            nlu_result = self.call_nlu(final_text)
            print("[NLU FINAL]", nlu_result)

            yield speech__pb2.RecognizeResponse(
                    text=final_text,
                    answer=nlu_result.get("answer", ""),
                    intent=nlu_result.get("intent", "") or "",
                    confidence=float(nlu_result.get("confidence", 0.0))
                )
            
    def call_nlu(self, text):
        try:
            response = requests.get(
                self.nlu_url,
                params={"text": text, "model_type": "bert"},
                timeout=2
            )

            data = response.json()

            return {
                "answer": data.get("answer"),
                "intent": data.get("intent"),
                "confidence": float(data.get("confidence") or 0.0)
            }

        except Exception as e:
            print(f"NLU error: {e}")
            return {
                "answer": "Помилка обробки запиту",
                "intent": None,
                "confidence": 0.0
            }