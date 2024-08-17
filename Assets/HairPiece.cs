using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairPiece : MonoBehaviour
{
    public Renderer objectRenderer;
    public Color32 startingMaskColor;
    public CircleCollider2D circleCollider;
    public Rigidbody2D rb;
    private bool isDragging = false;
    // Start is called before the first frame update
    void Awake()
    {
        CreateMaskTexture(GetComponent<SpriteRenderer>(), startingMaskColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (!isDragging)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D collider = Physics2D.OverlapPoint(mousePosition);

                if (collider == circleCollider)
                {
                    isDragging = true;
                    rb.isKinematic = true;
                }
            }
            else
            {
                isDragging = false;
            }
            
        }

        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 currentPosition = transform.position;

            // Smoothly move the object towards the mouse position
            float speed = 5f; // Adjust this value for faster or slower movement
            transform.position = Vector2.Lerp(currentPosition, mousePosition, speed * Time.deltaTime);
        }
    }

    void CreateMaskTexture(SpriteRenderer spriteRenderer, Color32 startingMaskColor)
    {
        Texture2D maskTexture = new Texture2D(spriteRenderer.sprite.texture.width, spriteRenderer.sprite.texture.height);
        Color32[] pixels = new Color32[maskTexture.width * maskTexture.height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = startingMaskColor;
        }
        maskTexture.SetPixels32(pixels);
        maskTexture.Apply();

        spriteRenderer.material.SetTexture("_MaskTex", maskTexture);
    }

    public Color GetColorFromMaskTexture(Vector3 worldPosition)
    {
        // Convert the world position to local position relative to the object
        Vector3 localPosition = objectRenderer.transform.InverseTransformPoint(worldPosition);

        // Get the material from the renderer
        Material material = objectRenderer.material;

        // Access the mask texture from the material
        Texture2D maskTexture = material.GetTexture("_MaskTex") as Texture2D;

        if (maskTexture == null)
        {
            Debug.LogError("Mask texture not found!");
            return Color.clear;
        }

        // Get the texture size and calculate texture coordinates
        Vector2 textureSize = new Vector2(maskTexture.width, maskTexture.height);

        // Assuming the object is a sprite, we need to calculate UV coordinates based on the local position
        // Adjust the following line based on how the UVs map to your object's mesh
        Vector2 texturePosition = new Vector2(
            (localPosition.x + 0.5f) * textureSize.x,
            (localPosition.y + 0.5f) * textureSize.y
        );

        // Convert texture coordinates to pixel coordinates
        int x = Mathf.FloorToInt(texturePosition.x);
        int y = Mathf.FloorToInt(texturePosition.y);

        if (x < 0 || x >= maskTexture.width || y < 0 || y >= maskTexture.height)
        {
            Debug.LogError("Pixel coordinates out of bounds!");
            return Color.clear;
        }

        // Get the pixel color from the mask texture
        Color color = maskTexture.GetPixel(x, y);

        return color;
    }

    public bool SetColorOnMaskTexture(Vector3 worldPosition, Color newColor, int radius)
    {
        // Convert the world position to local position relative to the object
        Vector3 localPosition = objectRenderer.transform.InverseTransformPoint(worldPosition);

        // Get the material from the renderer
        Material material = objectRenderer.material;

        // Access the mask texture from the material
        Texture2D maskTexture = material.GetTexture("_MaskTex") as Texture2D;

        if (maskTexture == null)
        {
            Debug.LogError("Mask texture not found!");
            return false;
        }

        // Get the texture size and calculate texture coordinates
        Vector2 textureSize = new Vector2(maskTexture.width, maskTexture.height);

        // Assuming the object is a sprite, we need to calculate UV coordinates based on the local position
        Vector2 texturePosition = new Vector2(
            (localPosition.x + 0.5f) * textureSize.x,
            (localPosition.y + 0.5f) * textureSize.y
        );

        // Convert texture coordinates to pixel coordinates
        int centerX = Mathf.FloorToInt(texturePosition.x);
        int centerY = Mathf.FloorToInt(texturePosition.y);

        // Check if the center coordinates are outside the texture bounds
        if (centerX < 0 || centerX >= maskTexture.width || centerY < 0 || centerY >= maskTexture.height)
        {
            // World position is outside of the texture bounds
            return false;
        }

        // Get the current color of the center pixel
        Color centerColor = maskTexture.GetPixel(centerX, centerY);

        // Iterate over the area within the radius, ensuring it's a circle
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                // Check if the pixel is within the circle defined by the radius
                if (x * x + y * y <= radius * radius)
                {
                    int pixelX = centerX + x;
                    int pixelY = centerY + y;

                    // Check if the pixel coordinates are within the texture bounds
                    if (pixelX >= 0 && pixelX < maskTexture.width && pixelY >= 0 && pixelY < maskTexture.height)
                    {
                        // Set the pixel color on the mask texture
                        maskTexture.SetPixel(pixelX, pixelY, newColor);
                    }
                }
            }
        }

        if (centerColor == newColor)
        {
            return false;
        }

        return true;
    }

    public void ApplyMaskTextureChanges()
    {
        // Get the material from the renderer
        Material material = objectRenderer.material;

        // Access the mask texture from the material
        Texture2D maskTexture = material.GetTexture("_MaskTex") as Texture2D;

        if (maskTexture == null)
        {
            Debug.LogError("Mask texture not found!");
            return;
        }

        // Apply the changes to the mask texture
        maskTexture.Apply();
    }

}
