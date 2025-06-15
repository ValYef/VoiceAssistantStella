import grpc
from concurrent import futures
from model_loader import ModelLoader
from speech_service import SpeechRecognizerServicer
import speech_pb2_grpc

def serve():
    loader = ModelLoader("models/vosk-model-uk-v3-lgraph")
    model = loader.load_model()

    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    speech_pb2_grpc.add_SpeechRecognizerServicer_to_server(SpeechRecognizerServicer(model), server)
    server.add_insecure_port('[::]:50051')
    print("gRPC server listening on port 50051")
    server.start()
    server.wait_for_termination()

if __name__ == "__main__":
    serve()
