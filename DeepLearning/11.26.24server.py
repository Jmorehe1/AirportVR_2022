import os
import zmq
from transformers import AutoTokenizer, AutoModelForCausalLM #BitsAndBytesConfig
from huggingface_hub import login, logout
import torch
import re  # String filtering of model output *action* or (translation)
import py3langid # Language classifier being used for filter

"""
Initialize custom prompting to resolve LLAMA2 issues
Chat -> initial question to ask client
Stylize -> ability to change the output question of model to different formatting/scenarios
Forward -> non-pipeline forward through LLAMA2


TODO
-Build a better stylize function
-Consider different memory methods to work with sylization
-Spanish system prompts
-Question bank 
"""

def get_tokens_as_list(word_list):
    "Converts a sequence of words into a list of tokens"
    tokens_list = []
    for word in word_list:
        tokenized_word = tokenizer([word], add_special_tokens=False).input_ids[0]
        tokens_list.append(tokenized_word)
    return tokens_list

# Load the model and tokenizer
print("\n[START-UP] Loading models...")

model_name = "migleolop/Sep1FineTune" # meta-llama/Llama-2-7b-chat-hf or migleolop/Sep1FineTune
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
print(f"MODEL LOADING: {model_name} USING TORCH DEVICE: {device}") 
print("^MAKE SURE TORCH DEVICE IS *GPU* AND NOT CPU^")
tokenizer = AutoTokenizer.from_pretrained(model_name)
model = AutoModelForCausalLM.from_pretrained(model_name, torch_dtype=torch.float16).to(device)
model.eval()

# bad_words_ids = get_tokens_as_list(["¡", "Great", "Perfecto", "Entendido", "!", "(", ")", "Haha"])
'''
These phrases would be tokenized and fed into model.generate in order to avoid generating these
phrases. ", bad_words_ids=bad_words_ids" would be passed in the def line of the forward_pass function. 
'''

print("\n[START-UP] Models loaded successfully")

def forward_pass(messages):
    while True:
        inputs = tokenizer.apply_chat_template(messages, tokenize=True, add_generation_prompt=True, return_tensors="pt").to(device)
        # inputs = tokenizer(messages, return_tensors="pt").to("cuda")
        outputs = model.generate(inputs, do_sample=True, temperature=0.9, top_k=30, top_p=0.9, max_new_tokens=512, num_return_sequences=1, use_cache=True)
        response = tokenizer.decode(outputs[0][len(inputs[0]):-1])
        response = re.sub(r'[*(][^)]*[*)]', '', response)  # Filters out parenthesis or asterisks.
        if response.startswith("User:"): # Filters out instances where "User:" responses are generated
            response = response.split("User:")[1].strip()
        # Language check
        lang,  = langid.classify(response)
        if lang == "es":  # Only proceed if the response is in Spanish
            break
    return response 

def detect_sentiment(model_response):
    """
    Function intends to classify and identify mood for agent to simulate matching facial expressions. 
    """
    # response = "No entiendo qué estás tratando de decir." # TEST SENTENCE, rename to use. 
    result = emotion_classifier(model_response)
    # Mood: {result[0]['label']}, Score: {result[0]['score']} # result follows this data format. 
    mood = result[0]['label']
    return mood

    system_prompt = "Determine if the given sentence is a Response, Question, or Refusal."
    
    intent_frame = [{"role": "system", "content": system_prompt}, 
                    {"role": "user", "content": "Voy a enseñar nuevos reclutas"},
                    {"role": "assistant", "content": "Response"},
                    {"role": "user", "content": "Sí"},
                    {"role": "assistant", "content": "Response"},
                    
                    {"role": "user", "content": "Por qué estoy aquí"},
                    {"role": "assistant", "content": "Question"},
                    {"role": "user", "content": "Por qué quieres saber eso"},
                    {"role": "assistant", "content": "Question"},
                    
                    {"role": "user", "content": "Hola"},
                    {"role": "assistant", "content": "Response"},
                   
                    {"role": "user", "content": "No tengo libertad para decir"},
                    {"role": "assistant", "content": "Refusal"},
                    {"role": "user", "content": "No deseo responder eso"},
                    {"role": "assistant", "content": "Refusal"},
                    
                    {"role": "user", "content": f"{last_output}"},
                   ]
    
    return forward_pass(intent_frame)


if __name__ == "__main__":
    # Login to model
    login("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")

    # Set up connection
    port = 8888
    context = zmq.Context()
    socket = context.socket(zmq.REP)
    socket.bind(f"tcp://*:{port}")

    # Display memory 
    os.system("nvidia-smi")

    print(f"\n[START-UP] Loaded model and configuration, starting Server on port {port}...")

    # Initial Question Bank
    question_bank = ["¿Cómo le ha ido en su viaje?",
                    "¿Ha notado algo sospechoso durante sus viajes hasta este momento?",
                    "¿Quién empaco sus maletas para el viaje?",
                    "¿Estará trabajando en la embajada de Estados Unidos, verdad?",
                    "¿Tiene con usted documentos oficiales que lo comprueben?",
                    "¿Cuánto tiempo lleva usted como parte de la Fuerza Aérea?",
                    "¿Cuándo fue que compro usted los boletos del vuelo?",
                    "¿Cómo se transportara usted hacia su destino?",
                    ]

    # Create initial messages list
    messages = [{"role": "system", "content": "Always speak in Spanish. You are an airport customs agent. Interview the User in Spanish relating to travel security. Never speak in English. Ask only a single question."}]

    print("\n\t[INFO] Waiting for client input")
    server_active = True
    first_input = True
    while server_active:

        for _ in range(10):
            socket_msg = socket.recv()
            response = socket_msg.decode("utf-8")
            print(f"\n\t[INFO] Received: {response}")

            messages.append({"role": "user", "content": response})

            # Compute question from user input
            question = forward_pass(messages)

            # Add computed question
            messages.append({"role": "assistant", "content": question})

            # Send question
            socket.send_string(question)
            print(f"\n\t[INFO] Sent: '{question}'") # \n\t>>Current history: \n{messages}

        print("\n\t[INFO] Exiting loop.")

        # Receive input
        socket_msg = socket.recv()
        response = socket_msg.decode("utf-8")
        print(f"\n\t[INFO] Received: {response}")
        
        if response == "EXIT":
            print("\n\t[EXIT]")
            break

        # Add user input
        messages.append({"role": "user", "content": response})

        # Intent detection
        print(f"\n\t[INFO] Detected intent: {detect_intent(response)}")


        # New question from bank
        question = question_bank.pop(0)
        messages.append({"role": "assistant", "content": question})

        # Send popped question
        socket.send_string(question)
        print(f"\n\t[INFO] Sent: '{question}'") # \n\t>>Current history: \n{messages}
        print("\n\t[INFO] Entering loop.")


    logout()
    print("\n[INFO] All responses have been processed.")
    print("\n[EXIT] Server shutting down...")
    print("\n\n\n", messages)
