import json
from vosk import KaldiRecognizer
import speech_pb2
import speech_pb2_grpc

class SpeechRecognizerServicer(speech_pb2_grpc.SpeechRecognizerServicer):
    def __init__(self, model):
        self.model = model

    def StreamRecognize(self, request_iterator, context):
        recognizer = KaldiRecognizer(self.model, 16000)
        recognizer.SetWords(True)

        for chunk in request_iterator:
            if recognizer.AcceptWaveform(chunk.audio_data):
                result = json.loads(recognizer.Result())
                yield speech_pb2.RecognizeResponse(text=result["text"])
            else:
                partial = json.loads(recognizer.PartialResult())
                if partial.get("partial"):
                    print("Partial:", partial["partial"])
                    yield speech_pb2.RecognizeResponse(text=partial["partial"])

        final = json.loads(recognizer.FinalResult())
        print("final text: ", final)
        if final.get("text"):
            yield speech_pb2.RecognizeResponse(text=final["text"])
