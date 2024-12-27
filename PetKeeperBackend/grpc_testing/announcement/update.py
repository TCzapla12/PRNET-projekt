import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
# Create a channel to connect to the .NET server
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.UserServiceStub(channel)  # Replace Greeter with your service name

email = "new_user22222@example.com"
password = "securepassword123"
# 1. get a new user
id = user_pb2.UserIdentifier(email=email)
update_request = user_pb2.UserUpdate(
    user_id=id,
    is_admin=True,
    avatar_url="/example/path/to/file/on/machine.png"
)
token = get_token(email, password, channel=channel)
update_response = stub.UpdateUser(update_request, metadata=[("authorization", f"Bearer {token}")])
print("UpdateUser Response:\n", update_response)
