import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AddressServiceStub(channel)

create_request = user_pb2.AddressGet(
    owner_id='408f7387-352f-4ea5-aeb8-bdea694ce496'
)

token = get_token(email, password, channel=channel)
create_response = stub.GetUserAddresses(create_request, metadata=[("authorization", f"Bearer {token}")])
print("AddressCreate Response:\n", create_response)
