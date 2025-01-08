import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from create import users, get_ids
ids = get_ids()

channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.UserServiceStub(channel)


updates = [
    {
        'user_id': user_pb2.UserIdentifier(id=ids[0]),
        'username': 'kocham_koty76',
    },
    {
        'user_id': user_pb2.UserIdentifier(id=ids[1]),
        'email': 'jurek_nowak321333333@wp.pl',
    },
    {
        'user_id': user_pb2.UserIdentifier(id=ids[2]),
        'username': 'bizbarborber',
        'password_hash': 'hihi_haha123'  # Only bool true should be returned, not password or password hash
    }
]

for usr, upd in zip(users, updates):
    update_request = user_pb2.UserUpdateCredentials(**upd)

    token = get_token(usr['email'], usr['password'], channel=channel)
    update_response = stub.UpdateUserCredentials(update_request, metadata=[("authorization", f"Bearer {token}")])
    print("UpdateUserCredentials Response:\n", update_response)

