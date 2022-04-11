from IssuesTrainRandomForest import get_best_of_n, save_model, load_model, predict
from flask import Flask, request, jsonify
from collections import namedtuple

app = Flask(__name__)


@app.route("/train", methods=['POST'])
def train_model():
    file = (request.files['train-data'])
    path = 'datasets/issues/train-data.csv'
    file.save(path)
    accuracy, model = get_best_of_n(path, 100)
    save_model(model)
    result = {'accuracy': accuracy}
    return jsonify(result)


@app.route("/predict", methods=['POST'])
def make_predict():
    json = request.get_json()
    data: list = json['data']
    mapper = namedtuple('IssueInput', 'assigneeLogin complexity issueType tagsConcatenated id')
    issues = list(
        map(lambda x: mapper(x['assigneeLogin'], x['complexity'], x['issueType'], x['tagsConcatenated'], x['id']),
            data))
    result = predict(issues, load_model())
    return jsonify(result)


app.run(host='localhost', port=14100)
