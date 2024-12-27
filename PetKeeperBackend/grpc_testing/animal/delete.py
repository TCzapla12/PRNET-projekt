import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AnimalServiceStub(channel)

create_request = user_pb2.AnimalMinimal(
    id='6227a1cd-c990-4515-8d29-a6c0645b8f44',
    #type='dog',
    #name='Fafik'
)

token = get_token(email, password, channel=channel)
create_response = stub.DeleteAnimal(create_request, metadata=[("authorization", f"Bearer {token}")])
print("AnimalDelete Response:\n", create_response)
