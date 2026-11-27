using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCocina : MonoBehaviour
{
    public float speed = 6f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1f);
        controller.Move(dir * speed * Time.deltaTime);
    }
}
