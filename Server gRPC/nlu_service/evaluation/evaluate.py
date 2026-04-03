import time
import random
import matplotlib.pyplot as plt
import seaborn as sns
from sklearn.metrics import accuracy_score, f1_score, confusion_matrix
from sklearn.metrics.pairwise import cosine_similarity
from collections import Counter
from service import NLUService
from data_loader import load_data

dataset_path = "C:\\Users\\lera6\\Documents\\Навчання\\3 курс\\ТП\\Курсова\\VoiceAssistantStella\\Server gRPC\\nlu_service\\data\\dataset_civic_terms.csv"

def split_data(questions, intents, test_size=0.2):
    data = list(zip(questions, intents))
    random.shuffle(data)
    split_idx = int(len(data) * (1 - test_size))
    train_data = data[:split_idx]
    test_data = data[split_idx:]
    q_train, i_train = zip(*train_data)
    q_test, i_test = zip(*test_data)
    return list(q_train), list(i_train), list(q_test), list(i_test)


def evaluate():
    print("Loading data...")
    questions, answers, intents = load_data(dataset_path)

    q_train, i_train, q_test, i_test = split_data(questions, intents)
    print(f"Train size: {len(q_train)}, Test size: {len(q_test)}")

    nlu = NLUService(dataset_path)

    # TF-IDF
    print("\n=== TF-IDF Evaluation ===")
    start = time.time()
    tfidf_preds = []
    tfidf_confidences = []
    
    for q in q_test:
        X = nlu.model_tfidf.vectorizer.transform([q])
        probs = nlu.model_tfidf.model.predict_proba(X)[0]

        confidence = max(probs)
        tfidf_confidences.append(confidence)

        pred = nlu.model_tfidf.model.classes_[probs.argmax()]
        tfidf_preds.append(pred)
        
    tfidf_time = time.time() - start
    tfidf_acc = accuracy_score(i_test, tfidf_preds)
    tfidf_f1 = f1_score(i_test, tfidf_preds, average='weighted')
    print("TF-IDF Accuracy:", tfidf_acc)
    print("TF-IDF F1-score:", tfidf_f1)
    print("TF-IDF Time:", tfidf_time)

    # BERT
    print("\n=== BERT Evaluation ===")
    start = time.time()

    bert_preds = []
    bert_confidences = []

    questions_list = questions
    intents_list = intents

    questions_embeddings = nlu.model_bert.model.encode(
        questions_list,
        batch_size=32,
        show_progress_bar=True
    )

    queries_embeddings = nlu.model_bert.model.encode(
        q_test,
        batch_size=32,
        show_progress_bar=True
    )

    sims_matrix = cosine_similarity(queries_embeddings, questions_embeddings)

    for sims in sims_matrix:
        best_idx = sims.argmax()
        confidence = sims[best_idx]
        matched_intent = intents_list[best_idx]

        bert_preds.append(matched_intent)
        bert_confidences.append(confidence)

    bert_time = time.time() - start
    bert_acc = accuracy_score(i_test, bert_preds)
    bert_f1 = f1_score(i_test, bert_preds, average='weighted')
    print("BERT Accuracy:", bert_acc)
    print("BERT F1-score:", bert_f1)
    print("BERT Time:", bert_time)

    plot_confusion_matrices(i_test, tfidf_preds, bert_preds)

    plot_metrics(
        tfidf_acc, bert_acc,
        tfidf_time, bert_time
    )
    
    plot_confidence_histograms(tfidf_confidences, bert_confidences)
    

def plot_metrics(tfidf_acc, bert_acc, tfidf_time, bert_time):

    plt.figure(figsize=(12,5))

    # Accuracy
    plt.subplot(1, 2, 1)
    plt.bar(["TF-IDF", "BERT"], [tfidf_acc, bert_acc])
    plt.ylim(0, 1)
    plt.title("Accuracy Comparison")

    # Time
    plt.subplot(1, 2, 2)
    plt.bar(["TF-IDF", "BERT"], [tfidf_time, bert_time])
    plt.title("Time Comparison")

    plt.tight_layout()
    plt.show()

def plot_confidence_histograms(tfidf_confidences, bert_confidences):
    plt.figure(figsize=(12,5))

    plt.subplot(1, 2, 1)
    plt.hist(tfidf_confidences, bins=20, alpha=0.7)
    plt.title("TF-IDF Confidence")
    plt.xlabel("Confidence")

    plt.subplot(1, 2, 2)
    plt.hist(bert_confidences, bins=20, alpha=0.7)
    plt.title("BERT Confidence")
    plt.xlabel("Similarity")

    plt.tight_layout()
    plt.show()
    
def plot_confusion_matrices(i_test, tfidf_preds, bert_preds):

    top_intents = [item[0] for item in Counter(i_test).most_common(10)]

    # TF-IDF
    tfidf_true = []
    tfidf_pred = []

    for true, pred in zip(i_test, tfidf_preds):
        if true in top_intents:
            tfidf_true.append(true)
            tfidf_pred.append(pred)

    # BERT
    bert_true = []
    bert_pred = []

    for true, pred in zip(i_test, bert_preds):
        if true in top_intents:
            bert_true.append(true)
            bert_pred.append(pred)

    labels = top_intents

    tfidf_cm = confusion_matrix(tfidf_true, tfidf_pred, labels=labels)
    bert_cm = confusion_matrix(bert_true, bert_pred, labels=labels)

    plt.figure(figsize=(14,6))

    plt.subplot(1, 2, 1)
    sns.heatmap(tfidf_cm, annot=True, fmt='d', xticklabels=labels, yticklabels=labels)
    plt.title("TF-IDF")

    plt.subplot(1, 2, 2)
    sns.heatmap(bert_cm, annot=True, fmt='d', xticklabels=labels, yticklabels=labels)
    plt.title("BERT")

    plt.tight_layout()
    plt.show()
     
if __name__ == "__main__":
    evaluate()