import zmq

port = 8888

# Set up connection
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind(f"tcp://*:{port}")


print(f"\n\t**Loaded model and configuration, starting Server on port {port}...")


print("\n\t>>Waiting for input")
server_active = True
while server_active:

    # Receive input
    socket_msg = socket.recv()
    response = socket_msg.decode("utf-8")

    
    if response == "EXIT":
        
        break
        
    print(f"\n\t>>Received: {response}")

    # Compute question
    question = response.upper()

    # Send question
    socket.send_string(question)
    print(f"\n\t>>Sent: '{question}'")

print("\n\t**All responses have been processed.")
print("\n\t**Server shutting down...")