import os
import zmq
from transformers import AutoTokenizer, AutoModelForCausalLM
import torch
import re
import py3langid
from huggingface_hub import login, logout

"""
Custom prompting logic is kept for LLAMA2 with memory optimizations specific to Windows.
"""

def get_tokens_as_list(word_list):
    tokens_list = []
    for word in word_list:
        tokenized_word = tokenizer([word], add_special_tokens=False).input_ids[0]
        tokens_list.append(tokenized_word)
    return tokens_list

print("\n[START-UP] Loading models...")

model_name = "migleolop/Sep1FineTune"  # Replace with your model path

# Enable device offloading for memory saving
model = AutoModelForCausalLM.from_pretrained(
    model_name,
    device_map="auto",  # Automatically load layers across CPU/GPU
    offload_folder="./offload",  # Save offloaded layers to disk temporarily
    torch_dtype=torch.float16,  # Use 16-bit precision for memory saving
)
tokenizer = AutoTokenizer.from_pretrained(model_name)

# Enable gradient checkpointing to save memory
model.gradient_checkpointing_enable()

# Compile model for further optimization
model = torch.compile(model)  # Torch 2.0+ only, significantly reduces memory

device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
model.eval()

print("\n[START-UP] Models loaded successfully!")

def forward_pass(messages):
    while True:
        # Use the correct tokenizer method and ensure 'inputs' is the right tensor type
        inputs = tokenizer.apply_chat_template(
            messages, 
            tokenize=True, 
            add_generation_prompt=True, 
            return_tensors="pt"
        ).to(device)

        # Extracting token tensors for model input
        input_ids = inputs["input_ids"]  # Access from dictionary
        attention_mask = inputs["attention_mask"]  # Access mask if used

        outputs = model.generate(
            input_ids=input_ids,
            attention_mask=attention_mask,
            do_sample=True,
            temperature=0.9,
            top_k=30,
            top_p=0.9,
            max_new_tokens=512,
            num_return_sequences=1,
            use_cache=True
        )

        # Decode the output correctly
        response = tokenizer.decode(outputs[0], skip_special_tokens=True)
        response = re.sub(r'[*(][^)]*[*)]', '', response)  # Filters out parenthesis or asterisks.
        if response.startswith("User:"): 
            response = response.split("User:")[1].strip()

        # Check the language
        lang = py3langid.classify(response)[0]
        if lang == "es":
            break

    return response

def detect_intent(last_output):
    system_prompt = "Determine if the given sentence is a Response, Question, or Refusal."
    intent_frame = [
        {"role": "system", "content": system_prompt},
        {"role": "user", "content": f"{last_output}"},
    ]
    return forward_pass(intent_frame)

if __name__ == "__main__":
    # Login to Hugging Face Hub
    login("YOUR_HF_API_KEY")  # Replace with your API key

    # Set up ZeroMQ server
    port = 8888
    context = zmq.Context()
    socket = context.socket(zmq.REP)
    socket.bind(f"tcp://*:{port}")

    os.system("nvidia-smi")
    print(f"\n[START-UP] Server running on port {port}...")

    # Initial Question Bank
    question_bank = [
        "¿Cómo le ha ido en su viaje?",
        "¿Ha notado algo sospechoso durante sus viajes hasta este momento?",
        "¿Quién empacó sus maletas para el viaje?",
    ]

    # Create initial messages list
    messages = [
        {"role": "system", "content": "Always speak in Spanish. You are an airport customs agent. Interview the User in Spanish about travel security. Never speak in English. Ask only one question."}
    ]

    print("\n[INFO] Waiting for client input...")
    server_active = True

    while server_active:
        socket_msg = socket.recv()
        response = socket_msg.decode("utf-8")
        print(f"\n[INFO] Received: {response}")

        if response == "EXIT":
            print("\n[EXIT] Server shutting down...")
            break

        messages.append({"role": "user", "content": response})

        question = forward_pass(messages)
        messages.append({"role": "assistant", "content": question})
        socket.send_string(question)
        print(f"\n[INFO] Sent: '{question}'")

    logout()
    print("\n[INFO] Server shutdown complete.")
