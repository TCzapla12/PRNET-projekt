import grpc
import user_pb2
import user_pb2_grpc
from auth.get import get_token
from user.create import users, get_user_ids
from announcement.create import get_announcement_ids
import os
from datetime import datetime
DATE_FORMAT = "%Y-%m-%d %H:%M:%S"
script_dir = os.path.dirname(os.path.abspath(__file__))

announcement_ids = get_announcement_ids()
user_ids = get_user_ids()
# announcement[0] is users[1]
# announcement[1] is users[2]
# user[0] has no announcement

channel = grpc.insecure_channel('localhost:8080')
stub = user_pb2_grpc.AnnouncementServiceStub(channel)

status_requests = [
    user_pb2.AnnouncementUpdate(  # user[0] takes users[1] announcement
        id=announcement_ids[0],  # Belongs to user[1]
        status="pending"
    ),
    user_pb2.AnnouncementUpdate(  # User[1] removes user[0] from the announcement
        id=announcement_ids[0],  # Belongs to user[1]
        status="created"
    ),
    user_pb2.AnnouncementUpdate(  # user[0] takes users[1] announcement again :)
        id=announcement_ids[0],  # Belongs to user[1]
        status="pending"
    ),
    user_pb2.AnnouncementUpdate(  # User[1] accepts user[0] for the announcement
        id=announcement_ids[0],   # Belongs to user[1]
        status="ongoing"
    ),
]


if __name__ == "__main__":
    for usr_id, usr, update_req in zip([user_ids[0], user_ids[1], user_ids[0], user_ids[1]], [users[0], users[1], users[0], users[1]], status_requests):
        token = get_token(usr['email'], usr['password'], channel=channel)
        status_update_response = stub.UpdateAnnouncement(update_req, metadata=[("authorization", f"Bearer {token}")])
        print("AnnouncementUpdate Response:\n", status_update_response)
