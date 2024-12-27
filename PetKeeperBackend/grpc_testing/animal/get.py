import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AnimalServiceStub(channel)

create_request = user_pb2.AnimalGet(
    #owner_id='8600d7f0-b6cb-41e2-a9e4-6607e92d0d6b',
    #type='dog',
    #name='Fafik'
)

token = get_token(email, password, channel=channel)
create_response = stub.GetAnimals(create_request, metadata=[("authorization", f"Bearer {token}")])
print("AnimalsGet Response:\n", create_response)
