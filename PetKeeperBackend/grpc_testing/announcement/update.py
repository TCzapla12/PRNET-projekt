import grpc
import user_pb2
import user_pb2_grpc
import sys
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

update_requests = [
    user_pb2.AnnouncementUpdate(
        id=announcement_ids[0],  # Belongs to user[1]
        keeper_profit=450,
        is_negotiable=False,
        address_id=secondary_address_ids[1]
    ),
    user_pb2.AnnouncementUpdate(  # Attempt updating other person's announcement (should fail)
        id=announcement_ids[1],  # Belongs to user[2]
        keeper_profit=200,
        description='Hax04',
        is_negotiable=True
    ),
    user_pb2.AnnouncementUpdate(  # Update other person's announcement as admin
        id=announcement_ids[1],
        end_term=int(datetime.strptime("2025-02-13 22:00:00", DATE_FORMAT).timestamp()),
        keeper_profit=80,
        is_negotiable=False  # Check if non-changing update is returned (should not be)
    ),
    # Attempt updating other person's announcement as admin, but set address not owned by this user (should fail)
    user_pb2.AnnouncementUpdate(
        id=announcement_ids[0],
        keeper_profit=200,
        description='Hax04',
        is_negotiable=True,
        address_id=primary_address_ids[2]
    ),
]

announcement_users = [users[1], users[1], users[2], users[2]]
announcement_user_ids = [user_ids[1], user_ids[1], user_ids[2], user_ids[2]]
for user, user_id, update_request in zip(announcement_users, announcement_user_ids, update_requests):
    token = get_token(user['email'], user['password'], channel=channel)
    try:
        update_response = stub.UpdateAnnouncement(update_request, metadata=[("authorization", f"Bearer {token}")])
        print("AnnouncementUpdate Response:\n", update_response)
    except grpc.RpcError as e:
        print(f"Error: {e}", file=sys.stderr)
