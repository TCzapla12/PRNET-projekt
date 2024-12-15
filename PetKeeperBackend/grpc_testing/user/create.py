import grpc
import user_pb2
import user_pb2_grpc
# Create a channel to connect to the .NET server
channel = grpc.insecure_channel('localhost:8080')  # Update with your server's address if needed
stub = user_pb2_grpc.UserServiceStub(channel)  # Replace Greeter with your service name

address = user_pb2.AddressCreate(
    street='Przykladowa',
    house_number='2',
    city='Warszawa',
    post_code='02-285',
    owner_id='0'
)
# 1. Create a new user
create_request = user_pb2.UserCreate(
    username="new_user2222",
    email="new_user22222@example.com",
    password="securepassword123",
    first_name='Jan',
    last_name='Kowalski',
    primary_address=address,
    phone='123456789',
    pesel='00112233445',
    document_url=['foo', 'baz']
)
create_response = stub.CreateUser(create_request)
print("CreateUser Response:", create_response)
