import os

import pandas as pd
from pandas import DataFrame
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.feature_extraction import FeatureHasher
from sklearn.utils import shuffle
from sklearn import metrics
from datetime import datetime
import joblib
import glob


def enum_to_int(delimiter: int, values: {}, tag: str):
    sum = 0
    for sub_tag in tag.split(','):
        sum += values[sub_tag]
    return sum / delimiter


def encode_string_column(input_dataframe, column_source, n_features=10):
    encoder = FeatureHasher(n_features, input_type='string')
    to_encode = (input_dataframe[column_source])
    encoded = encoder.transform(to_encode).toarray()
    input_dataframe = input_dataframe.drop(columns=[column_source])
    encoded = pd.DataFrame(encoded)
    encoded.columns = map(lambda x: f'{column_source}Enc{x}', encoded.columns)
    return pd.concat([input_dataframe, encoded], axis=1)


def save_model(model):
    os.makedirs('models', exist_ok=True)
    joblib.dump(model, f'models/random-forest-{datetime.utcnow().strftime("%Y-%m-%d-%H-%M")}.joblib')


def load_model():
    models = glob.glob("models/random-forest-*.joblib")
    last = sorted(models, reverse=True)[0]
    return joblib.load(last)


def train(path: str):
    dataframe: DataFrame = pd.DataFrame([])
    if not os.path.isfile(path):
        csv_file = 'datasets/issues/mockdata.csv'
        dataframe = pd.read_csv(csv_file)
    else:
        dataframe = pd.read_csv(path)

    dataframe = dataframe.drop(
        columns=['Summary', 'ProjectName', 'IdReadable', 'EstimationError', 'ReviewRefuses', 'TestRefuses'])
    dataframe = shuffle(dataframe)

    tags = (dataframe['TagsConcatenated'])
    tags_dict = {}
    uniq_tags = tags.unique()
    for tag in uniq_tags:
        if ',' in tag:
            for sub_tag in (tag.split(',')):
                if sub_tag not in tags_dict:
                    tags_dict[sub_tag] = 2 ** len(tags_dict)
        else:
            if tag not in tags_dict:
                tags_dict[tag] = 2 ** len(tags_dict)
    delimiter = sum(list(tags_dict.values())[-2:])
    tags = tags.transform(lambda tag: enum_to_int(delimiter, tags_dict, tag))
    dataframe['TagsConcatenated'] = tags

    categorical_features = ['AssigneeLogin', 'Complexity', 'IssueType']
    for feature in categorical_features:
        dataframe = encode_string_column(dataframe, feature)

    x = dataframe.drop(columns=['SuccessGrade'])
    y = dataframe['SuccessGrade']
    X_train, X_test, y_train, y_test = train_test_split(x, y, test_size=0.3, random_state=85)
    clf = RandomForestClassifier(n_estimators=30)
    clf.fit(X_train, y_train)
    y_pred = clf.predict(X_test)

    accuracy = metrics.accuracy_score(y_test, y_pred)
    return accuracy, clf


def get_best_of_n(path: str, n: int = 1):
    best_accuracy = -1
    model = RandomForestClassifier()
    for i in range(n):
        accuracy, model = train(path)
        if accuracy > best_accuracy:
            best_accuracy = accuracy

    print(f'best accuracy = {best_accuracy}')
    return best_accuracy, model
