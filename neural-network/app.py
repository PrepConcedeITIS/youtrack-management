from IssuesTrainRandomForest import get_best_of_n, save_model, load_model
from flask import Flask, request

app = Flask(__name__)


@app.route("/train", methods=['POST'])
def hello_world():
    file = (request.files['train-data'])
    path = 'datasets/issues/train-data.csv'
    file.save(path)
    accuracy, model = get_best_of_n(path, 10)
    save_model(model)
    return str(accuracy)

app.run()
