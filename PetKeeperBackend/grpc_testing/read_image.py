from PIL import Image
import io


def read_image(path):
    with Image.open(path) as img:
        img_byte_arr = io.BytesIO()
        img.save(img_byte_arr, format='PNG')
        img_byte_arr.seek(0)
        png_bytes = img_byte_arr.read()
        return png_bytes
