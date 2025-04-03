import time
import zmq
import pickle 
import tensorflow
from tensorflow import keras
from tensorflow.keras.preprocessing.text import Tokenizer
from tensorflow.keras.preprocessing.sequence import pad_sequences
#from tensorflow.keras.utils.np_utils import to_categorical
import numpy as np

#config
max_len = 23

#set up connection
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")

#load the model
print("Loading models...")
#model = keras.models.load_model('./model.h5')
model_syntax = keras.models.load_model('./model_syntax.h5')
model_morphology = keras.models.load_model('./model_morphology.h5')
model_semantics = keras.models.load_model('./model_semantics.h5')
model_pragmatics = keras.models.load_model('./model_pragmatics.h5')


print("Models loaded successfully")



#load tokenizers
tokenizer_q = None #for questions
tokenizer_a = None #for answers
with open('tokenizer_a.pickle', 'rb') as handle:
    tokenizer_a = pickle.load(handle)

with open('tokenizer_q.pickle', 'rb') as handle:
    tokenizer_q = pickle.load(handle)

#gather questions
#questions = open("./questions.txt","r", encoding='utf-8').readlines()

#print(questions)

def get_seq(sentence, tokenizer):
    t2s = tokenizer_a.texts_to_sequences([sentence])
    padded_seq = pad_sequences(t2s,maxlen=max_len,padding="post")
    print(padded_seq[0])
    return padded_seq[0]

def get_prediction(answer,question,model,tokenizer_a,tokenizer_q):
    psa = get_seq(answer,tokenizer_a)
    psq = get_seq(question,tokenizer_q)
    mp = model.predict([np.array([psa]),np.array([psq])])
    return np.argmax(mp[0])

# answer = "yo soy"
# prediction = str(get_prediction(answer,"adelante",tokenizer_a,tokenizer_q))
# print(prediction)
for i in range(0,10):
    print("-\n")
print(tensorflow.__version__)
print("Server Active, awaiting input for model :)")

question_index = 0
server_active = True
while server_active:
    socket_msg = socket.recv()
    socket_msg = socket_msg.decode("utf-8")
    #grab question and answer from the application
    question = socket_msg.split(';')[0]
    answer = socket_msg.split(';')[1]


    #print("Received request: %s" % socket_msg)
    print("Recieved question answer pair:")
    print("Q: %s" % question)
    print("A: %s" % answer)
    # answer = answer.decode("utf-8")
    # question  = question.decode("utf-8")

    print("Getting predictions...")
    print("Syntax...")
    syntax_prediction = str(get_prediction(answer,question,model_syntax,tokenizer_a,tokenizer_q))

    print("Morphology...")
    morphology_prediction = str(get_prediction(answer,question,model_morphology,tokenizer_a,tokenizer_q))

    print("Semantics...")
    semantics_prediction = str(get_prediction(answer,question,model_semantics,tokenizer_a,tokenizer_q))

    print("Pragmatics...")
    pragmatics_prediction = str(get_prediction(answer,question,model_pragmatics,tokenizer_a,tokenizer_q))

    #prediction = str(get_prediction(answer,questions[question_index],tokenizer_a,tokenizer_q))
    #print(prediction)

    encoded_prediction = syntax_prediction + " " + morphology_prediction + " " + semantics_prediction + " " + pragmatics_prediction

    socket.send(bytes(encoded_prediction, 'utf-8'))
    question_index += 1

print("All responses have been processed.")
print("Server shutting down...")


