import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
# Create a channel to connect to the .NET server
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.UserServiceStub(channel)  # Replace Greeter with your service name

email = "new_user22222@example.com"
password = "securepassword123"

id = user_pb2.UserIdentifier(email=email)
# 1. get a new user
get_request = user_pb2.UserGet(
    user_id=id
)

token = get_token(email, password, channel=channel)
get_response = stub.GetUser(get_request, metadata=[("authorization", f"Bearer {token}")])
print("GetUser Response:\n", get_response)
