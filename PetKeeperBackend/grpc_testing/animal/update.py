import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AnimalServiceStub(channel)

update_request = user_pb2.AnimalUpdate(
    id='6227a1cd-c990-4515-8d29-a6c0645b8f44',
    name='Gryzak',
    description='Niestety gryzie i nie lubi nieznajomych. Tylko dla specjalistów'
)

token = get_token(email, password, channel=channel)
update_response = stub.UpdateAnimal(update_request, metadata=[("authorization", f"Bearer {token}")])
print("AnimalUpdate Response:\n", update_response)
