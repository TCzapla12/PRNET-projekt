import grpc
import user_pb2
import user_pb2_grpc
import sys
from auth.get import get_token
from user.create import users, get_user_ids
from address.create import addresses, get_address_ids
primary_address_ids, secondary_address_ids = get_address_ids()
user_ids = get_user_ids()


channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AddressServiceStub(channel)

delete_requests = [
    user_pb2.AddressMinimal(  # Delete self secondary
        id=secondary_address_ids[0]
    ),
    user_pb2.AddressMinimal(  # Try Deleting someone else's address as a user (should fail)
        id=primary_address_ids[2],
        owner_id=user_ids[2]
    ),
    user_pb2.AddressMinimal(  # Try deleting own primary (should fail)
        id=primary_address_ids[2],
    ),
    user_pb2.AddressUpdate(  # Admin-delete secondary address of a user
        id=secondary_address_ids[1],
        owner_id=user_ids[1]
    ),
    user_pb2.AddressMinimal(  # Try deleting primary address of a different user as admin (should fail)
        id=primary_address_ids[1],
        owner_id=user_ids[1]
    )
]

for user, user_id, update_request in zip(users + [users[2], users[2]], user_ids + [user_ids[2], user_ids[2]], delete_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    try:
        update_response = stub.DeleteAddress(update_request, metadata=[("authorization", f"Bearer {token}")])
        print("AddressDelete Response:\n", update_response)
    except grpc.RpcError as e:
        print(f"Error: {e}", file=sys.stderr)

