import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.feature_extraction import FeatureHasher
from sklearn.utils import shuffle
from sklearn.metrics import classification_report, accuracy_score
from sklearn import metrics

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


csv_file = 'datasets/issues/mockdata.csv'
dataframe = pd.read_csv(csv_file)
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
print(tags_dict)
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

print("Accuracy:", metrics.accuracy_score(y_test, y_pred))
