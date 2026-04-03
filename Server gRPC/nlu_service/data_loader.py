import pandas as pd

def load_data(path):
    df = pd.read_csv(path)

    df = df.dropna(subset=['question', 'answer', 'intent'])

    df['question'] = df['question'].astype(str)
    df['answer'] = df['answer'].astype(str)
    df['intent'] = df['intent'].astype(str)

    return df['question'].tolist(), df['answer'].tolist(), df['intent'].tolist()