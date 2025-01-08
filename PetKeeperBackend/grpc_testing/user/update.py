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
        #'username': 'kocham_koty76',
        'avatar_url': '/path/to/avatar.png'
    },
    {
        'user_id': user_pb2.UserIdentifier(id=ids[1]),
        #'email': 'jurek_nowak321333333@wp.pl',
        'phone': '504003002'
    },
    {
        'user_id': user_pb2.UserIdentifier(id=ids[2]),
        #'password': 'hihi_haha123'
    }
]

for usr, upd in zip(users, updates):
    update_request = user_pb2.UserUpdate(**upd)

    token = get_token(usr['email'], usr['password'], channel=channel)
    update_response = stub.UpdateUser(update_request, metadata=[("authorization", f"Bearer {token}")])
    print("UpdateUser Response:\n", update_response)

# Attempt privilege escalation
print("Attempting privilege escalation by sending update with is_admin: True")
updates[0].update({'is_admin': True, 'is_verified': True})
update_request = user_pb2.UserUpdate(**updates[0])
token = get_token(users[0]['email'], users[0]['password'], channel=channel)
update_response = stub.UpdateUser(update_request, metadata=[("authorization", f"Bearer {token}")])
print("UpdateUser Response:\n", update_response)
