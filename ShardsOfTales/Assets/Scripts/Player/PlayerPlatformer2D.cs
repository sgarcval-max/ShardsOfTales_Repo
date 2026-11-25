using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPlatformer2D : MonoBehaviour
{
    public float speed = 6f;
    public float jumpHeight = 2.5f;
    public float gravity = -25f;
    public float climbSpeed = 4f;
    public SpriteRenderer sprite;

    private CharacterController controller;
    private float verticalVelocity;
    private bool onLadder;
    private bool climbing;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void SetOnLadder(bool value)
    {
        onLadder = value;
        if (!value) climbing = false;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (onLadder && v != 0f && h == 0f)
            climbing = true;

        if (climbing && h != 0f)
            climbing = false;

        if (climbing && controller.isGrounded && v < 0f)
            climbing = false;

        if (climbing)
        {
            verticalVelocity = v * climbSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                climbing = false;
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity += gravity * Time.deltaTime;
        }

        float horizontalMove = climbing ? 0f : h * speed;
        Vector3 move = new Vector3(horizontalMove, verticalVelocity, 0f);
        controller.Move(move * Time.deltaTime);

        if (sprite != null && h != 0f)
            sprite.flipX = h < 0f;
    }
}