import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
user_ids = get_user_ids()

channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AnimalServiceStub(channel)

get_requests = [
    user_pb2.AnimalGet(  # Either name or type here
        #name='Mruczek',
        type='cat',
    ),
    user_pb2.AnimalGet(owner_id=''),  # Should get all animal of user[1] (self)
    user_pb2.AnimalGet(owner_id=user_ids[0]),  # Check if user[2] can get user[0] animals
]

# The zip makes the last request be made by the users[0]
for user, user_id, request in zip(users + [users[0]], user_ids + [user_ids[0]], get_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    get_response = stub.GetAnimals(request, metadata=[("authorization", f"Bearer {token}")])

    print("AnimalGet Response:\n", get_response, "\n" + "-"*100, "\n\n")
