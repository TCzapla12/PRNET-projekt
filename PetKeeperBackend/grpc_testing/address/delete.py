import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AddressServiceStub(channel)

create_request = user_pb2.AddressMinimal(
    id='6e483e45-e359-487b-ac41-b702a4328f31'
)

token = get_token(email, password, channel=channel)
create_response = stub.DeleteAddress(create_request, metadata=[("authorization", f"Bearer {token}")])
print("DeleteCreate Response:\n", create_response)
