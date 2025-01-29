import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
import os
script_dir = os.path.dirname(os.path.abspath(__file__))
user_ids = get_user_ids()


channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AddressServiceStub(channel)

addresses = [
    {
        'street': 'Długa',
        'house_number': '30',
        'apartment_number': '27',
        'city': 'Raciniewo',
        'post_code': '86-260'
    },
    {
        'street': 'Aleje Jerozolimskie',
        'house_number': '46',
        'apartment_number': '1',
        'city': 'Warszawa',
        'post_code': '00-024'
    },
    {
        'street': 'Łączna',
        'house_number': '43',
        'city': 'Lipinki Łużyckie',
        'post_code': '68-213',
        'description': 'tutaj jak się wjeżdża zaraz koło poczty objazd tutaj'
    }
]


def get_address_ids():
    # Returns tuple: (primary_addresses_ids, secondary_addresses_ids)
    with open(os.path.join(script_dir, 'secondary_addr_ids.txt'), 'r') as fs, \
         open(os.path.join(script_dir, 'primary_addr_ids.txt'), 'r') as fp:
        return [line.strip() for line in fp.readlines()], [line.strip() for line in fs.readlines()]


if __name__ == "__main__":
    ids = []
    for usr_id, usr, addr in zip(user_ids, users, addresses):
        create_request = user_pb2.AddressCreate(**addr)

        token = get_token(usr['email'], usr['password'], channel=channel)
        create_response = stub.CreateAddress(create_request, metadata=[("authorization", f"Bearer {token}")])
        ids.append(create_response.id)
        print("AddressCreate Response:\n", create_response)

    with open('secondary_addr_ids.txt', 'w') as f:
        f.writelines(i + '\n' for i in ids)
