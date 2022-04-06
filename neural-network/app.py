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
    result = {'accuracy': accuracy};
    return jsonify(accuracy)


@app.route("/predict", methods=['POST'])
def make_predict():
    json = request.get_json()
    data: list = json['data']
    mapper = namedtuple('IssueInput', 'AssigneeLogin Complexity IssueType TagsConcatenated Id')
    issues = list(
        map(lambda x: mapper(x['AssigneeLogin'], x['Complexity'], x['IssueType'], x['TagsConcatenated'], x['Id']),
            data))
    result = predict(issues, load_model())
    return jsonify(result)


app.run(host='localhost', port=14100)
