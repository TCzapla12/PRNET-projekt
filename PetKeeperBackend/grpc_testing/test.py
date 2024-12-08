from wand.image import Image
from wand.color import Color
import wand


def remove_white_background_with_wand(image_path, output_path):
    """
    Removes white background using the Wand library.

    :param image_path: Path to the input image.
    :param output_path: Path to save the output image.
    """
    with Image(filename=image_path) as img:
        # Add an alpha channel (transparency)
        img.alpha_channel = 'activate'

        # Use a white color as the target for transparency
        with Color("white") as white_color:
            img.transparent_color(white_color, alpha=0.0, fuzz=0.05)  # Fuzz determines tolerance

        # Save the resulting image
        img.save(filename=output_path)
        print(f"Processed image saved to {output_path}")

print(wand.__version__)
# Example usage
remove_white_background_with_wand("C:\\Users\\batru\\Desktop\\balatro_cards\\hearts2.webp", "C:\\Users\\batru\\Desktop\\heartsA_transparent.png")