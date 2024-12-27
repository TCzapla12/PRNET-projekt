import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
# Create a channel to connect to the .NET server

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AnnouncementServiceStub(channel)  # Replace Greeter with your service name

request = user_pb2.AnnouncementMinimal(
    id='ccbc659c-5b62-41c3-997d-7afa0bde4403'
)

token = get_token(email, password, channel=channel)
response = stub.DeleteAnnouncement(request, metadata=[("authorization", f"Bearer {token}")])
print("DeleteAnnouncement Response:", response)
