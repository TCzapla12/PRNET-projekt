import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AddressServiceStub(channel)

create_request = user_pb2.AddressGet(
    owner_id='a7576c32-91a6-4d48-a139-f2ba4693953e'
)

token = get_token(email, password, channel=channel)
create_response = stub.GetUserAddresses(create_request, metadata=[("authorization", f"Bearer {token}")])
print("AddressCreate Response:\n", create_response)
