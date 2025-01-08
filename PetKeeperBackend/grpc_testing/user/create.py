import grpc
import user_pb2
import user_pb2_grpc

addresses = [
    {
        'street': 'Przykladowa',
        'house_number': '2',
        'city': 'Warszawa',
        'post_code': '02-356'
    },
    {
        'street': 'Wyjazdowa',
        'house_number': '5',
        'city': 'Zielona Góra',
        'post_code': '37-123'
    },
    {
        'street': 'Porzucona',
        'house_number': '1',
        'apartment_number': '22',
        'city': 'Słupsk',
        'post_code': '07-754'
    }
]

users = [
    {
        'username': 'kocham_psy123',
        'email': 'psy56123@gmail.com',
        'password': 'p13s-l0ve',
        'first_name': 'Anna',
        'last_name': 'Kowalska',
        'primary_address': addresses[0],
        'phone': '123789456',
        'pesel': '00301309034',
        'document_url': ['local/kocam_psy123/id0.png', 'local/kocam_psy123/id1.png']
    },
    {
        'username': 'jurek57621',
        'email': 'jurek57621@o2.pl',
        'password': 'wanda14071987',
        'first_name': 'Jerzy',
        'last_name': 'Nowak',
        'primary_address': addresses[1],
        'phone': '503000001',
        'pesel': '63071609033',
        'document_url': ['local/jurek57621/id0.png', 'local/jurek57621/id1.png']
    },
    {
        'username': 'cheepoi',
        'email': '783aaa3@hotmail.com',
        'password': '734HuMZxx@690',
        'first_name': 'John',
        'last_name': 'Doe',
        'primary_address': addresses[2],
        'phone': '976304999',
        'pesel': '78112345678',
        'document_url': ['local/cheepoi/id0.png', 'local/cheepoi/id1.png']
    }
]


def get_ids():
    with open('uids.txt', 'r') as fid:
        return [line.strip() for line in fid.readlines()]


###
if __name__ == "__main__":
    ids = []
    channel = grpc.insecure_channel('localhost:8080')
    stub = user_pb2_grpc.UserServiceStub(channel)
    for usr, addr in zip(users, addresses):
        address_proto = user_pb2.AddressCreate(**addr)
        user_proto = user_pb2.UserCreate(**usr)

        create_response = stub.CreateUser(user_proto)
        print("CreateUser Response\n", create_response)
        ids.append(create_response.id)

    with open('uids.txt', 'w') as fid:
        fid.writelines(i + '\n' for i in ids)

