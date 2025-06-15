from vosk import Model

class ModelLoader:
    def __init__(self, model_path: str):
        self.model_path = model_path
        self._model = None

    def load_model(self):
        if not self._model:
            print(f"Loading model from {self.model_path}...")
            self._model = Model(self.model_path)
        return self._model
