import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier

def enum_to_int(delimiter: int, values: {}, tag: str):
    sum = 0
    for sub_tag in tag.split(','):
        sum += values[sub_tag]
    return sum/delimiter

csv_file = 'datasets/issues/mockdata.csv'
dataframe = pd.read_csv(csv_file)
dataframe = dataframe.drop(
    columns=['Summary', 'ProjectName', 'IdReadable', 'EstimationError', 'ReviewRefuses', 'TestRefuses'])

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

print(dataframe.head())

x = dataframe.drop(columns=['SuccessGrade'])
y = dataframe['SuccessGrade']
X_train, X_test, y_train, y_test = train_test_split(x, y, test_size=0.3, random_state=85)
clf = RandomForestClassifier(n_estimators=30)
clf.fit(X_train, y_train)
y_pred = clf.predict(X_test)
print(y_pred)
