import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
user_ids = get_user_ids()

channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AddressServiceStub(channel)

# owner_id needs to be set to empty string if we want addresses only belonging to requesting user
# Addresses are not considered safe information, so users can retrieve addresses not belonging to them
get_requests = [
    user_pb2.AddressGet(  # Either city or street only here. Comment out the other
        #city='Warszawa',
        street='Łączna',
    ),
    user_pb2.AddressGet(owner_id=''),  # Should get all addresses of user[1]
    user_pb2.AddressGet(owner_id=user_ids[0]),  # Check if user[2] can get user[0] addresses
    user_pb2.AddressGet(is_primary=True)
]

# The zip makes the last request be made by the users[0]
for user, user_id, request in zip(users + [users[0]], user_ids + [user_ids[0]], get_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    get_response = stub.GetAddresses(request, metadata=[("authorization", f"Bearer {token}")])

    print("AddressGet Response:\n", get_response, "\n" + "-"*100, "\n\n")
