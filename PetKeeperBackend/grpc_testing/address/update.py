import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AddressServiceStub(channel)

update_request = user_pb2.AddressUpdate(
    id='65c912d5-f331-4865-94ca-25d751ec1495',
    city='Nasielsk',
    apartment_number='666'
)

token = get_token(email, password, channel=channel)
update_response = stub.UpdateAddress(update_request, metadata=[("authorization", f"Bearer {token}")])
print("AddressUpdate Response:\n", update_response)
