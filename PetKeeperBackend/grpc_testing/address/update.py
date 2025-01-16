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

update_requests = [
    user_pb2.AddressUpdate(  # Full update
        id=primary_address_ids[0],
        city='Nasielsk',
        street='Bacha',
        house_number='8',
        apartment_number='10',
        post_code='03-444',
        description='Blok - proszę dzwonić domofonem 8a kluczyk 10'
    ),
    user_pb2.AddressUpdate(  # Try update someone else's address as non-admin
        id=primary_address_ids[0],
        city='PwnCity',
        street='Hax0r',
        house_number='1337',
        apartment_number='1337',
        post_code='13-337',
        description='hacked!'
    ),
    user_pb2.AddressUpdate(  # Update someone else's address as admin (user[2] is admin)
        id=secondary_address_ids[1],
        description='Legendarna patelnia w Warszawie, metro centrum'
    )
]

for user, user_id, update_request in zip(users, user_ids, update_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    try:
        update_response = stub.UpdateAddress(update_request, metadata=[("authorization", f"Bearer {token}")])
        print("AddressUpdate Response:\n", update_response)
    except grpc.RpcError as e:
        print(f"Error: {e}", file=sys.stderr)

