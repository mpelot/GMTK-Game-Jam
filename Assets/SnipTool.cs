using UnityEngine;

public class SnipTool : MonoBehaviour
{
    public Hair hair;
    public GameObject hairPiece;

    public float speed = 5f;
    public float rotationSpeed = 5f;

    private bool isDragging = false;
    public CircleCollider2D circleCollider;
    public Rigidbody2D rb;
    private void Start()
    {
        hair = FindFirstObjectByType<Hair>();
    }
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(2))
        {
            if (!isDragging)
            {
                Collider2D collider = Physics2D.OverlapPoint(mousePosition);

                if (collider != null && collider == circleCollider)
                {
                    isDragging = true;
                    rb.isKinematic = true;

                }
            }
            else
            {
                isDragging = false;
                rb.isKinematic = false;
            }
            
        }
        // If space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Snip(2);
        }

        if (isDragging)
        {
            // Get the mouse position in world coordinates
            mousePosition.z = transform.position.z;

            // Move the GameObject towards the mouse position
            transform.position = Vector3.MoveTowards(transform.position, mousePosition, speed * Time.deltaTime);

            // Rotate left on left click and right on right click
            if (Input.GetMouseButton(0))
            {
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            }
            else if (Input.GetMouseButton(1))
            {
                transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
            }
        }
    }

    void Snip(int pixelSkip)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;

        GameObject spawnedHairPiece = Instantiate(hairPiece, transform.position, Quaternion.identity);
        HairPiece hairPieceScript = spawnedHairPiece.GetComponent<HairPiece>();

        // Get the texture data
        Texture2D texture = sprite.texture;
        Rect rect = sprite.textureRect;
        Vector2 pivot = sprite.pivot;

        // Iterate over each pixel in the sprite's texture
        for (int x = (int)rect.x; x < rect.x + rect.width; x += pixelSkip)
        {
            for (int y = (int)rect.y; y < rect.y + rect.height; y += pixelSkip)
            {
                Color pixelColor = texture.GetPixel(x, y);
                if (pixelColor.a > 0)
                {
                    // Get pixel's local position relative to the sprite's pivot
                    Vector2 localPos = new Vector2(x, y) - pivot;

                    // Scale local position according to sprite's pixels per unit
                    localPos /= sprite.pixelsPerUnit;

                    // Convert local position to world position
                    Vector3 worldPos = spriteRenderer.transform.TransformPoint(localPos);

                    Color color = hair.GetColorFromMaskTexture(worldPos);
                    Color white = Color.white;
                    white.a = 0;
                    if (color == white)
                    {
                        bool setHairPieceColor = hairPieceScript.SetColorOnMaskTexture(worldPos, Color.white, 5);
                    }
                }
            }
        }



        // Iterate over each pixel in the sprite's texture
        for (int x = (int)rect.x; x < rect.x + rect.width; x += pixelSkip)
        {
            for (int y = (int)rect.y; y < rect.y + rect.height; y += pixelSkip)
            {
                Color pixelColor = texture.GetPixel(x, y);
                if (pixelColor.a > 0)
                {
                    // Get pixel's local position relative to the sprite's pivot
                    Vector2 localPos = new Vector2(x, y) - pivot;

                    // Scale local position according to sprite's pixels per unit
                    localPos /= sprite.pixelsPerUnit;

                    // Convert local position to world position
                    Vector3 worldPos = spriteRenderer.transform.TransformPoint(localPos);

                    bool setColor = hair.SetColorOnMaskTexture(worldPos, Color.black, 5);
                }
            }
        }

        hair.ApplyMaskTextureChanges();
        hairPieceScript.ApplyMaskTextureChanges();
    }
}
