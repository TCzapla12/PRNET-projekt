import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
# Create a channel to connect to the .NET server

email = "new_user22222@example.com"
password = "securepassword123"
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.AnnouncementServiceStub(channel)  # Replace Greeter with your service name

create_request = user_pb2.AnnouncementCreate(
    keeper_id='076277c2-aa04-4b62-8da5-f6e2cd78e753',
    animal_id='040825f2-9d12-47f1-9c03-6b712c872917',
    keeper_profit=1000,
    is_negotiable=False,
    description='Wyjeżdżam na wakację. Potrzebuję osoby do zajęcia się przemiłym fafikiem na 3 dni :D',
    start_term=1735340464,
    end_term=1735682464,
    address_id='ac51df27-96b1-4284-8039-4849c80cf441'
)

token = get_token(email, password, channel=channel)
create_response = stub.CreateAnnouncement(create_request, metadata=[("authorization", f"Bearer {token}")])
print("CreateAnnouncement Response:", create_response)
