import grpc
import user_pb2
import user_pb2_grpc
# Create a channel to connect to the .NET server
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.UserServiceStub(channel)  # Replace Greeter with your service name

# 1. get a new user
update_request = user_pb2.UserFull(
    email="new_user22222@example.com",
    is_admin=True,
    username="BOASTY_ADMIN",
    avatar_url="example.co"
)
update_response = stub.UpdateUser(update_request)
print("UpdateUser Response:\n", update_response)
