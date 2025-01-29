import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
from address.create import addresses, get_address_ids
from animal.create import animals, get_animal_ids
from announcement.create import announcements, get_announcement_ids
from datetime import datetime
DATE_FORMAT = "%Y-%m-%d %H:%M:%S"

user_ids = get_user_ids()
animal_ids = get_animal_ids()
primary_address_ids, secondary_address_ids = get_address_ids()
announcement_ids = get_announcement_ids()

channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AnnouncementServiceStub(channel)

# User context (admin or non-admin) doesn't matter in Get
get_requests = [
    user_pb2.AnnouncementGet(
        id=announcement_ids[1]
    ),
    user_pb2.AnnouncementGet(
        keeper_profit_more=200,
        is_negotiable=True
    ),
    user_pb2.AnnouncementGet(
        start_term_before=int(datetime.strptime("2025-02-20 00:00:00", DATE_FORMAT).timestamp())
    ),
    user_pb2.AnnouncementGet(  # Should return empty
        start_term_after=int(datetime.strptime("2025-02-20 00:00:00", DATE_FORMAT).timestamp())
    )
]


for usr, get_req in zip(users + [users[0]], get_requests):
    token = get_token(usr['email'], usr['password'], channel=channel)
    get_response = stub.GetAnnouncements(get_req, metadata=[("authorization", f"Bearer {token}")])
    print("GetAnnouncement Response:\n", get_response)
