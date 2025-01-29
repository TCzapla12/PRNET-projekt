import grpc
import user_pb2
import user_pb2_grpc
import sys
from auth.get import get_token
from user.create import users, get_user_ids
from animal.create import get_animal_ids
animal_ids = get_animal_ids()
user_ids = get_user_ids()


channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AnimalServiceStub(channel)

delete_requests = [
    user_pb2.AnimalMinimal(  # Delete Fafik
        id=animal_ids[0]
    ),
    user_pb2.AnimalMinimal(  # Try Deleting someone else's animal as non-admin (should fail)
        id=animal_ids[2],
        owner_id=user_ids[2]
    ),
    user_pb2.AnimalMinimal(  # Admin-delete animal (Iker of user[1])
        id=animal_ids[1],
        owner_id=user_ids[1]
    ),
]

for user, user_id, update_request in zip(users, user_ids, delete_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    try:
        update_response = stub.DeleteAnimal(update_request, metadata=[("authorization", f"Bearer {token}")])
        print("AnimalDelete Response:\n", update_response)
    except grpc.RpcError as e:
        print(f"Error: {e}", file=sys.stderr)

