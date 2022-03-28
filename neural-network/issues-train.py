import numpy as np
import pandas as pd
import tensorflow as tf
import pydot
from tensorflow.keras import layers


def df_to_dataset(dataframe, shuffle=True, batch_size=32):
    df = dataframe.copy()
    labels = df.pop('target')
    df = {key: value[:, tf.newaxis] for key, value in dataframe.items()}
    ds = tf.data.Dataset.from_tensor_slices((dict(df), labels))
    if shuffle:
        ds = ds.shuffle(buffer_size=len(dataframe))
    ds = ds.batch(batch_size)
    ds = ds.prefetch(batch_size)
    return ds


def get_normalization_layer(name, dataset):
    # Create a Normalization layer for the feature.
    normalizer = layers.Normalization(axis=None)

    # Prepare a Dataset that only yields the feature.
    feature_ds = dataset.map(lambda x, y: x[name])

    # Learn the statistics of the data.
    normalizer.adapt(feature_ds)

    return normalizer


def get_category_encoding_layer(name, dataset, dtype, max_tokens=None):
    # Create a layer that turns strings into integer indices.
    if dtype == 'string':
        index = layers.StringLookup(max_tokens=max_tokens)
    # Otherwise, create a layer that turns integer values into integer indices.
    else:
        index = layers.IntegerLookup(max_tokens=max_tokens)

    # Prepare a `tf.data.Dataset` that only yields the feature.
    feature_ds = dataset.map(lambda x, y: x[name])

    # Learn the set of possible values and assign them a fixed integer index.
    index.adapt(feature_ds)

    # Encode the integer indices.
    encoder = layers.CategoryEncoding(num_tokens=index.vocabulary_size())

    # Apply multi-hot encoding to the indices. The lambda function captures the
    # layer, so you can use them, or include them in the Keras Functional model later.
    return lambda feature: encoder(index(feature))


csv_file = 'datasets/issues/mockdata0.csv'

# tf.keras.utils.get_file('petfinder_mini.zip', dataset_url, extract=True, cache_dir='.')
dataframe = pd.read_csv(csv_file)

dataframe['target'] = np.where(dataframe['SuccessGrade'] >= 3, 'Great', 'False')

# Drop unused features.
dataframe = dataframe.drop(
    columns=['Summary', 'ProjectName', 'IdReadable', 'EstimationError', 'ReviewRefuses', 'TestRefuses'])
dataframe.head()
# used columns input:
# TagsConcatenated - Categorical - String есть для дальнейшей проверки
# AssigneeLogin - Categorical - String есть для дальнейшей проверки
# Complexity - Categorical - String есть для дальнейшей проверки
# IssueType - Categorical - String есть для дальнейшей проверки
# Summary - Text - String есть для дальнейшей проверки, но нет в моковом датасете(мб генерить тоже?)

# EstimationError - Numerical - Float нет для дальнейшей проверки - не берем как инпут, потому что будет ломать результаты при предикте
# ReviewRefuses - Numerical - Integer нет для дальнейшей проверки - не берем как инпут, потому что будет ломать результаты при предикте
# TestRefuses - Numerical - Integer нет для дальнейшей проверки - не берем как инпут, потому что будет ломать результаты при предикте

# output
# SuccessGrade - Categorical - Number (в тренировочном датасете формируется исходя из возвратов и ошибки оценки)

train_features, val, test = np.split(dataframe.sample(frac=1), [int(0.8 * len(dataframe)), int(0.9 * len(dataframe))])

#
# value1 = 1
# value2 = 2
# value3 = 4
# value4 = 8
# [val1, val3] = 5/15 = 0.(3)
# [val1, val4] = 9/15 = ..
# [val1, val3] = 5
# [val1, val3, val2] = 5

#   1 2 3
# a 5 2 5
# b 3 5 1
# c 2 3 1
batch_size = 60
train_ds = df_to_dataset(train_features, batch_size=batch_size)
val_ds = df_to_dataset(val, shuffle=False, batch_size=batch_size)
test_ds = df_to_dataset(test, shuffle=False, batch_size=batch_size)

all_inputs = []
encoded_features = []

categorical_cols = ['TagsConcatenated', 'AssigneeLogin', 'Complexity', 'IssueType']

for header in categorical_cols:
    categorical_col = tf.keras.Input(shape=(1,), name=header, dtype='string')
    encoding_layer = get_category_encoding_layer(name=header,
                                                 dataset=train_ds,
                                                 dtype='string',
                                                 max_tokens=5)
    encoded_categorical_col = encoding_layer(categorical_col)
    all_inputs.append(categorical_col)
    encoded_features.append(encoded_categorical_col)

all_features = tf.keras.layers.concatenate(encoded_features)
x = tf.keras.layers.Dense(32, activation="relu")(all_features)
x = tf.keras.layers.Dropout(0.5)(x)
output = tf.keras.layers.Dense(1)(x)

model = tf.keras.Model(all_inputs, output)

model.compile(optimizer='adam',
              loss=tf.keras.losses.BinaryCrossentropy(from_logits=True),
              metrics=["accuracy"])

# Use `rankdir='LR'` to make the graph horizontal.
# todo: graphviz + pydot
# tf.keras.utils.plot_model(model, show_shapes=True, rankdir="LR")

model.fit(train_ds, epochs=10, validation_data=val_ds)

loss, accuracy = model.evaluate(test_ds)
print("Accuracy", accuracy)

#model.save('issues_classifier')
reloaded_model = tf.keras.models.load_model('issues_classifier')

sample = {
    'TagsConcatenated': 'React',
    'IssueType': 'Feature',
    'Complexity': 'Junior',
    'AssigneeLogin': 'Mark_Davis'
}

input_dict = {name: tf.convert_to_tensor([value]) for name, value in sample.items()}
predictions = reloaded_model.predict(input_dict)
prob = tf.nn.sigmoid(predictions[0])

print(f'success grade - {prob}')