from IssuesTrainRandomForest import get_best_of_n, save_model, load_model, predict
from flask import Flask, request, jsonify
from collections import namedtuple

app = Flask(__name__)


@app.route("/train/<project>", methods=['POST'])
def train_model(project):
    file = (request.files['train-data'])
    path = f'datasets/issues/{project}-train-data.csv'
    file.save(path)
    accuracy, model = get_best_of_n(path, 100)
    save_model(model, project)
    result = {'accuracy': accuracy}
    return jsonify(result)


@app.route("/predict/<project>", methods=['POST'])
def make_predict(project):
    json = request.get_json()
    data: list = json['data']
    mapper = namedtuple('IssueInput', 'assigneeLogin complexity issueType tagsConcatenated id')
    issues = list(
        map(lambda x: mapper(x['assigneeLogin'], x['complexity'], x['issueType'], x['tagsConcatenated'], x['id']),
            data))
    result = predict(issues, load_model(project))
    return jsonify(result)


app.run(host='localhost', port=14100)
